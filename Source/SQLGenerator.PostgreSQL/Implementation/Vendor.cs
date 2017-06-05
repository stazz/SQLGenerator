/*
 * Copyright 2014 Stanislav Muhametsin. All rights Reserved.
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
using SQLGenerator.Implementation;
using SQLGenerator.Implementation.Data;
using SQLGenerator.Implementation.Transformation;
using SQLGenerator.PostgreSQL.Implementation.Data;
using SQLGenerator.PostgreSQL.Implementation.Transformation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLGenerator.PostgreSQL.Implementation
{
   public class PostgreSQLVendorImpl : SQLVendorImpl, PostgreSQLVendor
   {

      public PostgreSQLVendorImpl()
         : base(
         PgSQLProcessors.DEFAULT_PROCESSORS,
         null,
         null,
         vendor => new PgSQLManipulationFactoryImpl( (PostgreSQLVendorImpl) vendor ),
         vendor => new PgSQLModificationFactoryImpl( vendor ) )
      {
         this.PgSQLSpecificFactory = new PgSQLSpecificFactoryImpl( this );
         this.LegacyOffsetAndLimit = false;
      }

      public PgSQLSpecificFactory PgSQLSpecificFactory { get; }

      public Boolean LegacyOffsetAndLimit { get; set; }
   }
}
