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
using UtilPack;

namespace SQLGenerator.Implementation.Transformation
{
   public class SQLProcessorAggregator : ObjectWithVendor
   {
      private static readonly IDictionary<Type, UnaryPredicateProcessor.UnaryOperatorOrientation> DEFAULT_UNARY_ORIENTATIONS;
      private static readonly IDictionary<Type, Tuple<String, String>> DEFAULT_OPERATORS;
      private static readonly IDictionary<Type, String> DEFAULT_SEPARATORS;
      private static readonly IDictionary<Type, Boolean> DEFAULT_PARENTHESIS_POLICIES;
      private static readonly IDictionary<Type, SQLProcessor> DEFAULT_PROCESSORS;

      static SQLProcessorAggregator()
      {
         var unaryOrientations = new Dictionary<Type, UnaryPredicateProcessor.UnaryOperatorOrientation>( 3 );
         unaryOrientations.Add( typeof( IsNullPredicateImpl ), UnaryPredicateProcessor.UnaryOperatorOrientation.AfterExpression );
         unaryOrientations.Add( typeof( ExistsPredicateImpl ), UnaryPredicateProcessor.UnaryOperatorOrientation.BeforeExpression );
         unaryOrientations.Add( typeof( UniquePredicateImpl ), UnaryPredicateProcessor.UnaryOperatorOrientation.BeforeExpression );
         DEFAULT_UNARY_ORIENTATIONS = unaryOrientations;

         var operators = new Dictionary<Type, Tuple<String, String>>( 13 );
         // Unary operators
         operators.Add( typeof( IsNullPredicateImpl ), Tuple.Create( "IS NULL", "IS NOT NULL" ) );
         operators.Add( typeof( ExistsPredicateImpl ), Tuple.Create( "EXISTS", "NOT EXISTS" ) );
         operators.Add( typeof( UniquePredicateImpl ), Tuple.Create( "UNIQUE", "NOT UNIQUE" ) );

         // Binary operators
         operators.Add( typeof( EqualsPredicateImpl ), Tuple.Create( "=", "<>" ) );
         operators.Add( typeof( NotEqualsPredicateImpl ), Tuple.Create( "<>", "=" ) );
         operators.Add( typeof( GreaterOrEqualToPredicateImpl ), Tuple.Create( ">=", "<" ) );
         operators.Add( typeof( GreaterThanPredicateImpl ), Tuple.Create( ">", ">=" ) );
         operators.Add( typeof( LessOrEqualToPredicateImpl ), Tuple.Create( "<=", ">" ) );
         operators.Add( typeof( LessThanPredicateImpl ), Tuple.Create( "<", ">=" ) );
         operators.Add( typeof( LikePredicateImpl ), Tuple.Create( "LIKE", "NOT LIKE" ) );
         operators.Add( typeof( RegexpPredicateImpl ), Tuple.Create( "SIMILAR TO", "NOT SIMILAR TO" ) );

         // Multi predicates
         operators.Add( typeof( BetweenPredicateImpl ), Tuple.Create( "BETWEEN", "NOT BETWEEN" ) );
         operators.Add( typeof( InPredicateImpl ), Tuple.Create( "IN", "NOT IN" ) );
         DEFAULT_OPERATORS = operators;

         // Separators
         var separators = new Dictionary<Type, String>( 2 );
         separators.Add( typeof( BetweenPredicateImpl ), " AND " );
         separators.Add( typeof( InPredicateImpl ), ", " );
         DEFAULT_SEPARATORS = separators;

         // Parenthesis policies
         var pPolicies = new Dictionary<Type, Boolean>( 2 );
         pPolicies.Add( typeof( BetweenPredicateImpl ), false );
         pPolicies.Add( typeof( InPredicateImpl ), true );
         DEFAULT_PARENTHESIS_POLICIES = pPolicies;

         // Processors
         var processors = new Dictionary<Type, SQLProcessor>();

         // Boolean expressions
         // Constants
         processors.Add( typeof( BooleanExpressions.True ), new ConstantProcessor( "TRUE" ) );
         processors.Add( typeof( BooleanExpressions.False ), new ConstantProcessor( "FALSE" ) );
         processors.Add( typeof( BooleanExpressions.EmptyPredicate ), new NoOpProcessor() );
         // Unary predicates
         AddNewUnaryPredicateProcessor( typeof( IsNullPredicateImpl ), processors, operators, unaryOrientations );
         AddNewUnaryPredicateProcessor( typeof( ExistsPredicateImpl ), processors, operators, unaryOrientations );
         AddNewUnaryPredicateProcessor( typeof( UniquePredicateImpl ), processors, operators, unaryOrientations );
         // Binary predicates
         AddNewBinaryPredicateProcessor( typeof( EqualsPredicateImpl ), processors, operators );
         AddNewBinaryPredicateProcessor( typeof( NotEqualsPredicateImpl ), processors, operators );
         AddNewBinaryPredicateProcessor( typeof( GreaterOrEqualToPredicateImpl ), processors, operators );
         AddNewBinaryPredicateProcessor( typeof( GreaterThanPredicateImpl ), processors, operators );
         AddNewBinaryPredicateProcessor( typeof( LessOrEqualToPredicateImpl ), processors, operators );
         AddNewBinaryPredicateProcessor( typeof( LessThanPredicateImpl ), processors, operators );
         AddNewBinaryPredicateProcessor( typeof( LikePredicateImpl ), processors, operators );
         AddNewBinaryPredicateProcessor( typeof( RegexpPredicateImpl ), processors, operators );
         // Multi predicates
         AddNewMultiPredicateProcessor( typeof( BetweenPredicateImpl ), processors, operators, separators, pPolicies );
         AddNewMultiPredicateProcessor( typeof( InPredicateImpl ), processors, operators, separators, pPolicies );
         // Composed
         processors.Add( typeof( ConjunctionImpl ), new ConjunctionProcessor() );
         processors.Add( typeof( DisjunctionImpl ), new DisjunctionProcessor() );
         processors.Add( typeof( NegationImpl ), new NegationProcessor() );
         processors.Add( typeof( BooleanTestImpl ), new BooleanTestProcessor() );

         // Common
         processors.Add( typeof( BinaryArithmeticExpressionImpl ), new BinaryArithmeticExpressionProcessor() );
         processors.Add( typeof( UnaryArithmeticExpressionImpl ), new UnaryArithmeticExpressionProcessor() );
         processors.Add( typeof( ColumnNameListImpl ), new ColumnNamesProcessor() );
         processors.Add( typeof( TableNameDirectImpl ), new TableNameDirectProcessor() );
         processors.Add( typeof( TableNameFunctionImpl ), new TableNameFunctionProcessor() );

         // Data Types
         processors.Add( typeof( SQLDTBigIntImpl ), new ConstantProcessor( "BIGINT" ) );
         processors.Add( typeof( SQLDTDoublePrecisionImpl ), new ConstantProcessor( "DOUBLE PRECISION" ) );
         processors.Add( typeof( SQLDTRealImpl ), new ConstantProcessor( "REAL" ) );
         processors.Add( typeof( SQLDTSmallIntImpl ), new ConstantProcessor( "SMALLINT" ) );
         processors.Add( typeof( SQLDTBooleanImpl ), new ConstantProcessor( "BOOLEAN" ) );
         processors.Add( typeof( SQLDTDateImpl ), new ConstantProcessor( "DATE" ) );
         processors.Add( typeof( SQLDTIntegerImpl ), new ConstantProcessor( "INTEGER" ) );
         processors.Add( typeof( SQLDTUserDefinedImpl ), new SQLDTUserDefinedProcessor() );
         processors.Add( typeof( SQLDTDecimalImpl ), new SQLDTDecimalProcessor() );
         processors.Add( typeof( SQLDTNumericImpl ), new SQLDTNumericProcessor() );
         processors.Add( typeof( SQLDTCharImpl ), new SQLDTCharProcessor() );
         processors.Add( typeof( SQLDTFloatImpl ), new SQLDTFloatProcessor() );
         processors.Add( typeof( SQLDTIntervalImpl ), new SQLDTIntervalProcessor() );
         processors.Add( typeof( SQLDTTimeImpl ), new SQLDTGenericTimeProcessor<SQLDTTime>( "TIME" ) );
         processors.Add( typeof( SQLDTTimeStampImpl ), new SQLDTGenericTimeProcessor<SQLDTTimestamp>( "TIMESTAMP" ) );

         // Definitions
         processors.Add( typeof( SchemaDefinitionImpl ), new SchemaDefinitionProcessor() );
         processors.Add( typeof( TableDefinitionImpl ), new TableDefinitionProcessor() );
         processors.Add( typeof( TableElementListImpl ), new TableElementListProcessor() );
         processors.Add( typeof( ColumnDefinitionImpl ), new ColumnDefinitionProcessor() );
         processors.Add( typeof( LikeClauseImpl ), new LikeClauseProcessor() );
         processors.Add( typeof( TableConstraintDefinitionImpl ), new TableConstraintDefinitionProcessor() );
         processors.Add( typeof( CheckConstraintImpl ), new CheckConstraintProcessor() );
         processors.Add( typeof( UniqueConstraintImpl ), new UniqueConstraintProcessor() );
         processors.Add( typeof( ForeignKeyConstraintImpl ), new ForeignKeyConstraintProcessor() );
         processors.Add( typeof( ViewDefinitionImpl ), new ViewDefinitionProcessor() );
         processors.Add( typeof( RegularViewSpecificationImpl ), new RegularViewSpecificationProcessor() );

         // Literals
         processors.Add( typeof( StringLiteralImpl ), new StringLiteralExpressionProcessor() );
         processors.Add( typeof( TimestampLiteralImpl ), new TimestampLiteralProcessor() );
         processors.Add( typeof( SQLFunctionLiteralImpl ), new SQLFunctionLiteralProcessor() );
         processors.Add( typeof( DirectLiteralImpl ), new DirectLiteralProcessor() );
         processors.Add( typeof( Int32NumericLiteralImpl ), new NumericLiteralProcessor<Int32NumericLiteral, Int32>() );
         processors.Add( typeof( Int64NumericLiteralImpl ), new NumericLiteralProcessor<Int64NumericLiteral, Int64>() );
         processors.Add( typeof( DoubleNumericLiteralImpl ), new NumericLiteralProcessor<DoubleNumericLiteral, Double>() );
         processors.Add( typeof( DecimalNumericLiteralImpl ), new NumericLiteralProcessor<DecimalNumericLiteral, Decimal>() );

         // Manipulation
         processors.Add( typeof( AlterTableStatementImpl ), new AlterTableStatementProcessor() );
         processors.Add( typeof( AddColumnDefinitionImpl ), new AddColumnDefinitionProcessor() );
         processors.Add( typeof( AddTableConstraintDefinitionImpl ), new AddTableConstraintDefinitionProcessor() );
         processors.Add( typeof( AlterColumnDefinitionImpl ), new AlterColumnDefinitionProcessor() );
         processors.Add( typeof( ManipulationFactoryImpl.DropDefault ), new ConstantProcessor( "DROP DEFAULT" ) );
         processors.Add( typeof( SetColumnDefaultImpl ), new SetColumnDefaultProcessor() );
         processors.Add( typeof( DropColumnDefinitionImpl ), new DropColumnDefinitionProcessor() );
         processors.Add( typeof( DropTableConstraintDefinitionImpl ), new DropTableConstraintDefinitionProcessor() );
         processors.Add( typeof( DropSchemaStatementImpl ), new DropSchemaStatementProcessor() );
         processors.Add( typeof( DropTableOrViewStatementImpl ), new DropTableOrViewStatementProcessor() );

         // Modification
         processors.Add( typeof( ColumnSourceByQueryImpl ), new ColumnSourceByQueryProcessor() );
         processors.Add( typeof( ColumnSourceByValuesImpl ), new ColumnSourceByValuesProcessor() );
         processors.Add( typeof( DeleteBySearchImpl ), new DeleteBySearchProcessor() );
         processors.Add( typeof( InsertStatementImpl ), new InsertStatementProcessor() );
         processors.Add( typeof( SetClauseImpl ), new SetClauseProcessor() );
         processors.Add( typeof( TargetTableImpl ), new TargetTableProcessor() );
         processors.Add( typeof( UpdateBySearchImpl ), new UpdateBySearchProcessor() );
         processors.Add( typeof( UpdateSourceByExpressionImpl ), new UpdateSourceByExpressionProcessor() );
         processors.Add( typeof( ValueSources.Default ), new ConstantProcessor( "DEFAULT" ) );
         processors.Add( typeof( ValueSources.Null ), new ConstantProcessor( "NULL" ) );
         processors.Add( typeof( ColumnSources.Defaults ), new ConstantProcessor( " DEFAULT VALUES" ) );

         // Query
         processors.Add( typeof( ColumnReferenceByNameImpl ), new ColumnReferenceByNameProcessor() );
         processors.Add( typeof( ColumnReferenceByExpressionImpl ), new ColumnReferenceByExpressionProcessor() );
         processors.Add( typeof( QueryExpressionBodyBinaryImpl ), new QueryExpressionBodyBinaryProcessor() );
         processors.Add( typeof( QuerySpecificationImpl ), new QuerySpecificationProcessor() );
         processors.Add( typeof( QueryExpressionImpl ), new QueryExpressionProcessor() );
         processors.Add( typeof( QueryExpressionBodies.EmptyExpressionBody ), new NoOpProcessor() );
         processors.Add( typeof( CorrespondingSpecImpl ), new CorrespondingSpecProcessor() );
         processors.Add( typeof( GroupingElements.GrandTotal ), new ConstantProcessor( SQLConstants.OPEN_PARENTHESIS + SQLConstants.CLOSE_PARENTHESIS ) );
         processors.Add( typeof( OrdinaryGroupingSetImpl ), new OrdinaryGroupingSetProcessor() );
         processors.Add( typeof( SortSpecificationImpl ), new SortSpecificationProcessor() );
         processors.Add( typeof( GroupByClauseImpl ), new GroupByClauseProcessor() );
         processors.Add( typeof( OrderByClauseImpl ), new OrderByClauseProcessor() );
         processors.Add( typeof( FromClauseImpl ), new FromClauseProcessor() );
         processors.Add( typeof( AsteriskSelectImpl ), new AsteriskSelectProcessor() );
         processors.Add( typeof( ColumnReferencesImpl ), new ColumnReferencesProcessor() );
         processors.Add( typeof( TableValueConstructorImpl ), new TableValueConstructorProcessor() );
         processors.Add( typeof( RowDefinitionImpl ), new RowDefinitionProcessor() );
         processors.Add( typeof( RowSubQueryImpl ), new RowSubQueryProcessor() );
         processors.Add( typeof( TableReferenceByNameImpl ), new TableReferenceByNameProcessor() );
         processors.Add( typeof( TableReferenceByQueryImpl ), new TableReferenceByQueryProcessor() );
         processors.Add( typeof( CrossJoinedTableImpl ), new CrossJoinedTableProcessor() );
         processors.Add( typeof( NaturalJoinedTableImpl ), new NaturalJoinedTableProcessor() );
         processors.Add( typeof( QualifiedJoinedTableImpl ), new QualifiedJoinedTableProcessor() );
         processors.Add( typeof( UnionJoinedTableImpl ), new UnionJoinedTableProcessor() );
         processors.Add( typeof( JoinConditionImpl ), new JoinConditionProcessor() );
         processors.Add( typeof( NamedColumnsJoinImpl ), new NamedColumnsJoinProcessor() );

         DEFAULT_PROCESSORS = processors;
      }

