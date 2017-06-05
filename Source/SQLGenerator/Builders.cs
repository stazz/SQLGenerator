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
   public interface AbstractBuilder<out TExpression> : ObjectWithVendor
      where TExpression : class
   {
      TExpression CreateExpression();
      Boolean CanCreateMeaningfulExpression();
   }

   public interface BooleanBuilder : AbstractBuilder<BooleanExpression>
   {
      BooleanBuilder And( BooleanExpression next );
      BooleanBuilder Or( BooleanExpression next );
      BooleanBuilder Not();
      BooleanBuilder Reset( BooleanExpression next = null );
   }

   public interface ForeignKeyConstraintBuilder : AbstractBuilder<ForeignKeyConstraint>
   {
      ForeignKeyConstraintBuilder AddSourceColumns( IEnumerable<String> columnNames );
      ForeignKeyConstraintBuilder AddTargetColumns( IEnumerable<String> columnNames );
      ForeignKeyConstraintBuilder SetTargetTableName( TableNameDirect tableName );
      ForeignKeyConstraintBuilder SetMatchType( MatchType? matchType );
      ForeignKeyConstraintBuilder SetOnUpdate( ReferentialAction? action );
      ForeignKeyConstraintBuilder SetOnDelete( ReferentialAction? action );
   }

   public interface SchemaDefinitionBuilder : AbstractBuilder<SchemaDefinition>
   {
      SchemaDefinitionBuilder SetSchemaName( String schemaName );
      SchemaDefinitionBuilder SetSchemaCharset( String charset );
      SchemaDefinitionBuilder AddSchemaElements( IEnumerable<SchemaElement> elements );
   }

   public interface TableDefinitionBuilder : AbstractBuilder<TableDefinition>
   {
      TableDefinitionBuilder SetTableScope( TableScope? scope );
      TableDefinitionBuilder SetTableName( TableNameDirect tableName );
      TableDefinitionBuilder SetCommitAction( TableCommitAction? action );
      TableDefinitionBuilder SetTableContentsSource( TableContentsSource contents );
   }

   public interface TableElementListBuilder : AbstractBuilder<TableElementList>
   {
      TableElementListBuilder AddTableElements( IEnumerable<TableElement> element );
   }

   public interface UniqueConstraintBuilder : AbstractBuilder<UniqueConstraint>
   {
      UniqueConstraintBuilder SetUniqueness( UniqueSpecification uniqueness );
      UniqueConstraintBuilder AddColumns( IEnumerable<String> columns );
   }

   public interface ViewDefinitionBuilder : AbstractBuilder<ViewDefinition>
   {
      ViewDefinitionBuilder SetRecursive( Boolean isRecursive );
      ViewDefinitionBuilder SetViewName( TableNameDirect viewName );
      ViewDefinitionBuilder SetQuery( QueryExpression query );
      ViewDefinitionBuilder SetViewCheckOption( ViewCheckOption? viewCheck );
      ViewDefinitionBuilder SetViewSpecification( ViewSpecification specification );
   }

   public interface DeleteBySearchBuilder : AbstractBuilder<DeleteBySearch>
   {
      DeleteBySearchBuilder SetTargetTable( TargetTable table );
      BooleanBuilder ConditionBuilder { get; }
   }

   public interface InsertStatementBuilder : AbstractBuilder<InsertStatement>
   {
      InsertStatementBuilder SetTableName( TableNameDirect tableName );
      InsertStatementBuilder SetColumnSource( ColumnSource source );
   }

   public interface ColumnSourceByValuesBuilder : AbstractBuilder<ColumnSourceByValues>
   {
      ColumnSourceByValuesBuilder AddValues( IEnumerable<ValueExpression> values );
      ColumnSourceByValuesBuilder AddColumnNames( IEnumerable<String> columnNames );
   }

   public interface UpdateBySearchBuilder : AbstractBuilder<UpdateBySearch>
   {
      UpdateBySearchBuilder SetTargetTable( TargetTable targetTable );
      BooleanBuilder ConditionBuilder { get; }
      UpdateBySearchBuilder AddSetClauses( IEnumerable<SetClause> clauses );
   }

   public interface QuerySpecificationBuilder : AbstractBuilder<QuerySpecification>
   {
      SelectColumnClauseBuilder ColumnsBuilder { get; }
      FromBuilder FromBuilder { get; }
      BooleanBuilder WhereBuilder { get; }
      GroupByBuilder GroupByBuilder { get; }
      BooleanBuilder HavingBuilder { get; }
      OrderByBuilder OrderByBuilder { get; }

      QuerySpecificationBuilder Limit( NonBooleanExpression max );
      QuerySpecificationBuilder Offset( NonBooleanExpression skip );

      QuerySpecificationBuilder TrimGroupBy();
   }

   public interface SelectColumnClauseBuilder : AbstractBuilder<SelectColumnClause>
   {
      SelectColumnClauseBuilder SelectAll();
      SelectColumnClauseBuilder AddColumns( IEnumerable<ColumnReferenceInfo> namedColumns );
      // Index zero-based
      SelectColumnClauseBuilder SetNameFor( Int32 idx, String alias );
      SelectColumnClauseBuilder SetSetQuantifier( SetQuantifier quantifier );
      Int32 ColumnCount { get; }
   }

   public interface FromBuilder : AbstractBuilder<FromClause>
   {
      FromBuilder AddTableNames( IEnumerable<Tuple<String, TableName>> tableNames );
      FromBuilder AddTableRefs( IEnumerable<TableReference> tableRefs );
      FromBuilder ClearTableRefs();
   }

   public interface JoinedTableBuilder : AbstractBuilder<TableReference>
   {
      JoinedTableBuilder AddQualifiedJoin( JoinType joinType, TableReference right, JoinSpecification joinSpec );
      JoinedTableBuilder AddCrossJoin( TableReference right );
      JoinedTableBuilder AddNaturalJoin( JoinType joinType, TableReference right );
      JoinedTableBuilder AddUnionJoin( TableReference right );
   }

   public interface GroupByBuilder : AbstractBuilder<GroupByClause>
   {
      GroupByBuilder AddGroupingElements( IEnumerable<GroupingElement> elements );
   }

   public interface OrderByBuilder : AbstractBuilder<OrderByClause>
   {
      OrderByBuilder AddSortSpecifications( IEnumerable<SortSpecification> specifications );
   }

   public interface QueryExpressionBodyBuilder : AbstractBuilder<QueryExpressionBody>
   {
      QueryExpressionBodyBuilder Union( QueryExpressionBody another, SetQuantifier setQuantifier = SetQuantifier.Distinct, CorrespondingSpec correspondingSpec = null );
      QueryExpressionBodyBuilder Intersect( QueryExpressionBody another, SetQuantifier setQuantifier = SetQuantifier.Distinct, CorrespondingSpec correspondingSpec = null );
      QueryExpressionBodyBuilder Except( QueryExpressionBody another, SetQuantifier setQuantifier = SetQuantifier.Distinct, CorrespondingSpec correspondingSpec = null );
   }
}

