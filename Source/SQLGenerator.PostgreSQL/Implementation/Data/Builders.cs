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
using SQLGenerator.PostgreSQL;
using SQLGenerator.Implementation.Data;

namespace SQLGenerator.PostgreSQL.Implementation.Data
{
   internal class PgSQLInsertStatementBuilderImpl : InsertStatementBuilderImpl, PgSQLInsertStatementBuilder
   {
      private SelectColumnClause _returning;

      internal PgSQLInsertStatementBuilderImpl( PostgreSQLVendorImpl vendor )
         : base( vendor )
      {

      }

      #region PgSQLInsertStatementBuilder Members

      public PgSQLInsertStatementBuilder SetReturningClause( SelectColumnClause clause )
      {
         this._returning = clause;
         return this;
      }

      #endregion


   }
}