      private static void AddNewUnaryPredicateProcessor( Type predicateType, IDictionary<Type, SQLProcessor> processors, IDictionary<Type, Tuple<String, String>> operators, IDictionary<Type, UnaryPredicateProcessor.UnaryOperatorOrientation> unaryOperatorOrientations )
      {
         processors.Add( predicateType, new UnaryPredicateProcessor( operators[predicateType].Item1, operators[predicateType].Item2, unaryOperatorOrientations[predicateType] ) );
      }

      private static void AddNewBinaryPredicateProcessor( Type predicateType, IDictionary<Type, SQLProcessor> processors, IDictionary<Type, Tuple<String, String>> operators )
      {
         processors.Add( predicateType, new BinaryPredicateProcessor( operators[predicateType].Item1, operators[predicateType].Item2 ) );
      }

      private static void AddNewMultiPredicateProcessor( Type predicateType, IDictionary<Type, SQLProcessor> processors, IDictionary<Type, Tuple<String, String>> operators, IDictionary<Type, String> separators, IDictionary<Type, Boolean> parenthesisPolicies )
      {
         processors.Add( predicateType, new MultiPredicateProcessor( operators[predicateType].Item1, operators[predicateType].Item2, separators[predicateType], parenthesisPolicies[predicateType] ) );
      }
      private readonly SQLVendor _vendor;
      private readonly IDictionary<Type, SQLProcessor> _processors;

