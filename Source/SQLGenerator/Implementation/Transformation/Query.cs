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


namespace SQLGenerator.Implementation.Transformation
{
   public abstract class ProcessorWithSetQuantifier<T> : AbstractProcessor<T>
      where T : class
   {
      private static readonly IDictionary<SetQuantifier, String> DEFAULT_SET_QUANTIFIERS = new Dictionary<SetQuantifier, String>
      {
         { SetQuantifier.All, "ALL" },
         { SetQuantifier.Distinct, "DISTINCT" }
      };

      private readonly IDictionary<SetQuantifier, String> _setQuantifiers;

      public ProcessorWithSetQuantifier( IDictionary<SetQuantifier, String> setQuantifiers = null )
      {
         this._setQuantifiers = setQuantifiers ?? DEFAULT_SET_QUANTIFIERS;
      }

      protected void ProcessSetQuantifier( SetQuantifier quantifier, StringBuilder builder )
      {
         builder.Append( this._setQuantifiers[quantifier] );
      }
   }

   public class QueryExpressionBodyBinaryProcessor : ProcessorWithSetQuantifier<QueryExpressionBodyBinary>
   {
      private static readonly IDictionary<SetOperations, String> DEFAULT_SET_OPERATIONS = new Dictionary<SetOperations, String>
      {
         { SetOperations.Except, "EXCEPT" },
         { SetOperations.Intersect, "INTERSECT" },
         { SetOperations.Union, "UNION" }
      };

      private readonly IDictionary<SetOperations, String> _setOperations;

      public QueryExpressionBodyBinaryProcessor( IDictionary<SetQuantifier, String> setQuantifiers = null, IDictionary<SetOperations, String> setOperations = null )
         : base( setQuantifiers )
      {
         this._setOperations = setOperations ?? DEFAULT_SET_OPERATIONS;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, QueryExpressionBodyBinary obj, StringBuilder builder )
      {
         var leftIsNonEmpty = !aggregator.SQLVendor.QueryFactory.Empty.Equals( obj.Left );
         if ( leftIsNonEmpty )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS );
            aggregator.Process( obj.Left, builder );
            builder
               .Append( SQLConstants.CLOSE_PARENTHESIS + SQLConstants.NEWLINE )
               .Append( this._setOperations[obj.SetOperation] )
               .Append( SQLConstants.TOKEN_SEPARATOR );
            this.ProcessSetQuantifier( obj.SetQuantifier, builder );

            var spec = obj.CorrespondingSpec;
            if ( spec != null )
            {
               builder.Append( SQLConstants.TOKEN_SEPARATOR );
               aggregator.Process( spec, builder );
            }

            builder.Append( SQLConstants.NEWLINE + SQLConstants.OPEN_PARENTHESIS );
         }

         aggregator.Process( obj.Right, builder );

