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
using SQLGenerator;

namespace SQLGenerator.PostgreSQL
{
   public interface PgSQLSpecificFactory
   {
      SQLDTText NewTextDataType();
      PgSQLDropTableOrViewStatement NewDropTableOrViewStatement( TableNameDirect tableName, ObjectType theType, DropBehaviour dropBehaviour, Boolean useIfExists = true );
      PgSQLInsertStatement NewInsertStatement( TableNameDirect table, ColumnSource columnSource, SelectColumnClause returning = null );
      PgSQLInsertStatementBuilder NewInsertStatementBuilder();
      ColumnNameListExpression NewColumnNameListExpression( ImmutableArray<String> columnNames );
      ValuesExpression NewValuesExpression( ImmutableArray<ImmutableArray<ValueExpression>> values );
   }
}

public static class PgSQLFactoryExtensions
{
   public static TableDefinition NewTableDefinitionPgSQL( this DefinitionFactory factory, TableNameDirect tableName, TableContentsSource contents, TableScope? tableScope = null, PgSQLTableCommitAction? commitAction = null )
   {
      return factory.NewTableDefinition( tableName, contents, tableScope, (TableCommitAction) commitAction );
   }

   public static ColumnNameListExpression NewColumnNameListExpression( this PgSQLSpecificFactory factory, params String[] columnNames )
   {
      return factory.NewColumnNameListExpression( columnNames.NewAQ() );
   }

   public static ValuesExpression NewValuesExpression( this PgSQLSpecificFactory factory, params ValueExpression[] values )
   {
      return factory.NewValuesExpression( new[] { values.NewAQ() }.NewAQ( false ) );
   }

   public static ValuesExpression NewValuesExpressionMultiple( this PgSQLSpecificFactory factory, params ValueExpression[][] values )
   {
      return factory.NewValuesExpression( values.Select( v => v.NewAQ() ).NewAQ() );
   }
}