      public SQLProcessorAggregator( SQLVendor vendor, IDictionary<Type, SQLProcessor> processors = null )
      {
         ArgumentValidator.ValidateNotNull( nameof( vendor ), vendor );

         this._vendor = vendor;
         this._processors = processors ?? GetDefaultProcessorsCopy();
      }

      public void Process( ObjectWithVendor obj, StringBuilder builder, Boolean negationActive = false )
      {
         SQLProcessor processor;
         if ( obj != null && this._processors.TryGetValue( obj.GetType(), out processor ) )
         {
            processor.Process( this, obj, builder, negationActive );
         }
#if DEBUG
         else if ( obj != null )
         {
            throw new InvalidOperationException( "The vendor " + this._vendor + " does not know how to handle element of type " + obj.GetType() + "." );
         }
#endif
      }

      #region ObjectWithVendor Members

      public SQLVendor SQLVendor
      {
         get
         {
            return this._vendor;
         }
      }

      #endregion

      public static IDictionary<Type, SQLProcessor> GetDefaultProcessorsCopy()
      {
         return new Dictionary<Type, SQLProcessor>( DEFAULT_PROCESSORS );
      }
   }

   public interface SQLProcessor
   {
      void Process( SQLProcessorAggregator aggregator, Object obj, StringBuilder builder, Boolean negationActive );
   }

