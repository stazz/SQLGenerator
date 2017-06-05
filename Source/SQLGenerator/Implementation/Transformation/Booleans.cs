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

namespace SQLGenerator.Implementation.Transformation
{
   public class UnaryPredicateProcessor : AbstractBooleanProcessor<UnaryPredicate<ValueExpression>>
   {
      public enum UnaryOperatorOrientation { BeforeExpression, AfterExpression }

      private readonly UnaryOperatorOrientation _orientation;
      private readonly String _operator;
      private readonly String _negatedOperator;

      public UnaryPredicateProcessor( String op, String negOp, UnaryOperatorOrientation operatorOrientation = UnaryOperatorOrientation.AfterExpression )
      {
         ArgumentValidator.ValidateNotNull( nameof( op ), op );
         ArgumentValidator.ValidateNotNull( nameof( negOp ), negOp );

         this._orientation = operatorOrientation;
         this._operator = op;
         this._negatedOperator = negOp;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, UnaryPredicate<ValueExpression> obj, StringBuilder builder, Boolean negationActive )
      {
         var op = negationActive ? this._negatedOperator : this._operator;
         if ( UnaryOperatorOrientation.BeforeExpression == this._orientation )
         {
            builder.Append( op ).Append( SQLConstants.TOKEN_SEPARATOR );
         }
         var exp = obj.Expression;
         var isQuery = exp is QueryExpression;
         if ( isQuery )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS );
         }
         aggregator.Process( exp, builder );
         if ( isQuery )
         {
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }

         if ( UnaryOperatorOrientation.AfterExpression == this._orientation )
         {
            builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( op );
         }
      }
   }

   public class BinaryPredicateProcessor : AbstractBooleanProcessor<BinaryPredicate>
   {
      private readonly String _operator;
      private readonly String _negatedOperator;
      public BinaryPredicateProcessor( String op, String negOp )
      {
         ArgumentValidator.ValidateNotNull( nameof( op ), op );
         ArgumentValidator.ValidateNotNull( nameof( negOp ), negOp );

         this._operator = op;
         this._negatedOperator = negOp;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, BinaryPredicate obj, StringBuilder builder, Boolean negationActive )
      {
         var currentIsQuery = BinaryArithmeticExpressionProcessor.ExpressionIsQuery( obj.Left );
         if ( currentIsQuery )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS );
         }
         aggregator.Process( obj.Left, builder );
         if ( currentIsQuery )
         {
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
         builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( negationActive ? this._negatedOperator : this._operator ).Append( SQLConstants.TOKEN_SEPARATOR );
         currentIsQuery = BinaryArithmeticExpressionProcessor.ExpressionIsQuery( obj.Right );
         if ( currentIsQuery )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS );
         }
         aggregator.Process( obj.Right, builder );
         if ( currentIsQuery )
         {
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
      }
   }

   public class MultiPredicateProcessor : AbstractBooleanProcessor<MultiPredicate>
   {
      private readonly String _operator;
      private readonly String _negatedOperator;
      private readonly String _separator;
      private readonly Boolean _needParenthesis;

      public MultiPredicateProcessor( String op, String negOp, String separator, Boolean needParenthesis )
      {
         ArgumentValidator.ValidateNotNull( nameof( op ), op );
         ArgumentValidator.ValidateNotNull( nameof( negOp ), negOp );
         ArgumentValidator.ValidateNotNull( nameof( separator ), separator );

         this._operator = op;
         this._negatedOperator = negOp;
         this._separator = separator;
         this._needParenthesis = needParenthesis;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, MultiPredicate obj, StringBuilder builder, Boolean negationActive )
      {
         aggregator.Process( obj.Left, builder );
         builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( negationActive ? this._negatedOperator : this._operator ).Append( SQLConstants.TOKEN_SEPARATOR );
         if ( this._needParenthesis )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS );
         }

         ProcessorUtils.AppendEnumerable( aggregator, builder, obj.Rights, this._separator, ( agg, b, exp ) =>
         {
            var isQuery = exp is QueryExpression;
            if ( isQuery )
            {
               b.Append( SQLConstants.OPEN_PARENTHESIS );
            }
            agg.Process( exp, b );
            if ( isQuery )
            {
               b.Append( SQLConstants.CLOSE_PARENTHESIS );
            }
         } );

         if ( this._needParenthesis )
         {
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
      }
   }

   public class NegationProcessor : AbstractBooleanProcessor<Negation>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, Negation obj, StringBuilder builder, Boolean negationActive )
      {
         if ( !obj.NegatedExpression.IsEmpty() )
         {
            aggregator.Process( obj.NegatedExpression, builder, !negationActive );
         }
      }
   }

   public class ConjunctionProcessor : AbstractBooleanProcessor<Conjunction>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, Conjunction obj, StringBuilder builder, Boolean negationActive )
      {
         ProcessorUtils.ProcessBinaryComposedBooleanExpression( aggregator, builder, negationActive, SQLConstants.AND, obj.Left, obj.Right );
      }
   }

   public class DisjunctionProcessor : AbstractBooleanProcessor<Disjunction>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, Disjunction obj, StringBuilder builder, Boolean negationActive )
      {
         ProcessorUtils.ProcessBinaryComposedBooleanExpression( aggregator, builder, negationActive, SQLConstants.OR, obj.Left, obj.Right );
      }
   }

   public class BooleanTestProcessor : AbstractBooleanProcessor<BooleanTest>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, BooleanTest obj, StringBuilder builder, Boolean negationActive )
      {
         builder.Append( SQLConstants.OPEN_PARENTHESIS );
         aggregator.Process( obj.Expression, builder );
         builder.Append( SQLConstants.CLOSE_PARENTHESIS ).Append( SQLConstants.TOKEN_SEPARATOR ).Append( SQLConstants.IS ).Append( SQLConstants.TOKEN_SEPARATOR );
         if ( negationActive )
         {
            builder.Append( SQLConstants.NOT ).Append( SQLConstants.TOKEN_SEPARATOR );
         }
         builder.Append( obj.TruthValue.HasValue ? ( obj.TruthValue.Value ? SQLConstants.TRUE : SQLConstants.FALSE ) : SQLConstants.UNKNOWN );
      }
   }
}
