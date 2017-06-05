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
using SQLGenerator.PostgreSQL.Implementation.Data;

namespace SQLGenerator.PostgreSQL.Implementation.Transformation
{
   internal class PgSQLProcessors
   {
      internal static readonly IDictionary<Type, SQLProcessor> DEFAULT_PROCESSORS;

      static PgSQLProcessors()
      {
         var processors = SQLProcessorAggregator.GetDefaultProcessorsCopy();

         // Override default processor for date-time
         processors[typeof( TimestampLiteralImpl )] = new PgSQLTimestampLiteralProcessor();

         // Override default processor for column definition
         processors[typeof( ColumnDefinitionImpl )] = new PgSQLColumnDefinitionProcessor();

         // Override default processor for regexp predicates
         processors[typeof( RegexpPredicateImpl )] = new BinaryPredicateProcessor( "~", "!~" );

         // Override default processor for query specification
         processors[typeof( QuerySpecificationImpl )] = new PgSQLQuerySpecificationProcessor();

         // Override default processor for table definition by adding support for table commit action
         var tableCommitActions = TableDefinitionProcessor.NewCopyOfDefaultCommitActions();
         tableCommitActions.Add( (TableCommitAction) PgSQLTableCommitAction.Drop, "DROP" );
         processors[typeof( TableDefinitionImpl )] = new TableDefinitionProcessor( null, tableCommitActions );

         // Add support for TEXT datatype
         processors.Add( typeof( SQLDTTextImpl ), new ConstantProcessor( "TEXT" ) );

         // Add IF EXISTS functionality to DROP TABLE/VIEW statements
         processors.Remove( typeof( DropTableOrViewStatementImpl ) );
         processors.Add( typeof( PgSQLDropTableOrViewStatementImpl ), new PgSQLDropTableOrViewStatementProcessor() );

         // Add support for PgSQL-specific INSERT statement RETURNING clause
         processors.Remove( typeof( InsertStatementImpl ) );
         processors.Add( typeof( PgSQLInsertStatementImpl ), new PgSQLInsertStatementProcessor() );

         // Add support for column name list to appear as non-boolean expression (e.g. within where-clause)
         processors.Add( typeof( ColumnNameListExpressionImpl ), new ColumnNamesProcessor() );

         // Add support for VALUES clause to be value expression (e.g. within where-clause)
         processors.Add( typeof( ValuesExpressionImpl ), new ValuesExpressionProcessor() );

         DEFAULT_PROCESSORS = processors;
      }
   }
}
