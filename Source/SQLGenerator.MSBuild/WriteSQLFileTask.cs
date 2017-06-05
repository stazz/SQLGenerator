/*
 * Copyright 2017 Stanislav Muhametsin. All rights Reserved.
 *
 * Licensed  under the  Apache License,  Version 2.0  (the "License");
 * you may not use  this file  except in  compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed  under the  License is distributed on an "AS IS" BASIS,
 * WITHOUT  WARRANTIES OR CONDITIONS  OF ANY KIND, either  express  or
 * implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using TNuGetPackageResolverCallback = System.Func<System.String, System.String, System.String, System.Threading.Tasks.Task<System.Reflection.Assembly>>;
using TAssemblyByPathResolverCallback = System.Func<System.String, System.Reflection.Assembly>;
using SQLGenerator.Usage;
using Microsoft.Build.Framework;
using UtilPack;
using System.Threading;

namespace SQLGenerator.MSBuild
{
   public class WriteSQLFileTask : Microsoft.Build.Utilities.Task, ICancelableTask
   {
      private readonly TNuGetPackageResolverCallback _loadNuGetPackageAssembly;
      private readonly TAssemblyByPathResolverCallback _loadAssembly;

      private readonly CancellationTokenSource _cancellationSource;

      public WriteSQLFileTask(
         TNuGetPackageResolverCallback loadNuGetPackageAssembly,
         TAssemblyByPathResolverCallback loadAssembly
         )
      {
         this._loadNuGetPackageAssembly = loadNuGetPackageAssembly;
         this._loadAssembly = loadAssembly;
         this._cancellationSource = new CancellationTokenSource();
      }

      public void Cancel()
      {
         this._cancellationSource.Cancel( false );
      }

      public override Boolean Execute()
      {
         // Reacquire must be called in same thread as yield -> run our Task synchronously
         var retVal = false;
         var yieldCalled = false;
         var be = (IBuildEngine3) this.BuildEngine;
         try
         {
            try
            {
               if ( !this.RunSynchronously )
               {
                  be.Yield();
                  yieldCalled = true;
               }
               this.ExecuteTaskAsync().GetAwaiter().GetResult();

               retVal = !this.Log.HasLoggedErrors;
            }
            catch ( OperationCanceledException )
            {
               // Canceled, do nothing
            }
            catch ( Exception exc )
            {
               // Only log if we did not receive cancellation
               if ( !this._cancellationSource.IsCancellationRequested )
               {
                  this.Log.LogErrorFromException( exc );
               }
            }
         }
         finally
         {
            if ( yieldCalled )
            {
               be.Reacquire();
            }
         }
         return retVal;
      }

      private async Task ExecuteTaskAsync()
      {
         Encoding encoding;
         var encodingName = this.FileEncoding;
         if ( String.IsNullOrEmpty( encodingName ) )
         {
            encoding = Encoding.UTF8;
         }
         else
         {
            encoding = Encoding.GetEncoding( encodingName );
         }

         var encodingInfo = encoding.CreateDefaultEncodingInfo();
         var vendor = await this.LoadDynamically<SQLVendor>( this.VendorPackageID, this.VendorPackageVersion, this.VendorAssemblyPath, this.VendorTypeName );
         if ( vendor != null )
         {
            var user = await this.LoadDynamically<SQLGeneratorUser>( this.GeneratorPackageID, this.GeneratorPackageVersion, this.GeneratorAssemblyPath, this.GeneratorTypeName );
            if ( user != null )
            {
               var path = Path.GetFullPath( this.OutputFile );
               using ( var fs = File.Open( path, FileMode.Create, FileAccess.Write, FileShare.None ) )
               {
                  var streamWriter = StreamFactory.CreateUnlimitedWriter( fs, this._cancellationSource.Token );
                  var writer = WriterFactory.CreateTransformableWriter<String, StreamWriterWithResizableBuffer, IEnumerable<Char>>(
                     new StreamCharacterWriter( encodingInfo, 1024 ),
                     streamWriter,
                     ToCharEnumerable
                     );
                  foreach ( var sql in user.GenerateSQL( vendor ) )
                  {
                     await writer.TryWriteAsync( sql );
                  }
                  await streamWriter.FlushAsync();
               }

               this.Log.LogMessage( MessageImportance.High, $"Successfully wrote SQL file \"{path}\"." );
            }
         }
      }

      public Boolean RunSynchronously { get; set; }

      public String VendorPackageID { get; set; }
      public String VendorPackageVersion { get; set; }
      public String VendorAssemblyPath { get; set; }
      public String VendorTypeName { get; set; }

      public String GeneratorPackageID { get; set; }
      public String GeneratorPackageVersion { get; set; }
      public String GeneratorAssemblyPath { get; set; }
      public String GeneratorTypeName { get; set; }

      [Required]
      public String OutputFile { get; set; }
      public String FileEncoding { get; set; }


      private async ValueTask<Object> LoadDynamically(
         String packageID,
         String packageVersion,
         String assemblyPath,
         String typeName,
         Type parentType
         )
      {
         Assembly assembly;
         Object retVal = null;
         if ( !String.IsNullOrEmpty( packageID ) )
         {
            assembly = await this._loadNuGetPackageAssembly( packageID, packageVersion, assemblyPath );
         }
         else if ( String.IsNullOrEmpty( packageVersion ) && !String.IsNullOrEmpty( assemblyPath ) && Path.IsPathRooted( assemblyPath ) )
         {
            assembly = this._loadAssembly( assemblyPath );
         }
         else
         {
            assembly = null;
         }

         if ( assembly != null )
         {
            var parent = parentType.GetTypeInfo();
            var checkParentType = !String.IsNullOrEmpty( typeName );
            Type targetType;
            if ( checkParentType )
            {
               // Instantiate directly
               targetType = assembly.GetType( typeName, false, false );
            }
            else
            {
               // Search for first available
               targetType = assembly.DefinedTypes.FirstOrDefault( t => !t.IsInterface && !t.IsAbstract && parent.IsAssignableFrom( t ) )?.AsType();
            }

            if ( targetType != null )
            {
               if ( !checkParentType || parent.IsAssignableFrom( targetType.GetTypeInfo() ) )
               {
                  // All checks passed, instantiate the target
                  retVal = Activator.CreateInstance( targetType );
               }
               else
               {
                  this.Log.LogError( $"The type \"{targetType.FullName}\" in \"{assembly}\" does not have required parent type \"{parentType.FullName}\"." );
               }
            }
            else
            {
               this.Log.LogError( $"Failed to find type within assembly in \"{assembly}\", try specify type name explicitly." );
            }
         }
         else
         {
            this.Log.LogError( $"Failed to load with given information: (id={packageID}, version={packageVersion}, path={assemblyPath}). Check that either at least package ID is present, or that assembly path is rooted." );
         }

         return retVal;
      }

      private async ValueTask<T> LoadDynamically<T>(
         String packageID,
         String packageVersion,
         String assemblyPath,
         String typeName
         )
      {
         return (T) ( await this.LoadDynamically( packageID, packageVersion, assemblyPath, typeName, typeof( T ) ) );
      }

      private static IEnumerable<Char> ToCharEnumerable( String str )
      {
         var max = str.Length;
         if ( max > 0 )
         {
            // Trim start and end whitespaces
            var i = 0;
            while ( Char.IsWhiteSpace( str[i] ) && ++i < max ) ;
            --max;
            while ( Char.IsWhiteSpace( str[max] ) && --max >= i ) ;

            if ( i <= max )
            {
               Char lastSeenChar = '\0';
               while ( i <= max )
               {
                  yield return ( lastSeenChar = str[i++] );
               }
               if ( lastSeenChar != ';' )
               {
                  yield return ';';
               }
               yield return '\n';
            }
         }
      }
   }
}
