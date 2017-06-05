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

using UtilPack;

namespace SQLGenerator.Implementation.Data
{
   public abstract class AbstractBuilderImpl<T> : AbstractBuilder<T>
      where T : class
   {

      protected readonly SQLVendor vendor;

      protected AbstractBuilderImpl( SQLVendor vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( vendor ), vendor );

         this.vendor = vendor;
      }

      #region AbstractBuilder<T> Members

      public abstract T CreateExpression();

      public abstract Boolean CanCreateMeaningfulExpression();

      #endregion

      #region ObjectWithVendor Members

      public SQLVendor SQLVendor
      {
         get
         {
            return this.vendor;
         }
      }

      #endregion

   }

   public class BooleanBuilderImpl : AbstractBuilderImpl<BooleanExpression>, BooleanBuilder
   {
      private BooleanExpression _topLevelExpression;

      public BooleanBuilderImpl( SQLVendor vendor, BooleanExpression expr )
         : base( vendor )
      {
         this._topLevelExpression = expr ?? vendor.CommonFactory.Empty;
      }

      public override BooleanExpression CreateExpression()
      {
         return this._topLevelExpression;
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._topLevelExpression != this.vendor.CommonFactory.Empty;
      }

      #region BooleanBuilder Members

      public BooleanBuilder And( BooleanExpression next )
      {
         this._topLevelExpression = this.vendor.CommonFactory.And( this._topLevelExpression, next );
         return this;
      }

      public BooleanBuilder Or( BooleanExpression next )
      {
         this._topLevelExpression = this.vendor.CommonFactory.Or( this._topLevelExpression, next );
         return this;
      }

      public BooleanBuilder Not()
      {
         this._topLevelExpression = this.vendor.CommonFactory.Not( this._topLevelExpression );
         return this;
      }

      public BooleanBuilder Reset( BooleanExpression next = null )
      {
         this._topLevelExpression = next == null ? this.vendor.CommonFactory.Empty : next;
         return this;
      }

      #endregion
   }

   public class ForeignKeyConstraintBuilderImpl : AbstractBuilderImpl<ForeignKeyConstraint>, ForeignKeyConstraintBuilder
   {
      private readonly List<String> _sourceColumns;
      private readonly List<String> _targetColumns;
      private TableNameDirect _targetTable;
      private MatchType? _matchType;
      private ReferentialAction? _onUpdate;
      private ReferentialAction? _onDelete;

      public ForeignKeyConstraintBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._sourceColumns = new List<String>();
         this._targetColumns = new List<String>();
      }

      public override ForeignKeyConstraint CreateExpression()
      {
         return this.vendor.DefinitionFactory.NewForeignKeyConstraint( this._sourceColumns.NewAQ(), this._targetTable, this._targetColumns.NewAQ(), this._matchType, this._onUpdate, this._onDelete );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._targetTable != null && this._sourceColumns.Count > 0;
      }

      #region ForeignKeyConstraintBuilder Members

      public ForeignKeyConstraintBuilder AddSourceColumns( IEnumerable<String> columnNames )
      {
         this._sourceColumns.AddRange( columnNames );
         return this;
      }

      public ForeignKeyConstraintBuilder AddTargetColumns( IEnumerable<String> columnNames )
      {
         this._targetColumns.AddRange( columnNames );
         return this;
      }

      public ForeignKeyConstraintBuilder SetTargetTableName( TableNameDirect tableName )
      {
         this._targetTable = tableName;
         return this;
      }

      public ForeignKeyConstraintBuilder SetMatchType( MatchType? matchType )
      {
         this._matchType = matchType;
         return this;
      }

      public ForeignKeyConstraintBuilder SetOnUpdate( ReferentialAction? action )
      {
         this._onUpdate = action;
         return this;
      }

      public ForeignKeyConstraintBuilder SetOnDelete( ReferentialAction? action )
      {
         this._onDelete = action;
         return this;
      }

      #endregion
   }

   public class SchemaDefinitionBuilderImpl : AbstractBuilderImpl<SchemaDefinition>, SchemaDefinitionBuilder
   {
      private String _schemaName;
      private String _charset;
      private readonly List<SchemaElement> _elements;

      public SchemaDefinitionBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._elements = new List<SchemaElement>();
      }

      public override SchemaDefinition CreateExpression()
      {
         return this.vendor.DefinitionFactory.NewSchemaDefinition( this._schemaName, this._charset, this._elements.NewAQ() );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._schemaName != null;
      }

      #region SchemaDefinitionBuilder Members

      public SchemaDefinitionBuilder SetSchemaName( String schemaName )
      {
         this._schemaName = schemaName;
         return this;
      }

      public SchemaDefinitionBuilder SetSchemaCharset( String charset )
      {
         this._charset = charset;
         return this;
      }

      public SchemaDefinitionBuilder AddSchemaElements( IEnumerable<SchemaElement> elements )
      {
         this._elements.AddRange( elements );
         return this;
      }

      #endregion
   }

   public class TableDefinitionBuilderImpl : AbstractBuilderImpl<TableDefinition>, TableDefinitionBuilder
   {
      private TableScope? _scope;
      private TableNameDirect _tableName;
      private TableCommitAction? _tableCommitAction;
      private TableContentsSource _tableContents;

      public TableDefinitionBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {

      }

      public override TableDefinition CreateExpression()
      {
         return this.vendor.DefinitionFactory.NewTableDefinition( this._tableName, this._tableContents, this._scope, this._tableCommitAction );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._tableName != null;
      }

      #region TableDefinitionBuilder Members

      public TableDefinitionBuilder SetTableScope( TableScope? scope )
      {
         this._scope = scope;
         return this;
      }

      public TableDefinitionBuilder SetTableName( TableNameDirect tableName )
      {
         this._tableName = tableName;
         return this;
      }

      public TableDefinitionBuilder SetCommitAction( TableCommitAction? action )
      {
         this._tableCommitAction = action;
         return this;
      }

      public TableDefinitionBuilder SetTableContentsSource( TableContentsSource contents )
      {
         this._tableContents = contents;
         return this;
      }

      #endregion
   }

   public class TableElementListBuilderImpl : AbstractBuilderImpl<TableElementList>, TableElementListBuilder
   {
      private readonly List<TableElement> _elements;

      public TableElementListBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._elements = new List<TableElement>();
      }

      public override TableElementList CreateExpression()
      {
         return this.vendor.DefinitionFactory.NewTableElementList( this._elements.NewAQ() );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._elements.Count > 0;
      }

      #region TableElementListBuilder Members

      public TableElementListBuilder AddTableElements( IEnumerable<TableElement> elements )
      {
         this._elements.AddRange( elements );
         return this;
      }

      #endregion
   }

   public class UniqueConstraintBuilderImpl : AbstractBuilderImpl<UniqueConstraint>, UniqueConstraintBuilder
   {
      private UniqueSpecification _uniqueness;
      private readonly List<String> _cols;

      public UniqueConstraintBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._uniqueness = UniqueSpecification.Unique;
         this._cols = new List<String>();
      }

      public override UniqueConstraint CreateExpression()
      {
         return this.vendor.DefinitionFactory.NewUniqueConstraint( this._uniqueness, this._cols.NewAQ() );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._cols.Count > 0;
      }

      #region UniqueConstraintBuilder Members

      public UniqueConstraintBuilder SetUniqueness( UniqueSpecification uniqueness )
      {
         this._uniqueness = uniqueness;
         return this;
      }

      public UniqueConstraintBuilder AddColumns( IEnumerable<String> columns )
      {
         this._cols.AddRange( columns );
         return this;
      }

      #endregion
   }

   public class ViewDefinitionBuilderImpl : AbstractBuilderImpl<ViewDefinition>, ViewDefinitionBuilder
   {

      private TableNameDirect _viewName;
      private QueryExpression _query;
      private ViewSpecification _viewSpec;
      private ViewCheckOption? _viewCheck;
      private Boolean _isRecursive;

      public ViewDefinitionBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {

      }

      public override ViewDefinition CreateExpression()
      {
         return this.vendor.DefinitionFactory.NewViewDefinition( this._viewName, this._query, this._viewSpec, this._viewCheck, this._isRecursive );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._viewName != null && this._viewSpec != null && this._query != null;
      }

      #region ViewDefinitionBuilder Members

      public ViewDefinitionBuilder SetRecursive( Boolean isRecursive )
      {
         this._isRecursive = isRecursive;
         return this;
      }

      public ViewDefinitionBuilder SetViewName( TableNameDirect viewName )
      {
         this._viewName = viewName;
         return this;
      }

      public ViewDefinitionBuilder SetQuery( QueryExpression query )
      {
         this._query = query;
         return this;
      }

      public ViewDefinitionBuilder SetViewCheckOption( ViewCheckOption? viewCheck )
      {
         this._viewCheck = viewCheck;
         return this;
      }

      public ViewDefinitionBuilder SetViewSpecification( ViewSpecification specification )
      {
         this._viewSpec = specification;
         return this;
      }

      #endregion
   }

   public class ColumnSourceByValuesBuilderImpl : AbstractBuilderImpl<ColumnSourceByValues>, ColumnSourceByValuesBuilder
   {
      private readonly List<String> _colNames;
      private readonly List<List<ValueExpression>> _values;

      public ColumnSourceByValuesBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._colNames = new List<String>();
         this._values = new List<List<ValueExpression>>();
      }

      public override ColumnSourceByValues CreateExpression()
      {
         return this.vendor.ModificationFactory.NewColumnSourceByValues( this._values.Select( v => v.NewAQ() ).NewAQ(), this.vendor.CommonFactory.ColumnNames( this._colNames.NewAQ() ) );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._values.Count > 0 && this._values.All( v => v.Count > 0 );
      }

      #region ColumnSourceByValuesBuilder Members

      public ColumnSourceByValuesBuilder AddValues( IEnumerable<ValueExpression> values )
      {
         this._values.Add( values.ToList() );
         return this;
      }

      public ColumnSourceByValuesBuilder AddValuesToCurrent( IEnumerable<ValueExpression> values )
      {
         List<ValueExpression> current;
         if ( this._values.Count > 0 )
         {
            current = this._values[this._values.Count - 1];
         }
         else
         {
            current = new List<ValueExpression>();
            this._values.Add( current );
         }
         current.AddRange( values );
         return this;
      }

      public ColumnSourceByValuesBuilder AddColumnNames( IEnumerable<String> columnNames )
      {
         this._colNames.AddRange( columnNames );
         return this;
      }

      #endregion
   }

   public class DeleteBySearchBuilderImpl : AbstractBuilderImpl<DeleteBySearch>, DeleteBySearchBuilder
   {
      private readonly BooleanBuilder _condition;
      private TargetTable _target;

      public DeleteBySearchBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._condition = vendor.CommonFactory.NewBooleanBuilder();
      }

      public override DeleteBySearch CreateExpression()
      {
         return this.vendor.ModificationFactory.NewDeleteBySearch( this._target, this._condition.CanCreateMeaningfulExpression() ? this._condition.CreateExpression() : null );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._target != null;
      }

      #region DeleteBySearchBuilder Members

      public DeleteBySearchBuilder SetTargetTable( TargetTable table )
      {
         this._target = table;
         return this;
      }

      public BooleanBuilder ConditionBuilder
      {
         get
         {
            return this._condition;
         }
      }

      #endregion
   }

   public class InsertStatementBuilderImpl : AbstractBuilderImpl<InsertStatement>, InsertStatementBuilder
   {
      private TableNameDirect _table;
      private ColumnSource _source;

      public InsertStatementBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {

      }

      public override InsertStatement CreateExpression()
      {
         return this.vendor.ModificationFactory.NewInsertStatement( this._table, this._source );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._table != null && this._source != null;
      }

      #region InsertStatementBuilder Members

      public InsertStatementBuilder SetTableName( TableNameDirect tableName )
      {
         this._table = tableName;
         return this;
      }

      public InsertStatementBuilder SetColumnSource( ColumnSource source )
      {
         this._source = source;
         return this;
      }

      #endregion
   }

   public class UpdateBySearchBuilderImpl : AbstractBuilderImpl<UpdateBySearch>, UpdateBySearchBuilder
   {
      private readonly BooleanBuilder _condition;
      private TargetTable _target;
      private readonly List<SetClause> _setClauses;

      public UpdateBySearchBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._condition = vendor.CommonFactory.NewBooleanBuilder();
         this._setClauses = new List<SetClause>();
      }

      public override UpdateBySearch CreateExpression()
      {
         return this.vendor.ModificationFactory.NewUpdateBySearch( this._target, this._setClauses.NewAQ(), this._condition.CanCreateMeaningfulExpression() ? this._condition.CreateExpression() : null );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._setClauses.Count > 0 && this._target != null;
      }

      #region UpdateBySearchBuilder Members

      public UpdateBySearchBuilder SetTargetTable( TargetTable targetTable )
      {
         this._target = targetTable;
         return this;
      }

      public BooleanBuilder ConditionBuilder
      {
         get
         {
            return this._condition;
         }
      }

      public UpdateBySearchBuilder AddSetClauses( IEnumerable<SetClause> clauses )
      {
         this._setClauses.AddRange( clauses );
         return this;
      }

      #endregion
   }

   public class SelectColumnClauseBuilderImpl : AbstractBuilderImpl<SelectColumnClause>, SelectColumnClauseBuilder
   {
      private readonly List<ColumnReferenceInfo> _cols;
      private SetQuantifier _quantifier;
      private Boolean _selectAll;

      public SelectColumnClauseBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._selectAll = false;
         this._cols = new List<ColumnReferenceInfo>();
         this._quantifier = SetQuantifier.All;
      }

      public override SelectColumnClause CreateExpression()
      {
         return this._selectAll ? this.vendor.QueryFactory.NewSelectAllClause( this._quantifier ) : (SelectColumnClause) this.vendor.QueryFactory.NewSelectClause( this._quantifier, this._cols.NewAQ() );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._selectAll || this._cols.Count > 0;
      }

      #region ColumnsBuilder Members

      public SelectColumnClauseBuilder SelectAll()
      {
         this._selectAll = true;
         this._cols.Clear();
         return this;
      }

      public SelectColumnClauseBuilder AddColumns( IEnumerable<ColumnReferenceInfo> namedColumns )
      {
         this._selectAll = false;
         this._cols.AddRange( namedColumns );
         return this;
      }

      public SelectColumnClauseBuilder SetNameFor( Int32 idx, string alias )
      {
         this._selectAll = false;
         this._cols[idx] = new ColumnReferenceInfo( alias, this._cols[idx].Reference );
         return this;
      }

      public SelectColumnClauseBuilder SetSetQuantifier( SetQuantifier quantifier )
      {
         this._selectAll = false;
         this._quantifier = quantifier;
         return this;
      }

      public Int32 ColumnCount
      {
         get
         {
            return this._cols.Count;
         }
      }

      #endregion

      internal List<ColumnReferenceInfo> Cols
      {
         get
         {
            return this._cols;
         }
      }
   }

   public class FromBuilderImpl : AbstractBuilderImpl<FromClause>, FromBuilder
   {
      private readonly List<TableReference> _refs;

      public FromBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._refs = new List<TableReference>();
      }

      public override FromClause CreateExpression()
      {
         return this.vendor.QueryFactory.NewFromClause( this._refs.NewAQ() );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._refs.Count > 0;
      }

      #region FromBuilder Members

      public FromBuilder AddTableNames( IEnumerable<Tuple<String, TableName>> tableNames )
      {
         this._refs.AddRange( tableNames.Select( tn => this.vendor.QueryFactory.NewTableReferenceByName( tn.Item2, tn.Item1 == null ? null : this.vendor.QueryFactory.NewTableAlias( tn.Item1 ) ) ) );
         return this;
      }

      public FromBuilder AddTableRefs( IEnumerable<TableReference> tableRefs )
      {
         this._refs.AddRange( tableRefs );
         return this;
      }

      public FromBuilder ClearTableRefs()
      {
         this._refs.Clear();
         return this;
      }

      #endregion
   }

   public class JoinedTableBuilderImpl : AbstractBuilderImpl<TableReference>, JoinedTableBuilder
   {

      private TableReference _currentTable;

      public JoinedTableBuilderImpl( SQLVendor vendor, TableReferencePrimary startingTable )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( startingTable ), startingTable );

         this._currentTable = startingTable;
      }

      public override TableReference CreateExpression()
      {
         return this._currentTable;
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return true;
      }

      #region JoinedTableBuilder Members

      public JoinedTableBuilder AddQualifiedJoin( JoinType joinType, TableReference right, JoinSpecification joinSpec )
      {
         this._currentTable = this.vendor.QueryFactory.NewQualifiedJoinedTable( this._currentTable, right, joinSpec, joinType );
         return this;
      }

      public JoinedTableBuilder AddCrossJoin( TableReference right )
      {
         this._currentTable = this.vendor.QueryFactory.NewCrossJoinedTable( this._currentTable, right );
         return this;
      }

      public JoinedTableBuilder AddNaturalJoin( JoinType joinType, TableReference right )
      {
         this._currentTable = this.vendor.QueryFactory.NewNaturalJoinedTable( this._currentTable, right, joinType );
         return this;
      }

      public JoinedTableBuilder AddUnionJoin( TableReference right )
      {
         this._currentTable = this.vendor.QueryFactory.NewUnionJoinedTable( this._currentTable, right );
         return this;
      }

      #endregion
   }

   public class GroupByBuilderImpl : AbstractBuilderImpl<GroupByClause>, GroupByBuilder
   {
      private readonly List<GroupingElement> _groupingElements;

      public GroupByBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._groupingElements = new List<GroupingElement>();
      }

      public override GroupByClause CreateExpression()
      {
         return this.vendor.QueryFactory.NewGroupByClause( this._groupingElements.NewAQ() );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._groupingElements.Count > 0;
      }

      #region GroupByBuilder Members

      public GroupByBuilder AddGroupingElements( IEnumerable<GroupingElement> elements )
      {
         this._groupingElements.AddRange( elements );
         return this;
      }

      #endregion

      internal List<GroupingElement> GroupingElements
      {
         get
         {
            return this._groupingElements;
         }
      }
   }

   public class OrderByBuilderImpl : AbstractBuilderImpl<OrderByClause>, OrderByBuilder
   {
      private readonly List<SortSpecification> _specs;

      public OrderByBuilderImpl( SQLVendor vendor )
         : base( vendor )
      {
         this._specs = new List<SortSpecification>();
      }

      public override OrderByClause CreateExpression()
      {
         return this.vendor.QueryFactory.NewOrderByClause( this._specs.NewAQ() );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._specs.Count > 0;
      }

      #region OrderByBuilder Members

      public OrderByBuilder AddSortSpecifications( IEnumerable<SortSpecification> specifications )
      {
         this._specs.AddRange( specifications );
         return this;
      }

      #endregion
   }

   public class QuerySpecificationBuilderImpl : AbstractBuilderImpl<QuerySpecification>, QuerySpecificationBuilder
   {
      private readonly SelectColumnClauseBuilder _colsBuilder;
      private readonly FromBuilder _fromBuilder;
      private readonly BooleanBuilder _whereBuilder;
      private readonly GroupByBuilder _groupByBuilder;
      private readonly BooleanBuilder _havingBuilder;
      private readonly OrderByBuilder _orderByBuilder;
      private NonBooleanExpression _limit;
      private NonBooleanExpression _offset;

      public QuerySpecificationBuilderImpl(
         SQLVendor vendor,
         SelectColumnClauseBuilder columnsBuilder,
         FromBuilder fromBuilder,
         BooleanBuilder whereBuilder,
         GroupByBuilder groupByBuilder,
         BooleanBuilder havingBuilder,
         OrderByBuilder orderByBuilder
         )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( columnsBuilder ), columnsBuilder );
         ArgumentValidator.ValidateNotNull( nameof( fromBuilder ), fromBuilder );
         ArgumentValidator.ValidateNotNull( nameof( whereBuilder ), whereBuilder );
         ArgumentValidator.ValidateNotNull( nameof( groupByBuilder ), groupByBuilder );
         ArgumentValidator.ValidateNotNull( nameof( havingBuilder ), havingBuilder );
         ArgumentValidator.ValidateNotNull( nameof( orderByBuilder ), orderByBuilder );

         this._colsBuilder = columnsBuilder;
         this._fromBuilder = fromBuilder;
         this._whereBuilder = whereBuilder;
         this._groupByBuilder = groupByBuilder;
         this._havingBuilder = havingBuilder;
         this._orderByBuilder = orderByBuilder;
      }

      public override QuerySpecification CreateExpression()
      {
         return this.vendor.QueryFactory.NewQuerySpecification(
            this._colsBuilder.CreateExpression(),
            this._fromBuilder.CanCreateMeaningfulExpression() ? this._fromBuilder.CreateExpression() : null,
            this._whereBuilder.CanCreateMeaningfulExpression() ? this._whereBuilder.CreateExpression() : null,
            this._groupByBuilder.CanCreateMeaningfulExpression() ? this._groupByBuilder.CreateExpression() : null,
            this._havingBuilder.CanCreateMeaningfulExpression() ? this._havingBuilder.CreateExpression() : null,
            this._orderByBuilder.CanCreateMeaningfulExpression() ? this._orderByBuilder.CreateExpression() : null,
            this._offset,
            this._limit
            );
      }

      public override Boolean CanCreateMeaningfulExpression()
      {
         return this._colsBuilder.CanCreateMeaningfulExpression();
      }

      #region QuerySpecificationBuilder Members

      public SelectColumnClauseBuilder ColumnsBuilder
      {
         get
         {
            return this._colsBuilder;
         }
      }

      public FromBuilder FromBuilder
      {
         get
         {
            return this._fromBuilder;
         }
      }

      public BooleanBuilder WhereBuilder
      {
         get
         {
            return this._whereBuilder;
         }
      }

      public GroupByBuilder GroupByBuilder
      {
         get
         {
            return this._groupByBuilder;
         }
      }

      public BooleanBuilder HavingBuilder
      {
         get
         {
            return this._havingBuilder;
         }
      }

      public OrderByBuilder OrderByBuilder
      {
         get
         {
            return this._orderByBuilder;
         }
      }

      public QuerySpecificationBuilder Limit( NonBooleanExpression max )
      {
         this._limit = max;
         return this;
      }

      public QuerySpecificationBuilder Offset( NonBooleanExpression skip )
      {
         this._offset = skip;
         return this;
      }

      public QuerySpecificationBuilder TrimGroupBy()
      {
         if ( this._havingBuilder.CreateExpression() != this.vendor.CommonFactory.Empty )
         {
            var grpCols = ( (GroupByBuilderImpl) this._groupByBuilder ).GroupingElements
               .OfType<OrdinaryGroupingSet>()
               .SelectMany( gs => gs.Columns )
               .OfType<ColumnReference>();
            this._groupByBuilder.AddGroupingElements(
               ( (SelectColumnClauseBuilderImpl) this._colsBuilder ).Cols.Select( col => col.Reference )
               .Except( grpCols )
               .Select( g => this.vendor.QueryFactory.NewGroupingElement( g ) )
               );
         }
         return this;
      }

      #endregion

   }

   public class QueryExpressionBodyBuilderImpl : AbstractBuilderImpl<QueryExpressionBody>, QueryExpressionBodyBuilder
   {
      private QueryExpressionBody _current;

      public QueryExpressionBodyBuilderImpl( SQLVendor vendor, QueryExpressionBody current )
         : base( vendor )
      {
         this._current = current ?? vendor.QueryFactory.Empty;
      }

      public override QueryExpressionBody CreateExpression()
      {
         return this._current;
      }

      public override bool CanCreateMeaningfulExpression()
      {
         return true;
      }

      #region QueryExpressionBodyBuilder Members

      public QueryExpressionBodyBuilder Union( QueryExpressionBody another, SetQuantifier setQuantifier = SetQuantifier.Distinct, CorrespondingSpec correspondingSpec = null )
      {
         this._current = this.vendor.QueryFactory.NewBinaryQuery( this._current, another, SetOperations.Union, setQuantifier, correspondingSpec );
         return this;
      }

      public QueryExpressionBodyBuilder Intersect( QueryExpressionBody another, SetQuantifier setQuantifier = SetQuantifier.Distinct, CorrespondingSpec correspondingSpec = null )
      {
         this._current = this.vendor.QueryFactory.NewBinaryQuery( this._current, another, SetOperations.Intersect, setQuantifier, correspondingSpec );
         return this;
      }

      public QueryExpressionBodyBuilder Except( QueryExpressionBody another, SetQuantifier setQuantifier = SetQuantifier.Distinct, CorrespondingSpec correspondingSpec = null )
      {
         this._current = this.vendor.QueryFactory.NewBinaryQuery( this._current, another, SetOperations.Except, setQuantifier, correspondingSpec );
         return this;
      }

      #endregion
   }
}
