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
using SQLGenerator;

namespace SQLGenerator
{
   public interface DeleteStatement : SQLStatement
   {
   }

   public interface DeleteBySearch : DeleteStatement
   {
      TargetTable TargetTable { get; }
      [OptionalSQLElement]
      BooleanExpression Condition { get; }
   }

   public interface TargetTable : SQLElement
   {
      Boolean IsOnly { get; }
      TableNameDirect TableName { get; }
   }

   public interface UpdateStatement : SQLStatement
   {
   }

   public interface UpdateBySearch : UpdateStatement
   {
      TargetTable TargetTable { get; }
      [OptionalSQLElement]
      BooleanExpression Condition { get; }
      ImmutableArray<SetClause> SetClauses { get; }
   }

   public interface SetClause : SQLElement
   {
      String UpdateTarget { get; }
      UpdateSource UpdateSource { get; }
   }

   public interface UpdateSource : SQLElement
   {

   }

   public interface UpdateSourceByExpression : UpdateSource
   {
      ValueExpression Expression { get; }
   }

   public interface InsertStatement : SQLStatement
   {
      TableNameDirect TableName { get; }
      ColumnSource ColumnSource { get; }
   }

   public interface ColumnSource : SQLElement
   {

   }

   public interface DynamicColumnSource : ColumnSource
   {
      [OptionalSQLElement]
      ColumnNameList ColumnNames { get; }
   }

   public interface ColumnSourceByQuery : DynamicColumnSource
   {
      QueryExpression Query { get; }
   }

   public interface AbstractValuesList
   {
      ImmutableArray<ImmutableArray<ValueExpression>> Values { get; }
   }

   public interface ColumnSourceByValues : DynamicColumnSource, AbstractValuesList
   {

   }


}
