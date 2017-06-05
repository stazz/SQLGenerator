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
using System.Threading;
using SQLGenerator.Implementation.Transformation;
using SQLGenerator.Implementation.Data;

namespace SQLGenerator.Implementation
{

   public abstract class SQLVendorImpl : SQLVendor
   {
      private readonly Lazy<CommonFactory> _commonFactory;
      private readonly Lazy<DefinitionFactory> _definitionFactory;
      private readonly Lazy<ManipulationFactory> _manipulationFactory;
      private readonly Lazy<ModificationFactory> _modificationFactory;
      private readonly Lazy<QueryFactory> _queryFactory;

      protected SQLVendorImpl(
         IDictionary<Type, SQLProcessor> processors = null,
         Func<SQLVendorImpl, CommonFactory> cf = null,
         Func<SQLVendorImpl, DefinitionFactory> df = null,
         Func<SQLVendorImpl, ManipulationFactory> maf = null,
         Func<SQLVendorImpl, ModificationFactory> mof = null,
         Func<SQLVendorImpl, QueryFactory> qf = null
         )
      {
         this.Processor = new SQLProcessorAggregator( this, processors );
         this._commonFactory = new Lazy<CommonFactory>( () => cf?.Invoke( this ) ?? new CommonFactoryImpl( this ), LazyThreadSafetyMode.ExecutionAndPublication );
         this._definitionFactory = new Lazy<DefinitionFactory>( () => df?.Invoke( this ) ?? new DefinitionFactoryImpl( this ), LazyThreadSafetyMode.ExecutionAndPublication );
         this._manipulationFactory = new Lazy<ManipulationFactory>( () => maf?.Invoke( this ) ?? new ManipulationFactoryImpl( this ), LazyThreadSafetyMode.ExecutionAndPublication );
         this._modificationFactory = new Lazy<ModificationFactory>( () => mof?.Invoke( this ) ?? new ModificationFactoryImpl( this ), LazyThreadSafetyMode.ExecutionAndPublication );
         this._queryFactory = new Lazy<QueryFactory>( () => qf?.Invoke( this ) ?? new QueryFactoryImpl( this ), LazyThreadSafetyMode.ExecutionAndPublication );
      }

      protected SQLProcessorAggregator Processor { get; }

      #region SQLVendor Members

      public CommonFactory CommonFactory
      {
         get
         {
            return this._commonFactory.Value;
         }
      }

      public DefinitionFactory DefinitionFactory
      {
         get
         {
            return this._definitionFactory.Value;
         }
      }

      public ManipulationFactory ManipulationFactory
      {
         get
         {
            return this._manipulationFactory.Value;
         }
      }

      public ModificationFactory ModificationFactory
      {
         get
         {
            return this._modificationFactory.Value;
         }
      }

      public QueryFactory QueryFactory
      {
         get
         {
            return this._queryFactory.Value;
         }
      }

      #endregion

      public String ToString( SQLElementBase element )
      {
         var b = new StringBuilder( 20 );
         this.Processor.Process( element, b, false );
         return b.ToString();
      }
   }
}
