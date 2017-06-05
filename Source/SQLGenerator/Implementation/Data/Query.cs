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

using SQLGenerator.Implementation.Transformation;
using UtilPack;

namespace SQLGenerator.Implementation.Data
{
   public abstract class SelectColumnClauseImpl : SQLElementBase, SelectColumnClause
   {
      private readonly SetQuantifier _setQuantifier;

      protected SelectColumnClauseImpl( SQLVendorImpl vendor, SetQuantifier setQuantifier )
         : base( vendor )
      {
         this._setQuantifier = setQuantifier;
      }

      #region SelectColumnClause Members

      public SetQuantifier SetQuantifier
      {
         get
         {
            return this._setQuantifier;
         }
      }

      #endregion
   }

   public class AsteriskSelectImpl : SelectColumnClauseImpl, AsteriskSelect
   {
      public AsteriskSelectImpl( SQLVendorImpl vendor, SetQuantifier setQuantifier )
         : base( vendor, setQuantifier )
      {
      }
   }

   public class ColumnReferencesImpl : SelectColumnClauseImpl, ColumnReferences
   {
      private readonly ImmutableArray<ColumnReferenceInfo> _columns;

      public ColumnReferencesImpl( SQLVendorImpl vendor, SetQuantifier setQuantifier, ImmutableArray<ColumnReferenceInfo> columns )
         : base( vendor, setQuantifier )
      {
         ArgumentValidator.ValidateNotNull( nameof( columns ), columns );
         if ( columns.Length <= 0 )
         {
            throw new ArgumentException( "Select column list must have at least one column reference." );
         }
         foreach ( var col in columns )
         {
            ArgumentValidator.ValidateNotNull( nameof( col ), col );
         }

         this._columns = columns;
      }

      #region ColumnReferences Members

      public ImmutableArray<ColumnReferenceInfo> Columns
      {
         get
         {
            return this._columns;
         }
      }

      #endregion
   }

   public class ColumnReferenceByExpressionImpl : SQLElementBase, ColumnReferenceByExpression
   {
      private readonly ValueExpression _expression;

      public ColumnReferenceByExpressionImpl( SQLVendorImpl vendor, ValueExpression expression )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( expression ), expression );

