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
   public interface SchemaDefinitionStatement : SchemaStatement
   {

   }

   public interface SchemaDefinition : SchemaDefinitionStatement
   {
      String SchemaName { get; }

      [OptionalSQLElement]
      String SchemaCharset { get; }

      ImmutableArray<SchemaElement> SchemaElements { get; }
   }

   public interface SchemaElement : ObjectWithVendor
   {

   }

   public interface TableDefinition : SchemaDefinitionStatement, SchemaElement
   {
      TableScope? TableScope { get; }
      TableNameDirect TableName { get; }
      TableCommitAction? CommitAction { get; }
      TableContentsSource Contents { get; }
   }

   public enum TableScope { GlobalTemporary, LocalTemporary, MaxValue }
   public enum TableCommitAction { OnCommitPreserveRows, OnCommitDeleteRows, MaxValue }

   public interface TableContentsSource : SQLElement
   {
   }

   public interface TableElementList : TableContentsSource
   {
      ImmutableArray<TableElement> Elements { get; }
   }

   public interface TableElement : SQLElement
   {

   }

   public interface ColumnDefinition : TableElement
   {
      String ColumnName { get; }
      SQLDataType DataType { get; }
      // TODO is string good enough?
      [OptionalSQLElement]
      String Default { get; }
      Boolean MayBeNull { get; }
      AutoGenerationPolicy? AutoGenerationPolicy { get; }
   }
   public enum AutoGenerationPolicy { Always, ByDefault, MaxValue }

   public interface LikeClause : TableElement
   {
      TableNameDirect TableName { get; }
   }

   public interface TableConstraintDefinition : TableElement
   {
      [OptionalSQLElement]
      String ConstraintName { get; }
      ConstraintCharacteristics? ConstraintCharacteristics { get; }
      TableConstraint Constraint { get; }
   }
   public enum ConstraintCharacteristics { InitiallyImmediate_Deferrable, InitiallyDeferred_Deferrable, NotDeferrable, MaxValue }

   public interface TableConstraint : ObjectWithVendor
   {
   }

   public interface CheckConstraint : TableConstraint
   {
      BooleanExpression CheckCondition { get; }
   }

   public interface ForeignKeyConstraint : TableConstraint
   {
      ColumnNameList SourceColumns { get; }
      TableNameDirect TargetTable { get; }
      [OptionalSQLElement]
      ColumnNameList TargetColumns { get; }
      MatchType? MatchType { get; }
      ReferentialAction? OnUpdate { get; }
      ReferentialAction? OnDelete { get; }
   }
   public enum MatchType { Full, Partial, Simple, MaxValue }
   public enum ReferentialAction { Cascade, SetNull, SetDefault, Restrict, NoAction, MaxValue }

   public interface UniqueConstraint : TableConstraint
   {
      UniqueSpecification UniquenessKind { get; }
      ColumnNameList ColumnNames { get; }
   }
   public enum UniqueSpecification { PrimaryKey, Unique, MaxValue }

   public interface ViewDefinition : SchemaDefinitionStatement, SchemaElement
   {
      Boolean IsRecursive { get; }
      TableNameDirect ViewName { get; }
      ViewCheckOption? ViewCheckOption { get; }
      ViewSpecification ViewSpecification { get; }
      QueryExpression ViewQuery { get; }
   }
   public enum ViewCheckOption { Cascaded, Local, MaxValue }

   public interface ViewSpecification : SQLElement
   {
   }

   public interface RegularViewSpecification : ViewSpecification
   {
      [OptionalSQLElement]
      ColumnNameList Columns { get; }
   }
}
