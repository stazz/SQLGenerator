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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLGenerator.PostgreSQL;
using UtilPack;
using SQLGenerator.Implementation.Data;

namespace SQLGenerator.PostgreSQL.Implementation.Data
{
   internal class ColumnNameListExpressionImpl : ColumnNameListImpl, ColumnNameListExpression
   {
      internal ColumnNameListExpressionImpl( PostgreSQLVendorImpl vendor, ImmutableArray<String> names )
         : base( vendor, names )
      {

      }
   }

   internal class ValuesExpressionImpl : SQLElementBase, ValuesExpression
   {
      private readonly ImmutableArray<ImmutableArray<ValueExpression>> _values;

      internal ValuesExpressionImpl( PostgreSQLVendorImpl vendor, ImmutableArray<ImmutableArray<ValueExpression>> values )
         : base( vendor )
      {
         values.ValidateNotEmpty( nameof( values ) );

         this._values = values;
      }

      public ImmutableArray<ImmutableArray<ValueExpression>> Values
      {
         get
         {
            return this._values;
         }
      }
   }
}
