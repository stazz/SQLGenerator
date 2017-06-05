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
using UtilPack;

namespace SQLGenerator
{
   public interface QueryExpression : NonBooleanExpression, SQLStatement
   {
      QueryExpressionBody Body { get; }
      // TODO With-statement
   }

   public interface QueryExpressionBody : NonBooleanExpression
   {

   }

   public interface QueryExpressionBodyActual : QueryExpressionBody
   {

   }

   public interface QueryExpressionBodyBinary : QueryExpressionBodyActual
   {
      SetOperations SetOperation { get; }
      [OptionalSQLElement]
      CorrespondingSpec CorrespondingSpec { get; }
      QueryExpressionBody Left { get; }
      QueryExpressionBody Right { get; }
      SetQuantifier SetQuantifier { get; }
   }
   public enum SetOperations { Union, Intersect, Except, MaxValue }
   public enum SetQuantifier { All, Distinct, MaxValue }
   public interface CorrespondingSpec : ObjectWithVendor
   {
      [OptionalSQLElement]
      ColumnNameList ColumnNames { get; }
   }

   public interface QueryExpressionBodyQuery : QueryExpressionBodyActual
   {

   }

   public interface QuerySpecification : QueryExpressionBodyQuery
   {
      SelectColumnClause Select { get; }

      [OptionalSQLElement]
      FromClause From { get; }

      [OptionalSQLElement]
      BooleanExpression Where { get; }

      [OptionalSQLElement]
      GroupByClause GroupBy { get; }

      [OptionalSQLElement]
      BooleanExpression Having { get; }

      [OptionalSQLElement]
      OrderByClause OrderBy { get; }

      [OptionalSQLElement]
      NonBooleanExpression Limit { get; }
      [OptionalSQLElement]
      NonBooleanExpression Offset { get; }
   }

   public interface TableValueConstructor : QueryExpressionBodyActual
   {
      ImmutableArray<RowValueConstructor> Rows { get; }
   }
   public interface RowValueConstructor : SQLElement
   {
   }
   public interface RowDefinition : RowValueConstructor
   {
      ImmutableArray<ValueExpression> RowElements { get; }
   }
   public interface RowSubQuery : RowValueConstructor
   {
      QueryExpression Query { get; }
   }

   public interface SelectColumnClause : SQLElement
   {
      SetQuantifier SetQuantifier { get; }
   }

   public interface AsteriskSelect : SelectColumnClause
   {
   }
   public interface ColumnReferences : SelectColumnClause
   {
      ImmutableArray<ColumnReferenceInfo> Columns { get; }
   }
   public sealed class ColumnReferenceInfo
   {
      private readonly String _alias;
      private readonly ColumnReference _reference;

      public ColumnReferenceInfo( ColumnReference aRef )
         : this( null, aRef )
      {
      }

      public ColumnReferenceInfo( String alias, ColumnReference reference )
      {
         ArgumentValidator.ValidateNotNull( nameof( reference ), reference );

         this._alias = alias;
         this._reference = reference;
      }

      public String Alias
      {
         get
         {
            return this._alias;
         }
      }

      public ColumnReference Reference
      {
         get
         {
            return this._reference;
         }
      }
   }
   public interface ColumnReference : NonBooleanExpression
   {
   }
   public interface ColumnReferenceByName : ColumnReference
   {
      [OptionalSQLElement]
      String TableName { get; }
      String ColumnName { get; }
   }
   public interface ColumnReferenceByExpression : ColumnReference
   {
      ValueExpression Expression { get; }
   }

   public interface FromClause : SQLElement
   {
      ImmutableArray<TableReference> TableReferences { get; }
   }
   public interface TableReference : SQLElement
   {
   }
   public interface TableReferencePrimary : TableReference
   {
      [OptionalSQLElement]
      TableAlias TableAlias { get; }
   }
   public interface TableAlias : SQLElement
   {
      String TableAlias { get; }
      [OptionalSQLElement]
      ColumnNameList ColumnAliases { get; }
   }
   public interface TableReferenceByName : TableReferencePrimary
   {
      TableName TableName { get; }
   }
   public interface TableReferenceByQuery : TableReferencePrimary
   {
      QueryExpression Query { get; }
   }

   public interface JoinedTable : QueryExpressionBody, TableReference
   {
      TableReference Left { get; }
      TableReference Right { get; }
   }
   public interface CrossJoinedTable : JoinedTable
   {
   }
   public interface NaturalJoinedTable : JoinedTable
   {
      JoinType JoinType { get; }
   }
   public enum JoinType { Inner, LeftOuter, RightOuter, FullOuter, MaxValue }
   public interface QualifiedJoinedTable : JoinedTable
   {
      JoinType JoinType { get; }
      JoinSpecification JoinSpecification { get; }
   }
   public interface UnionJoinedTable : JoinedTable
   {
   }
   public interface JoinSpecification : SQLElement
   {
   }
   public interface JoinCondition : JoinSpecification
   {
      BooleanExpression SearchCondition { get; }
   }
   public interface NamedColumnsJoin : JoinSpecification
   {
      ColumnNameList ColumnNames { get; }
   }

   public interface GroupByClause : SQLElement
   {
      ImmutableArray<GroupingElement> GroupingElements { get; }
   }

   public interface GroupingElement : SQLElement
   {
   }

   public interface OrdinaryGroupingSet : GroupingElement
   {
      // TODO maybe column reference instead of non boolean exp? then again, what about "table.col + 5"?
      ImmutableArray<NonBooleanExpression> Columns { get; }
   }

   public interface OrderByClause : SQLElement
   {
      ImmutableArray<SortSpecification> OrderingColumns { get; }
   }

   public interface SortSpecification : SQLElement
   {
      Ordering Ordering { get; }
      //TODO maybe column reference instead of non boolean exp? then again, what about "table.col + 5"?
      ValueExpression ColumnReference { get; }
   }
   public enum Ordering { Ascending, Descending, MaxValue }
}
