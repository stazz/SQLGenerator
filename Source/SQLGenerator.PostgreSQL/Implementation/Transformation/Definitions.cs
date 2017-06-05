/*
 * Copyright 2013 Stanislav Muhametsin. All rights Reserved.
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
using System.Linq;
using System.Text;
using SQLGenerator.Implementation.Data;
using SQLGenerator.Implementation.Transformation;

namespace SQLGenerator.PostgreSQL.Implementation.Transformation
{
   internal class PgSQLColumnDefinitionProcessor : ColumnDefinitionProcessor
   {
      private static readonly IDictionary<Type, String> DEFAULT_DATA_TYPE_SERIAL_NAMES;

      static PgSQLColumnDefinitionProcessor()
      {
         var dic = new Dictionary<Type, String>( 3 );
         dic.Add( typeof( SQLDTBigIntImpl ), "BIGSERIAL" );
         dic.Add( typeof( SQLDTIntegerImpl ), "SERIAL" );
         dic.Add( typeof( SQLDTSmallIntImpl ), "SMALLSERIAL" );

         DEFAULT_DATA_TYPE_SERIAL_NAMES = dic;
      }

      private readonly IDictionary<Type, String> _dataTypeSerialNames;

      internal PgSQLColumnDefinitionProcessor( IDictionary<AutoGenerationPolicy, String> autoGenPolicies = null, IDictionary<Type, String> dataTypeSerialNames = null )
         : base( autoGenPolicies )
      {
         this._dataTypeSerialNames = dataTypeSerialNames ?? DEFAULT_DATA_TYPE_SERIAL_NAMES;
      }

      protected override void ProcessDataType( SQLProcessorAggregator aggregator, ColumnDefinition obj, StringBuilder builder )
      {
         if ( obj.AutoGenerationPolicy.HasValue )
         {
            // PostgreSQL can't handle the ALWAYS strategy
            if ( AutoGenerationPolicy.ByDefault == obj.AutoGenerationPolicy.Value )
            {
               // Don't produce default data type if auto generated
               String dtStr;
               if ( this._dataTypeSerialNames.TryGetValue( obj.DataType.GetType(), out dtStr ) )
               {
                  builder.Append( dtStr );
               }
               else
               {
                  throw new NotSupportedException( "Unsupported column data type " + obj.DataType + " for auto-generated column." );
               }
            }
            else
            {
               throw new NotSupportedException( "Unsupported auto generation policy: " + obj.AutoGenerationPolicy.Value + "." );
            }
         }
         else
         {
            base.ProcessDataType( aggregator, obj, builder );
         }
      }

      protected override void ProcessAutoGenerationPolicy( ColumnDefinition obj, StringBuilder builder )
      {
         // Nothing to do - auto generation policy handled in data type processing
      }
   }
}