         this._expression = expression;
      }

      #region ColumnReferenceByExpression Members

      public ValueExpression Expression
      {
         get
         {
            return this._expression;
         }
      }

      #endregion

      public override Boolean Equals( Object obj )
      {
         return Object.ReferenceEquals( this, obj ) || ( obj is ColumnReferenceByExpression && Object.Equals( this._expression, ( (ColumnReferenceByExpression) obj ).Expression ) );
      }

      public override Int32 GetHashCode()
      {
         return this._expression.GetHashCode();
      }
   }

   public class ColumnReferenceByNameImpl : SQLElementBase, ColumnReferenceByName
   {
      private readonly String _tableName;
      private readonly String _colName;

      public ColumnReferenceByNameImpl( SQLVendorImpl vendor, String tableName, String colName )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( colName ), colName );

         this._tableName = tableName;
         this._colName = colName;
      }

      #region ColumnReferenceByName Members

      public String TableName
      {
         get
         {
            return this._tableName;
         }
      }

      public String ColumnName
      {
         get
         {
            return this._colName;
         }
      }

      #endregion

      public override Boolean Equals( Object obj )
      {
         return Object.ReferenceEquals( this, obj ) || ( obj is ColumnReferenceByName && Object.Equals( this._tableName, ( (ColumnReferenceByName) obj ).TableName ) && Object.Equals( this._colName, ( (ColumnReferenceByName) obj ).ColumnName ) );
      }

      public override Int32 GetHashCode()
      {
         return this._colName.GetHashCode();
      }
   }

   public class CorrespondingSpecImpl : SQLElementBase, CorrespondingSpec
   {
      private readonly ColumnNameList _columns;

      public CorrespondingSpecImpl( SQLVendorImpl vendor, ColumnNameList cols )
         : base( vendor )
      {
         this._columns = cols;
      }

      #region CorrespondingSpec Members

      public ColumnNameList ColumnNames
      {
         get
         {
            return this._columns;
         }
      }

      #endregion
   }

   public class FromClauseImpl : SQLElementBase, FromClause
   {
      private readonly ImmutableArray<TableReference> _tables;

      public FromClauseImpl( SQLVendorImpl vendor, ImmutableArray<TableReference> tableReferences )
         : base( vendor )
      {
         tableReferences.ValidateNotEmpty( nameof( tableReferences ) );
         foreach ( var tableReference in tableReferences )
         {
            ArgumentValidator.ValidateNotNull( nameof( tableReference ), tableReference );
         }
         this._tables = tableReferences;
      }

      #region FromClause Members

      public ImmutableArray<TableReference> TableReferences
      {
         get
         {
            return this._tables;
         }
      }

      #endregion
   }

   public class GroupByClauseImpl : SQLElementBase, GroupByClause
   {
      private readonly ImmutableArray<GroupingElement> _elements;

      public GroupByClauseImpl( SQLVendorImpl vendor, ImmutableArray<GroupingElement> groupingElements )
         : base( vendor )
      {
         groupingElements.ValidateNotEmpty( nameof( groupingElements ) );
         foreach ( var groupingElement in groupingElements )
         {
            ArgumentValidator.ValidateNotNull( nameof( groupingElement ), groupingElement );
         }

         this._elements = groupingElements;
      }

      #region GroupByClause Members

      public ImmutableArray<GroupingElement> GroupingElements
      {
         get
         {
            return this._elements;
         }
      }

      #endregion
   }

   public class OrderByClauseImpl : SQLElementBase, OrderByClause
   {
      private readonly ImmutableArray<SortSpecification> _orderingColumns;

      public OrderByClauseImpl( SQLVendorImpl vendor, ImmutableArray<SortSpecification> sortSpecifications )
         : base( vendor )
      {
         sortSpecifications.ValidateNotEmpty( nameof( sortSpecifications ) );
         foreach ( var sortSpecificaton in sortSpecifications )
         {
            ArgumentValidator.ValidateNotNull( nameof( sortSpecificaton ), sortSpecificaton );
         }

         this._orderingColumns = sortSpecifications;
      }

      #region OrderByClause Members

      public ImmutableArray<SortSpecification> OrderingColumns
      {
         get
         {
            return this._orderingColumns;
         }
      }

      #endregion
   }

   public class OrdinaryGroupingSetImpl : SQLElementBase, OrdinaryGroupingSet
   {
      private readonly ImmutableArray<NonBooleanExpression> _columns;

      public OrdinaryGroupingSetImpl( SQLVendorImpl vendor, ImmutableArray<NonBooleanExpression> columns )
         : base( vendor )
      {
         columns.ValidateNotEmpty( nameof( columns ) );
         foreach ( var column in columns )
         {
            ArgumentValidator.ValidateNotNull( nameof( column ), column );
         }
         this._columns = columns;
      }

      #region OrdinaryGroupingSet Members

      public ImmutableArray<NonBooleanExpression> Columns
      {
         get
         {
            return this._columns;
         }
      }

      #endregion
   }

   public class QueryExpressionBodyBinaryImpl : SQLElementBase, QueryExpressionBodyBinary
   {
      private readonly SetOperations _setOperation;
      private readonly CorrespondingSpec _correspondingSpec;
      private readonly QueryExpressionBody _left;
      private readonly QueryExpressionBody _right;
      private readonly SetQuantifier _setQuantifier;

      public QueryExpressionBodyBinaryImpl( SQLVendorImpl vendor, SetOperations setOperation, CorrespondingSpec correspondingSpec, QueryExpressionBody left, QueryExpressionBody right, SetQuantifier setQuantifier )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( left ), left );
         ArgumentValidator.ValidateNotNull( nameof( right ), right );

         this._setOperation = setOperation;
         this._correspondingSpec = correspondingSpec;
         this._left = left;
         this._right = right;
         this._setQuantifier = setQuantifier;
      }

      #region QueryExpressionBodyBinary Members

      public SetOperations SetOperation
      {
         get
         {
            return this._setOperation;
         }
      }

      public CorrespondingSpec CorrespondingSpec
      {
         get
         {
            return this._correspondingSpec;
         }
      }

      public QueryExpressionBody Left
      {
         get
         {
            return this._left;
         }
      }

      public QueryExpressionBody Right
      {
         get
         {
            return this._right;
         }
      }

      public SetQuantifier SetQuantifier
      {
         get
         {
            return this._setQuantifier;
         }
      }

      #endregion
   }

   public class QueryExpressionImpl : SQLElementBase, QueryExpression
   {
      private readonly QueryExpressionBody _body;

      public QueryExpressionImpl( SQLVendorImpl vendor, QueryExpressionBody body )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( body ), body );

         this._body = body;
      }

      #region QueryExpression Members

      public QueryExpressionBody Body
      {
         get
         {
            return this._body;
         }
      }

      #endregion
   }

   public class QuerySpecificationImpl : SQLElementBase, QuerySpecification
   {
      private readonly SelectColumnClause _select;
      private readonly FromClause _from;
      private readonly BooleanExpression _where;
      private readonly GroupByClause _groupBy;
      private readonly BooleanExpression _having;
      private readonly OrderByClause _orderBy;
      private readonly NonBooleanExpression _offset;
      private readonly NonBooleanExpression _limit;

      public QuerySpecificationImpl( SQLVendorImpl vendor, SelectColumnClause select, FromClause from, BooleanExpression where, GroupByClause groupBy, BooleanExpression having, OrderByClause orderBy, NonBooleanExpression offset, NonBooleanExpression limit )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( select ), select );

         this._select = select;
         this._from = from;
         this._where = where;
         this._groupBy = groupBy;
         this._having = having;
         this._orderBy = orderBy;
         this._offset = offset;
         this._limit = limit;
      }

      #region QuerySpecification Members

      public SelectColumnClause Select
      {
         get
         {
            return this._select;
         }
      }

      public FromClause From
      {
         get
         {
            return this._from;
         }
      }

      public BooleanExpression Where
      {
         get
         {
            return this._where;
         }
      }

      public GroupByClause GroupBy
      {
         get
         {
            return this._groupBy;
         }
      }

      public BooleanExpression Having
      {
         get
         {
            return this._having;
         }
      }

      public OrderByClause OrderBy
      {
         get
         {
            return this._orderBy;
         }
      }

      public NonBooleanExpression Limit
      {
         get
         {
            return this._limit;
         }
      }

      public NonBooleanExpression Offset
      {
         get
         {
            return this._offset;
         }
      }

      #endregion
   }

   public class RowDefinitionImpl : SQLElementBase, RowDefinition
   {
      private readonly ImmutableArray<ValueExpression> _elements;

      public RowDefinitionImpl( SQLVendorImpl vendor, ImmutableArray<ValueExpression> elements )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( elements ), elements );

         foreach ( var el in elements )
         {
            ArgumentValidator.ValidateNotNull( nameof( el ), el );
         }

         this._elements = elements;
      }

      #region RowDefinition Members

      public ImmutableArray<ValueExpression> RowElements
      {
         get
         {
            return this._elements;
         }
      }

      #endregion
   }

   public class RowSubQueryImpl : SQLElementBase, RowSubQuery
   {
      private readonly QueryExpression _query;

      public RowSubQueryImpl( SQLVendorImpl vendor, QueryExpression query )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( query ), query );

         this._query = query;
      }

      #region RowSubQuery Members

      public QueryExpression Query
      {
         get
         {
            return this._query;
         }
      }

      #endregion
   }

   public class SortSpecificationImpl : SQLElementBase, SortSpecification
   {
      private readonly Ordering _ordering;
      private readonly ValueExpression _expression;

      public SortSpecificationImpl( SQLVendorImpl vendor, Ordering ordering, ValueExpression expression )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( expression ), expression );

         this._ordering = ordering;
         this._expression = expression;
      }

      #region SortSpecification Members

      public Ordering Ordering
      {
         get
         {
            return this._ordering;
         }
      }

      public ValueExpression ColumnReference
      {
         get
         {
            return this._expression;
         }
      }

      #endregion
   }

   public class TableAliasImpl : SQLElementBase, TableAlias
   {
      private readonly String _tableAlias;
      private readonly ColumnNameList _columnAliases;

      public TableAliasImpl( SQLVendorImpl vendor, String tableAlias, ColumnNameList columnAliases )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotEmpty( nameof( tableAlias ), tableAlias );

         this._tableAlias = tableAlias;
         this._columnAliases = columnAliases;
      }

      #region TableAlias Members

      public String TableAlias
      {
         get
         {
            return this._tableAlias;
         }
      }

      public ColumnNameList ColumnAliases
      {
         get
         {
            return this._columnAliases;
         }
      }

      #endregion
   }

   public abstract class TableReferencePrimaryImpl : SQLElementBase, TableReferencePrimary
   {
      private readonly TableAlias _tableAlias;

      protected TableReferencePrimaryImpl( SQLVendorImpl vendor, TableAlias tableAlias )
         : base( vendor )
      {
         this._tableAlias = tableAlias;
      }

      #region TableReferencePrimary Members

      public TableAlias TableAlias
      {
         get
         {
            return this._tableAlias;
         }
      }

      #endregion
   }

   public class TableReferenceByQueryImpl : TableReferencePrimaryImpl, TableReferenceByQuery
   {
      private readonly QueryExpression _query;

      public TableReferenceByQueryImpl( SQLVendorImpl vendor, TableAlias tableAlias, QueryExpression query )
         : base( vendor, tableAlias )
      {
         ArgumentValidator.ValidateNotNull( nameof( query ), query );

         this._query = query;
      }

      #region TableReferenceByQuery Members

      public QueryExpression Query
      {
         get
         {
            return this._query;
         }
      }

      #endregion
   }

   public class TableReferenceByNameImpl : TableReferencePrimaryImpl, TableReferenceByName
   {
      private readonly TableName _tableName;

      public TableReferenceByNameImpl( SQLVendorImpl vendor, TableAlias tableAlias, TableName tableName )
         : base( vendor, tableAlias )
      {
         ArgumentValidator.ValidateNotNull( nameof( tableName ), tableName );

         this._tableName = tableName;
      }

      #region TableReferenceByName Members

      public TableName TableName
      {
         get
         {
            return this._tableName;
         }
      }

      #endregion
   }

   public class TableValueConstructorImpl : SQLElementBase, TableValueConstructor
   {
      private readonly ImmutableArray<RowValueConstructor> _rows;

      public TableValueConstructorImpl( SQLVendorImpl vendor, ImmutableArray<RowValueConstructor> rows )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( rows ), rows );

         foreach ( var row in rows )
         {
            ArgumentValidator.ValidateNotNull( nameof( row ), row );
         }

         this._rows = rows;
      }

      #region TableValueConstructor Members

      public ImmutableArray<RowValueConstructor> Rows
      {
         get
         {
            return this._rows;
         }
      }

      #endregion
   }

   public abstract class JoinedTableImpl : SQLElementBase, JoinedTable
   {
      private readonly TableReference _left;
      private readonly TableReference _right;

      protected JoinedTableImpl( SQLVendorImpl vendor, TableReference left, TableReference right )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( left ), left );
         ArgumentValidator.ValidateNotNull( nameof( right ), right );

         this._left = left;
         this._right = right;
      }

      #region JoinedTable Members

      public TableReference Left
      {
         get
         {
            return this._left;
         }
      }

      public TableReference Right
      {
         get
         {
            return this._right;
         }
      }

      #endregion
   }

   public class CrossJoinedTableImpl : JoinedTableImpl, CrossJoinedTable
   {
      public CrossJoinedTableImpl( SQLVendorImpl vendor, TableReference left, TableReference right )
         : base( vendor, left, right )
      {
      }
   }

   public class NaturalJoinedTableImpl : JoinedTableImpl, NaturalJoinedTable
   {
      private readonly JoinType _joinType;

      public NaturalJoinedTableImpl( SQLVendorImpl vendor, TableReference left, TableReference right, JoinType jt )
         : base( vendor, left, right )
      {
         this._joinType = jt;
      }

      #region NaturalJoinedTable Members

      public JoinType JoinType
      {
         get
         {
            return this._joinType;
         }
      }

      #endregion
   }

   public class QualifiedJoinedTableImpl : JoinedTableImpl, QualifiedJoinedTable
   {
      private readonly JoinType _joinType;
      private readonly JoinSpecification _joinSpec;

      public QualifiedJoinedTableImpl( SQLVendorImpl vendor, TableReference left, TableReference right, JoinType jt, JoinSpecification js )
         : base( vendor, left, right )
      {
         ArgumentValidator.ValidateNotNull( nameof( js ), js );

         this._joinType = jt;
         this._joinSpec = js;
      }

      #region QualifiedJoinedTable Members

      public JoinType JoinType
      {
         get
         {
            return this._joinType;
         }
      }

      public JoinSpecification JoinSpecification
      {
         get
         {
            return this._joinSpec;
         }
      }

      #endregion
   }

   public class UnionJoinedTableImpl : JoinedTableImpl, UnionJoinedTable
   {
      public UnionJoinedTableImpl( SQLVendorImpl vendor, TableReference left, TableReference right )
         : base( vendor, left, right )
      {
      }
   }

   public class JoinConditionImpl : SQLElementBase, JoinCondition
   {
      private readonly BooleanExpression _condition;

      public JoinConditionImpl( SQLVendorImpl vendor, BooleanExpression condition )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( condition ), condition );

         this._condition = condition;
      }
      #region JoinCondition Members

      public BooleanExpression SearchCondition
      {
         get
         {
            return this._condition;
         }
      }

      #endregion
   }

   public class NamedColumnsJoinImpl : SQLElementBase, NamedColumnsJoin
   {
      private readonly ColumnNameList _columns;

      public NamedColumnsJoinImpl( SQLVendorImpl vendor, ColumnNameList cols )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( cols ), cols );

         this._columns = cols;
      }

      #region NamedColumnsJoin Members

      public ColumnNameList ColumnNames
      {
         get
         {
            return this._columns;
         }
      }

      #endregion
   }

   static class GroupingElements
   {
      public class GrandTotal : SQLElementBase, GroupingElement
      {
         public GrandTotal( SQLVendorImpl vendor )
            : base( vendor )
         {
         }
      }
   }

   static class QueryExpressionBodies
   {
      public class EmptyExpressionBody : SQLElementBase, QueryExpressionBody
      {
         public EmptyExpressionBody( SQLVendorImpl vendor )
            : base( vendor )
         {
         }
      }
   }
}