   public abstract class AbstractBooleanProcessor<T> : SQLProcessor
      where T : class
   {

      #region SQLProcessor Members

      public void Process( SQLProcessorAggregator aggregator, Object obj, StringBuilder builder, Boolean negationActive )
      {
         if ( obj != null )
         {
            this.DoProcess( aggregator, (T) obj, builder, negationActive );
         }
      }

      #endregion

      protected abstract void DoProcess( SQLProcessorAggregator aggregator, T obj, StringBuilder builder, Boolean negationActive );
   }

   public abstract class AbstractProcessor<T> : SQLProcessor
      where T : class
   {

      #region SQLProcessor Members

      public void Process( SQLProcessorAggregator aggregator, Object obj, StringBuilder builder, Boolean negationActive )
      {
         if ( obj != null )
         {
            this.DoProcess( aggregator, (T) obj, builder );
         }
      }

      #endregion

      protected abstract void DoProcess( SQLProcessorAggregator aggregator, T obj, StringBuilder builder );
   }

   public static class ProcessorUtils
   {
      public static void AppendEnumerable<U>( SQLProcessorAggregator aggregator, StringBuilder builder, ImmutableArray<U> aq, String separator, Action<SQLProcessorAggregator, StringBuilder, U> action )
      {
         var array = aq.ArrayObject;
         for ( var idx = 0; idx < array.Length; ++idx )
         {
            action( aggregator, builder, array[idx] );


            if ( idx + 1 < array.Length )
            {
               builder.Append( separator );
            }
         }
      }
      public static void AppendEnumerable<U>( SQLProcessorAggregator aggregator, StringBuilder builder, ImmutableArray<U> aq, String separator )
         where U : ObjectWithVendor
      {
         var array = aq.ArrayObject;
         for ( var idx = 0; idx < array.Length; ++idx )
         {
            aggregator.Process( array[idx], builder );


            if ( idx + 1 < array.Length )
            {
               builder.Append( separator );
            }
         }
      }