public static partial class E_SQLGenerator
{
   public static QuerySpecificationBuilder Limit( this QuerySpecificationBuilder builder, Int32 max )
   {
      return builder.Limit( builder.SQLVendor.CommonFactory.I32( max ) );
   }

   public static QuerySpecificationBuilder Offset( this QuerySpecificationBuilder builder, Int32 skip )
   {
      return builder.Offset( builder.SQLVendor.CommonFactory.I32( skip ) );
   }

   public static QuerySpecificationBuilder Limit( this QuerySpecificationBuilder builder, Int64 max )
   {
      return builder.Limit( builder.SQLVendor.CommonFactory.I64( max ) );
   }

   public static QuerySpecificationBuilder Offset( this QuerySpecificationBuilder builder, Int64 skip )
   {
      return builder.Offset( builder.SQLVendor.CommonFactory.I64( skip ) );
   }

   public static QueryExpression CreateSelectStatement( this QuerySpecification specification )
   {
      return specification.SQLVendor.QueryFactory.NewQuery( specification );
   }

   public static QueryExpression CreateSelectStatement( this QuerySpecificationBuilder builder )
   {
      return CreateSelectStatement( builder.CreateExpression() );
   }

   public static QuerySpecificationBuilder Select( this QuerySpecificationBuilder builder, params String[] colNames )
   {
      builder.ColumnsBuilder.AddUnnamedColumns( colNames.Select( cn => builder.SQLVendor.QueryFactory.ColumnName( cn ) ).ToArray() );
      return builder;
   }

   public static QuerySpecificationBuilder WithSetQuantifier( this QuerySpecificationBuilder builder, SetQuantifier quantifier )
   {
      builder.ColumnsBuilder.SetSetQuantifier( quantifier );
      return builder;
   }

   public static QuerySpecificationBuilder Select( this QuerySpecificationBuilder builder, params ValueExpression[] expressions )
   {
      builder.ColumnsBuilder.AddUnnamedColumns( expressions.Select( e => builder.SQLVendor.QueryFactory.ColumnExpression( e ) ).ToArray() );
      return builder;
   }

   public static QuerySpecificationBuilder As( this QuerySpecificationBuilder builder, String alias )
   {
      builder.ColumnsBuilder.SetNameFor( builder.ColumnsBuilder.ColumnCount - 1, alias );
      return builder;
   }

   public static QuerySpecificationBuilder SelectAll( this QuerySpecificationBuilder builder )
   {
      builder.ColumnsBuilder.SelectAll();
      return builder;
   }

   public static QuerySpecificationBuilder From( this QuerySpecificationBuilder builder, params TableName[] tables )
   {
      builder.FromBuilder.AddTableNames( tables );
      return builder;
   }

