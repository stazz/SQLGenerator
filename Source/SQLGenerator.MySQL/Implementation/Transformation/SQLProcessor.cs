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

namespace SQLGenerator.MySQL.Implementation.Transformation
{
   internal class MySQLProcessors
   {
      internal static readonly IDictionary<Type, SQLProcessor> DEFAULT_PROCESSORS;

      static MySQLProcessors()
      {
         var processors = SQLProcessorAggregator.GetDefaultProcessorsCopy();

         // MySQL doesn't understand schemas
         processors[typeof( TableNameDirectImpl )] = new MySQLTableNameDirectProcessor();
         processors[typeof( TableNameFunctionImpl )] = new MySQLTableNameFunctionProcessor();
         processors[typeof( SchemaDefinitionImpl )] = new MySQLSchemaDefinitionProcessor();
         processors[typeof( DropSchemaStatementImpl )] = new NoOpProcessor();

         // MySQL has custom way of handling auto generation syntax of columns
         processors[typeof( ColumnDefinitionImpl )] = new MySQLColumnDefinitionProcessor();

         // MySQL has different syntax for OFFSET/FETCH
         processors[typeof( QuerySpecificationImpl )] = new MySQLQuerySpecificationProcessor();

         DEFAULT_PROCESSORS = processors;
      }
   }
}
