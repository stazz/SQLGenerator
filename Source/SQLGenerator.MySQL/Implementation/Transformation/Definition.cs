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
using SQLGenerator.Implementation.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGenerator.MySQL.Implementation.Transformation
{
   internal class MySQLSchemaDefinitionProcessor : SchemaDefinitionProcessor
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SchemaDefinition obj, StringBuilder builder )
      {
         // Ignore actual schema, process other elements
         this.ProcessSchemaElements( aggregator, obj, builder );
      }
   }

   internal class MySQLColumnDefinitionProcessor : ColumnDefinitionProcessor
   {
      protected override void ProcessAutoGenerationPolicy( ColumnDefinition obj, StringBuilder builder )
      {
         // MySQL combines both ALWAYS and BY DEFAULT policies
         builder.Append( " AUTO_INCREMENT" );
      }
   }
}