   public static QuerySpecificationBuilder Where( this QuerySpecificationBuilder builder, BooleanExpression expr )
   {
      builder.WhereBuilder.Reset( expr );
      return builder;
   }

   public static QuerySpecificationBuilder GroupBy( this QuerySpecificationBuilder builder, params String[] columns )
   {
      builder.GroupByBuilder.AddGroupingElements( columns.Select( c => builder.SQLVendor.QueryFactory.NewGroupingElement( builder.SQLVendor.QueryFactory.ColumnName( c ) ) ).ToArray() );
      return builder;
   }

   public static QuerySpecificationBuilder Having( this QuerySpecificationBuilder builder, BooleanExpression expr )
   {
      builder.HavingBuilder.Reset( expr );
      return builder;
   }

   public static QuerySpecificationBuilder OrderBy( this QuerySpecificationBuilder builder, Ordering ordering, params String[] columns )
   {
      builder.OrderByBuilder.AddSortSpecifications( columns.Select( c => builder.SQLVendor.QueryFactory.NewSortSpecification( builder.SQLVendor.QueryFactory.ColumnName( c ), ordering ) ).ToArray() );
      return builder;
   }

   public static QuerySpecificationBuilder OrderByAsc( this QuerySpecificationBuilder builder, params String[] columns )
   {
      return OrderBy( builder, Ordering.Ascending, columns );
   }

   public static QuerySpecificationBuilder OrderByDesc( this QuerySpecificationBuilder builder, params String[] columns )
   {
      return OrderBy( builder, Ordering.Descending, columns );
   }

   public static FromBuilder AddTableNames( this FromBuilder builder, params TableName[] tables )
   {
      return builder.AddTableNames( tables.Select( t => Tuple.Create<String, TableName>( null, t ) ).ToArray() );
   }

   public static ForeignKeyConstraintBuilder AddSourceColumnsP( this ForeignKeyConstraintBuilder builder, params String[] columnNames )
   {
      return builder.AddSourceColumns( columnNames );
   }

   public static ForeignKeyConstraintBuilder AddTargetColumnsP( this ForeignKeyConstraintBuilder builder, params String[] columnNames )
   {
      return builder.AddTargetColumns( columnNames );
   }

   public static SchemaDefinitionBuilder AddSchemaElementsP( this SchemaDefinitionBuilder builder, params SchemaElement[] elements )
   {
      return builder.AddSchemaElements( elements );
   }

   public static TableElementListBuilder AddTableElementsP( this TableElementListBuilder builder, params TableElement[] elements )
   {
      return builder.AddTableElements( elements );
   }

   public static UniqueConstraintBuilder AddColumnsP( this UniqueConstraintBuilder builder, params String[] columns )
   {
      return builder.AddColumns( columns );
   }

   public static ColumnSourceByValuesBuilder AddValuesP( this ColumnSourceByValuesBuilder builder, params ValueExpression[] values )
   {
      return builder.AddValues( values );
   }

   public static ColumnSourceByValuesBuilder AddColumnNamesP( this ColumnSourceByValuesBuilder builder, params String[] columnNames )
   {
      return builder.AddColumnNames( columnNames );
   }

   public static UpdateBySearchBuilder AddSetClausesP( this UpdateBySearchBuilder builder, params SetClause[] clauses )
   {
      return builder.AddSetClauses( clauses );
   }

   public static SelectColumnClauseBuilder AddUnnamedColumnsP( this SelectColumnClauseBuilder builder, params ColumnReference[] columns )
   {
      return AddUnnamedColumns( builder, columns );
   }

   public static SelectColumnClauseBuilder AddUnnamedColumns( this SelectColumnClauseBuilder builder, IEnumerable<ColumnReference> columns )
   {
      return builder.AddColumns( columns.Select( col => new ColumnReferenceInfo( col ) ) );
   }

   public static SelectColumnClauseBuilder AddNamedColumnsP( this SelectColumnClauseBuilder builder, params ColumnReferenceInfo[] namedColumns )
   {
      return builder.AddColumns( namedColumns );
   }

   public static FromBuilder AddTableNamesP( this FromBuilder builder, params Tuple<String, TableName>[] tableNames )
   {
      return builder.AddTableNames( tableNames );
   }

   public static FromBuilder AddTableRefsP( this FromBuilder builder, params TableReference[] tableRefs )
   {
      return builder.AddTableRefs( tableRefs );
   }

   public static FromBuilder AddTableNamesP( this FromBuilder builder, params TableName[] tableNames )
   {
      return builder.AddTableNames( tableNames.Select( n => Tuple.Create<String, TableName>( null, n ) ) );
   }

   public static GroupByBuilder AddGroupingElementsP( this GroupByBuilder builder, params GroupingElement[] elements )
   {
      return builder.AddGroupingElements( elements );
   }

   public static OrderByBuilder AddSortSpecificationsP( this OrderByBuilder builder, params SortSpecification[] specifications )
   {
      return builder.AddSortSpecifications( specifications );
   }

}
