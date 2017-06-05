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

namespace SQLGenerator.PostgreSQL.Implementation.Data
{
   internal class PgSQLDropTableOrViewStatementImpl : DropTableOrViewStatementImpl, PgSQLDropTableOrViewStatement
   {
      private readonly Boolean _useIfExists;

      internal PgSQLDropTableOrViewStatementImpl( PostgreSQLVendorImpl vendor, DropBehaviour db, ObjectType whatToDrop, TableNameDirect table, Boolean useIfExists )
         : base( vendor, db, whatToDrop, table )
      {
         this._useIfExists = useIfExists;
      }

      #region PgSQLDropTableOrViewStatement Members

      public Boolean UseIfExists
      {
         get
         {
            return this._useIfExists;
         }
      }

      #endregion

   }
}