         if ( leftIsNonEmpty )
         {
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
      }
   }

   public class QuerySpecificationProcessor : AbstractProcessor<QuerySpecification>
   {

      protected override void DoProcess( SQLProcessorAggregator aggregator, QuerySpecification obj, StringBuilder builder )
      {
         aggregator.Process( obj.Select, builder );
         aggregator.Process( obj.From, builder );
         ProcessorUtils.ProcessOptionalBooleanExpresssion( aggregator, builder, obj.Where, SQLConstants.NEWLINE, SQLConstants.WHERE );
         aggregator.Process( obj.GroupBy, builder );
         ProcessorUtils.ProcessOptionalBooleanExpresssion( aggregator, builder, obj.Having, SQLConstants.NEWLINE, SQLConstants.HAVING );
         aggregator.Process( obj.OrderBy, builder );
         if ( obj.Offset != null || obj.Limit != null )
         {
            this.ProcessOffsetAndLimit( aggregator, obj.Offset, obj.Limit, builder );
         }
      }

      protected virtual void ProcessOffsetAndLimit( SQLProcessorAggregator aggregator, NonBooleanExpression offset, NonBooleanExpression limit, StringBuilder builder )
      {
         NonBooleanExpression first, second;
         var offsetBeforeLimit = this.IsOffsetBeforeLimit( aggregator.SQLVendor );
         if ( offsetBeforeLimit )
         {
            first = offset;
            second = limit;
         }
         else
         {
            first = limit;
            second = offset;
         }

         if ( first != null )
         {
            builder.Append( SQLConstants.NEWLINE );
            this.ProcessOffsetOrLimit( aggregator, builder, first, offsetBeforeLimit );
         }

         if ( second != null )
         {
            builder.Append( SQLConstants.NEWLINE );
            this.ProcessOffsetOrLimit( aggregator, builder, second, !offsetBeforeLimit );
         }

      }

      protected virtual void ProcessOffsetOrLimit( SQLProcessorAggregator aggregator, StringBuilder builder, NonBooleanExpression expr, Boolean isOffset )
      {
         var prefix = isOffset ? this.GetOffsetPrefix( aggregator.SQLVendor ) : this.GetLimitPrefix( aggregator.SQLVendor );
         if ( prefix != null )
         {
            builder.Append( prefix ).Append( SQLConstants.TOKEN_SEPARATOR );
         }
         var isComplex = !( expr is LiteralExpression );
         if ( isComplex )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS + SQLConstants.NEWLINE );
         }
         aggregator.Process( expr, builder );
         if ( isComplex )
         {
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
         var postfix = isOffset ? this.GetOffsetPostfix( aggregator.SQLVendor ) : this.GetLimitPostfix( aggregator.SQLVendor );
         if ( postfix != null )
         {
            builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( postfix );
         }
      }

      protected virtual String GetOffsetPrefix( SQLVendor vendor )
      {
         return SQLConstants.OFFSET_PREFIX;
      }

      protected virtual String GetOffsetPostfix( SQLVendor vendor )
      {
         return SQLConstants.OFFSET_POSTFIX;
      }

      protected virtual String GetLimitPrefix( SQLVendor vendor )
      {
         return SQLConstants.LIMIT_PREFIX;
      }

      protected virtual String GetLimitPostfix( SQLVendor vendor )
      {
         return SQLConstants.LIMIT_POSTFIX;
      }

      protected virtual Boolean IsOffsetBeforeLimit( SQLVendor vendor )
      {
         return true;
      }
   }

   public abstract class AbstractSelectColumnsProcessor<T> : ProcessorWithSetQuantifier<T>
      where T : class, SelectColumnClause
   {
      public AbstractSelectColumnsProcessor( IDictionary<SetQuantifier, String> setQuantifiers = null )
         : base( setQuantifiers )
      {

      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, T obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.SELECT + SQLConstants.TOKEN_SEPARATOR );
         this.ProcessSetQuantifier( obj.SetQuantifier, builder );
         builder.Append( SQLConstants.TOKEN_SEPARATOR );
         this.DoProcessSelectColumns( aggregator, obj, builder );
      }

      protected abstract void DoProcessSelectColumns( SQLProcessorAggregator aggregator, T obj, StringBuilder builder );
   }

   public class ColumnReferencesProcessor : AbstractSelectColumnsProcessor<ColumnReferences>
   {
      public ColumnReferencesProcessor( IDictionary<SetQuantifier, String> setQuantifiers = null )
         : base( setQuantifiers )
      {
      }

      protected override void DoProcessSelectColumns( SQLProcessorAggregator aggregator, ColumnReferences obj, StringBuilder builder )
      {
         DoProcessSelectColumnsStatic( aggregator, obj, builder );
      }

      public static void DoProcessSelectColumnsStatic( SQLProcessorAggregator aggregator, ColumnReferences obj, StringBuilder builder )
      {
         ProcessorUtils.AppendEnumerable( aggregator, builder, obj.Columns, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR, ( agg, b, col ) =>
         {
            agg.Process( col.Reference, b );
            if ( !String.IsNullOrEmpty( col.Alias ) )
            {
               b.Append( SQLConstants.TOKEN_SEPARATOR + SQLConstants.ALIAS_DEFINER + SQLConstants.TOKEN_SEPARATOR ).Append( col.Alias );
            }
         } );
      }
   }

   public class ColumnReferenceByNameProcessor : AbstractProcessor<ColumnReferenceByName>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, ColumnReferenceByName obj, StringBuilder builder )
      {
         if ( !String.IsNullOrEmpty( obj.TableName ) )
         {
            builder.Append( obj.TableName ).Append( SQLConstants.TABLE_COLUMN_SEPARATOR );
         }
         builder.Append( obj.ColumnName );
      }
   }

   public class ColumnReferenceByExpressionProcessor : AbstractProcessor<ColumnReferenceByExpression>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, ColumnReferenceByExpression obj, StringBuilder builder )
      {
         aggregator.Process( obj.Expression, builder );
      }
   }

   public class AsteriskSelectProcessor : AbstractSelectColumnsProcessor<AsteriskSelect>
   {
      public AsteriskSelectProcessor( IDictionary<SetQuantifier, String> setQuantifiers = null )
         : base( setQuantifiers )
      {
      }

      protected override void DoProcessSelectColumns( SQLProcessorAggregator aggregator, AsteriskSelect obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.ASTERISK );
      }
   }

   public class FromClauseProcessor : AbstractProcessor<FromClause>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, FromClause obj, StringBuilder builder )
      {
         if ( obj.TableReferences.Length > 0 )
         {
            builder.Append( SQLConstants.NEWLINE + SQLConstants.FROM + SQLConstants.TOKEN_SEPARATOR );
            ProcessorUtils.AppendEnumerable( aggregator, builder, obj.TableReferences, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR );
         }
      }
   }

   public class QueryExpressionProcessor : AbstractProcessor<QueryExpression>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, QueryExpression obj, StringBuilder builder )
      {
         aggregator.Process( obj.Body, builder );
      }
   }

   public class CorrespondingSpecProcessor : AbstractProcessor<CorrespondingSpec>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, CorrespondingSpec obj, StringBuilder builder )
      {
         builder.Append( "CORRESPONDING" );
         if ( obj.ColumnNames != null )
         {
            builder.Append( SQLConstants.TOKEN_SEPARATOR + "BY" + SQLConstants.TOKEN_SEPARATOR );
            aggregator.Process( obj.ColumnNames, builder );
         }
      }
   }

   public class SortSpecificationProcessor : AbstractProcessor<SortSpecification>
   {

      private static readonly IDictionary<Ordering, String> DEFAULT_ORDERINGS = new Dictionary<Ordering, String>
      {
         { Ordering.Ascending, "ASC" },
         { Ordering.Descending, "DESC" }
      };

      private readonly IDictionary<Ordering, String> _orderings;

      public SortSpecificationProcessor( IDictionary<Ordering, String> orderings = null )
      {
         this._orderings = orderings ?? DEFAULT_ORDERINGS;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, SortSpecification obj, StringBuilder builder )
      {
         aggregator.Process( obj.ColumnReference, builder );
         builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( this._orderings[obj.Ordering] );
      }
   }

   public class OrdinaryGroupingSetProcessor : AbstractProcessor<OrdinaryGroupingSet>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, OrdinaryGroupingSet obj, StringBuilder builder )
      {
         ProcessorUtils.AppendEnumerable( aggregator, builder, obj.Columns, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR );
      }
   }

   public class GroupByClauseProcessor : AbstractProcessor<GroupByClause>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, GroupByClause obj, StringBuilder builder )
      {
         if ( obj.GroupingElements.Length > 0 )
         {
            builder.Append( SQLConstants.NEWLINE + SQLConstants.GROUP_BY + SQLConstants.TOKEN_SEPARATOR );
            ProcessorUtils.AppendEnumerable( aggregator, builder, obj.GroupingElements, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR );
         }
      }
   }

   public class OrderByClauseProcessor : AbstractProcessor<OrderByClause>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, OrderByClause obj, StringBuilder builder )
      {
         if ( obj.OrderingColumns.Length > 0 )
         {
            builder.Append( SQLConstants.NEWLINE + SQLConstants.ORDER_BY + SQLConstants.TOKEN_SEPARATOR );
            ProcessorUtils.AppendEnumerable( aggregator, builder, obj.OrderingColumns, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR );
         }
      }
   }

   public class TableValueConstructorProcessor : AbstractProcessor<TableValueConstructor>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, TableValueConstructor obj, StringBuilder builder )
      {
         builder.Append( "VALUES" + SQLConstants.TOKEN_SEPARATOR );
         ProcessorUtils.AppendEnumerable( aggregator, builder, obj.Rows, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR );
      }
   }

   public abstract class RowValueConstructorProcessor<T> : AbstractProcessor<T>
      where T : class, RowValueConstructor
   {

      protected override void DoProcess( SQLProcessorAggregator aggregator, T obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.OPEN_PARENTHESIS );
         this.DoProcessActually( aggregator, obj, builder );
         builder.Append( SQLConstants.CLOSE_PARENTHESIS );
      }

      protected abstract void DoProcessActually( SQLProcessorAggregator aggregator, T obj, StringBuilder builder );
   }

   public class RowSubQueryProcessor : RowValueConstructorProcessor<RowSubQuery>
   {
      protected override void DoProcessActually( SQLProcessorAggregator aggregator, RowSubQuery obj, StringBuilder builder )
      {
         aggregator.Process( obj.Query, builder );
      }
   }

   public class RowDefinitionProcessor : RowValueConstructorProcessor<RowDefinition>
   {
      protected override void DoProcessActually( SQLProcessorAggregator aggregator, RowDefinition obj, StringBuilder builder )
      {
         ProcessorUtils.AppendEnumerable( aggregator, builder, obj.RowElements, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR );
      }
   }

   public abstract class TableReferencePrimaryProcessor<T> : AbstractProcessor<T>
      where T : class, TableReferencePrimary
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, T obj, StringBuilder builder )
      {
         this.DoProcessTableReferencePrimary( aggregator, obj, builder );
         if ( obj.TableAlias != null )
         {
            this.ProcessTableAlias( aggregator, obj.TableAlias, builder );
         }
      }

      protected abstract void DoProcessTableReferencePrimary( SQLProcessorAggregator aggregator, T tRef, StringBuilder builder );

      protected virtual void ProcessTableAlias( SQLProcessorAggregator aggregator, TableAlias alias, StringBuilder builder )
      {
         builder.Append( SQLConstants.TOKEN_SEPARATOR + SQLConstants.ALIAS_DEFINER + SQLConstants.TOKEN_SEPARATOR ).Append( alias.TableAlias );
         if ( alias.ColumnAliases != null )
         {
            aggregator.Process( alias.ColumnAliases, builder );
         }
      }
   }

   public class TableReferenceByNameProcessor : TableReferencePrimaryProcessor<TableReferenceByName>
   {
      protected override void DoProcessTableReferencePrimary( SQLProcessorAggregator aggregator, TableReferenceByName tRef, StringBuilder builder )
      {
         aggregator.Process( tRef.TableName, builder );
      }
   }

   public class TableReferenceByQueryProcessor : TableReferencePrimaryProcessor<TableReferenceByQuery>
   {
      protected override void DoProcessTableReferencePrimary( SQLProcessorAggregator aggregator, TableReferenceByQuery tRef, StringBuilder builder )
      {
         builder.Append( SQLConstants.OPEN_PARENTHESIS );
         aggregator.Process( tRef.Query, builder );
         builder.Append( SQLConstants.CLOSE_PARENTHESIS );
      }
   }

   public abstract class JoinedTableProcessor<T> : AbstractProcessor<T>
      where T : class, JoinedTable
   {
      private static readonly IDictionary<JoinType, String> DEFAULT_JOIN_TYPES = new Dictionary<JoinType, String>
      {
         { JoinType.Inner, "INNER" },
         { JoinType.FullOuter, "FULL" },
         { JoinType.LeftOuter, "LEFT" },
         { JoinType.RightOuter, "RIGHT" }
      };

      private readonly IDictionary<JoinType, String> _joinTypes;

      protected JoinedTableProcessor( IDictionary<JoinType, String> joinTypes = null )
      {
         this._joinTypes = joinTypes ?? DEFAULT_JOIN_TYPES;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, T obj, StringBuilder builder )
      {
         aggregator.Process( obj.Left, builder );
         builder.Append( SQLConstants.NEWLINE );
         this.DoProcessJoinedTable( aggregator, obj, builder );
      }

      protected abstract void DoProcessJoinedTable( SQLProcessorAggregator aggregator, T obj, StringBuilder builder );

      protected void ProcessJoinType( JoinType joinType, StringBuilder builder )
      {
         builder.Append( this._joinTypes[joinType] ).Append( " JOIN " );
      }
   }

   public class CrossJoinedTableProcessor : JoinedTableProcessor<CrossJoinedTable>
   {
      public CrossJoinedTableProcessor( IDictionary<JoinType, String> joinTypes = null )
         : base( joinTypes )
      {
      }

      protected override void DoProcessJoinedTable( SQLProcessorAggregator aggregator, CrossJoinedTable obj, StringBuilder builder )
      {
         builder.Append( " CROSS JOIN " );
         aggregator.Process( obj.Right, builder );
      }
   }

   public class NaturalJoinedTableProcessor : JoinedTableProcessor<NaturalJoinedTable>
   {
      public NaturalJoinedTableProcessor( IDictionary<JoinType, String> joinTypes = null )
         : base( joinTypes )
      {
      }

      protected override void DoProcessJoinedTable( SQLProcessorAggregator aggregator, NaturalJoinedTable obj, StringBuilder builder )
      {
         builder.Append( " NATURAL " );
         this.ProcessJoinType( obj.JoinType, builder );
         builder.Append( SQLConstants.TOKEN_SEPARATOR );
         aggregator.Process( obj.Right, builder );
      }
   }

   public class QualifiedJoinedTableProcessor : JoinedTableProcessor<QualifiedJoinedTable>
   {
      public QualifiedJoinedTableProcessor( IDictionary<JoinType, String> joinTypes = null )
         : base( joinTypes )
      {
      }

      protected override void DoProcessJoinedTable( SQLProcessorAggregator aggregator, QualifiedJoinedTable obj, StringBuilder builder )
      {
         this.ProcessJoinType( obj.JoinType, builder );
         aggregator.Process( obj.Right, builder );
         aggregator.Process( obj.JoinSpecification, builder );
      }
   }

   public class UnionJoinedTableProcessor : JoinedTableProcessor<UnionJoinedTable>
   {
      public UnionJoinedTableProcessor( IDictionary<JoinType, String> joinTypes = null )
         : base( joinTypes )
      {
      }

      protected override void DoProcessJoinedTable( SQLProcessorAggregator aggregator, UnionJoinedTable obj, StringBuilder builder )
      {
         builder.Append( " UNION JOIN " );
         aggregator.Process( obj.Right, builder );
      }
   }

   public class JoinConditionProcessor : AbstractProcessor<JoinCondition>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, JoinCondition obj, StringBuilder builder )
      {
         builder.Append( " ON " );
         aggregator.Process( obj.SearchCondition, builder );
      }
   }

   public class NamedColumnsJoinProcessor : AbstractProcessor<NamedColumnsJoin>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, NamedColumnsJoin obj, StringBuilder builder )
      {
         builder.Append( " USING " );
         aggregator.Process( obj.ColumnNames, builder );
      }
   }
}
