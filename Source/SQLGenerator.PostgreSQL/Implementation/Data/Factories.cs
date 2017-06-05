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
using SQLGenerator.Implementation;
using SQLGenerator.Implementation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGenerator.PostgreSQL.Implementation.Data
{
   internal class PgSQLSpecificFactoryImpl : AbstractSQLFactoryImpl, PgSQLSpecificFactory
   {
      private readonly SQLDTText _text;

      internal PgSQLSpecificFactoryImpl( PostgreSQLVendorImpl vendor )
         : base( vendor )
      {
         this._text = new SQLDTTextImpl( vendor );
      }

      #region PgSQLSpecificFactory Members

      public SQLDTText NewTextDataType()
      {
         return this._text;
      }

      public PgSQLDropTableOrViewStatement NewDropTableOrViewStatement( TableNameDirect tableName, ObjectType theType, DropBehaviour dropBehaviour, Boolean useIfExists = true )
      {
         return new PgSQLDropTableOrViewStatementImpl( (PostgreSQLVendorImpl) this.vendor, dropBehaviour, theType, tableName, useIfExists );
      }

      public PgSQLInsertStatement NewInsertStatement( TableNameDirect table, ColumnSource columnSource, SelectColumnClause returning = null )
      {
         return new PgSQLInsertStatementImpl( (PostgreSQLVendorImpl) this.vendor, table, columnSource, returning );
      }

      public PgSQLInsertStatementBuilder NewInsertStatementBuilder()
      {
         return new PgSQLInsertStatementBuilderImpl( (PostgreSQLVendorImpl) this.vendor );
      }

      public ColumnNameListExpression NewColumnNameListExpression( ImmutableArray<String> columnNames )
      {
         return new ColumnNameListExpressionImpl( (PostgreSQLVendorImpl) this.vendor, columnNames );
      }

      public ValuesExpression NewValuesExpression( ImmutableArray<ImmutableArray<ValueExpression>> values )
      {
         return new ValuesExpressionImpl( (PostgreSQLVendorImpl) this.vendor, values );
      }

      #endregion

   }

   internal class PgSQLManipulationFactoryImpl : ManipulationFactoryImpl
   {
      internal PgSQLManipulationFactoryImpl( PostgreSQLVendorImpl vendor )
         : base( vendor )
      {
      }


      public override DropTableOrViewStatement NewDropTableOrViewStatement( TableNameDirect tableName, ObjectType theType, DropBehaviour dropBehaviour )
      {
         return ( (PostgreSQLVendor) this.vendor ).PgSQLSpecificFactory.NewDropTableOrViewStatement( tableName, theType, dropBehaviour );
      }
   }

   internal class PgSQLModificationFactoryImpl : ModificationFactoryImpl
   {
      internal PgSQLModificationFactoryImpl( SQLVendorImpl vendor )
         : base( vendor )
      {
      }

      public override InsertStatementBuilder NewInsertStatementBuilder()
      {
         return ( (PostgreSQLVendor) this.vendor ).PgSQLSpecificFactory.NewInsertStatementBuilder();
      }

      public override InsertStatement NewInsertStatement( TableNameDirect table, ColumnSource columnSource )
      {
         return ( (PostgreSQLVendor) this.vendor ).PgSQLSpecificFactory.NewInsertStatement( table, columnSource );
      }
   }
}