      public static void ProcessBinaryComposedBooleanExpression( SQLProcessorAggregator aggregator, StringBuilder builder, Boolean negationActive, String op, BooleanExpression left, BooleanExpression right )
      {
         var leftEmpty = left.IsEmpty();
         var rightEmpty = right.IsEmpty();
         if ( !leftEmpty || !rightEmpty )
         {
            if ( negationActive )
            {
               builder.Append( SQLConstants.NOT );
            }
            var oneEmpty = leftEmpty || rightEmpty;
            var parenthesisNeeded = !oneEmpty || negationActive;
            if ( parenthesisNeeded )
            {
               builder.Append( SQLConstants.OPEN_PARENTHESIS );
            }
            aggregator.Process( left, builder );

            if ( !oneEmpty )
            {
               builder.Append( SQLConstants.TOKEN_SEPARATOR );
               builder.Append( op );
               builder.Append( SQLConstants.TOKEN_SEPARATOR );
            }
            aggregator.Process( right, builder );
            if ( parenthesisNeeded )
            {
               builder.Append( SQLConstants.CLOSE_PARENTHESIS );
            }
         }
      }

      public static void ProcessOptionalBooleanExpresssion( SQLProcessorAggregator aggregator, StringBuilder builder, BooleanExpression expression, String prefix, String name )
      {
         if ( expression != null && !expression.IsEmpty() )
         {
            ProcessNonOptional( aggregator, expression, builder, prefix, name );
         }
      }

      public static void ProcessOptional( SQLProcessorAggregator aggregator, ObjectWithVendor obj, StringBuilder builder, Boolean negationActive, String prefix, String name )
      {
         if ( obj != null )
         {
            ProcessNonOptional( aggregator, obj, builder, prefix, name );
         }
      }

      public static void ProcessNonOptional( SQLProcessorAggregator aggregator, ObjectWithVendor obj, StringBuilder builder, String prefix, String name )
      {
         builder.Append( prefix );
         if ( name != null )
         {
            builder.Append( name ).Append( SQLConstants.TOKEN_SEPARATOR );
         }
         aggregator.Process( obj, builder );
      }
   }
}
