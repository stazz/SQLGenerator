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
using System.Threading;

using SQLGenerator.Implementation.Transformation;
using UtilPack;

namespace SQLGenerator.Implementation.Data
{
   public abstract class UnaryPredicateImpl<TTargetExpression> : SQLElementBase, UnaryPredicate<TTargetExpression>
      where TTargetExpression : class, ValueExpression
   {
      private readonly TTargetExpression _expression;

      protected UnaryPredicateImpl( SQLVendorImpl vendor, TTargetExpression expression )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( expression ), expression );
         this._expression = expression;
      }

      #region UnaryPredicate<TTargetExpression> Members

      public TTargetExpression Expression
      {
         get
         {
            return this._expression;
         }
      }

      #endregion
   }

   public abstract class BinaryPredicateImpl<TExpression> : SQLElementBase, BinaryPredicate
   {
      private readonly ValueExpression _left;
      private readonly ValueExpression _right;

      protected BinaryPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ValueExpression right )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( left ), left );
         ArgumentValidator.ValidateNotNull( nameof( right ), right );
         this._left = left;
         this._right = right;
      }

      #region BinaryPredicate Members

      public ValueExpression Left
      {
         get
         {
            return this._left;
         }
      }

      public ValueExpression Right
      {
         get
         {
            return this._right;
         }
      }

      #endregion
   }

   public abstract class MultiPredicateImpl<TExpression> : SQLElementBase, MultiPredicate
      where TExpression : MultiPredicate
   {
      private readonly ValueExpression _left;
      private readonly ImmutableArray<ValueExpression> _rights;

      protected MultiPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ImmutableArray<ValueExpression> rights )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( left ), left );
         ArgumentValidator.ValidateNotNull( nameof( rights ), rights );

         this._left = left;
         this._rights = rights;
      }

      #region MultiPredicate Members

      public ValueExpression Left
      {
         get
         {
            return this._left;
         }
      }

      public ImmutableArray<ValueExpression> Rights
      {
         get
         {
            return this._rights;
         }
      }

      #endregion
   }

   public class BetweenPredicateImpl : MultiPredicateImpl<BetweenPredicate>, BetweenPredicate
   {
      public BetweenPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ImmutableArray<ValueExpression> rights )
         : base( vendor, left, rights )
      {
         if ( rights.Length < 2 )
         {
            throw new ArgumentException( "Too few arguments for BETWEEN predicate (needed 2, had " + rights.Length + ")." );
         }
      }
   }

   public class EqualsPredicateImpl : BinaryPredicateImpl<EqualsPredicate>, EqualsPredicate
   {
      public EqualsPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ValueExpression right )
         : base( vendor, left, right )
      {
      }
   }

   public class ExistsPredicateImpl : UnaryPredicateImpl<QueryExpression>, ExistsPredicate
   {
      public ExistsPredicateImpl( SQLVendorImpl vendor, QueryExpression expression )
         : base( vendor, expression )
      {
      }
   }

   public class GreaterOrEqualToPredicateImpl : BinaryPredicateImpl<GreaterOrEqualToPredicate>, GreaterOrEqualToPredicate
   {
      public GreaterOrEqualToPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ValueExpression right )
         : base( vendor, left, right )
      {
      }
   }

   public class GreaterThanPredicateImpl : BinaryPredicateImpl<GreaterThanPredicate>, GreaterThanPredicate
   {
      public GreaterThanPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ValueExpression right )
         : base( vendor, left, right )
      {
      }
   }

   public class InPredicateImpl : MultiPredicateImpl<InPredicate>, InPredicate
   {
      public InPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ImmutableArray<ValueExpression> rights )
         : base( vendor, left, rights )
      {
      }
   }

   public class IsNullPredicateImpl : UnaryPredicateImpl<ValueExpression>, IsNullPredicate
   {
      public IsNullPredicateImpl( SQLVendorImpl vendor, ValueExpression expression )
         : base( vendor, expression )
      {
      }
   }

   public class LessOrEqualToPredicateImpl : BinaryPredicateImpl<LessOrEqualToPredicate>, LessOrEqualToPredicate
   {
      public LessOrEqualToPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ValueExpression right )
         : base( vendor, left, right )
      {
      }
   }

   public class LessThanPredicateImpl : BinaryPredicateImpl<LessThanPredicate>, LessThanPredicate
   {
      public LessThanPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ValueExpression right )
         : base( vendor, left, right )
      {
      }
   }

   public class LikePredicateImpl : BinaryPredicateImpl<LikePredicate>, LikePredicate
   {
      public LikePredicateImpl( SQLVendorImpl vendor, ValueExpression left, ValueExpression right )
         : base( vendor, left, right )
      {
      }
   }

   public class NotEqualsPredicateImpl : BinaryPredicateImpl<NotEqualsPredicate>, NotEqualsPredicate
   {
      public NotEqualsPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ValueExpression right )
         : base( vendor, left, right )
      {

      }
   }

   public class RegexpPredicateImpl : BinaryPredicateImpl<RegexpPredicate>, RegexpPredicate
   {
      public RegexpPredicateImpl( SQLVendorImpl vendor, ValueExpression left, ValueExpression right )
         : base( vendor, left, right )
      {
      }
   }

   public class UniquePredicateImpl : UnaryPredicateImpl<QueryExpression>, UniquePredicate
   {
      public UniquePredicateImpl( SQLVendorImpl vendor, QueryExpression query )
         : base( vendor, query )
      {
      }
   }

   public abstract class ComposedBooleanExpressionImpl : SQLElementBase, ComposedBooleanExpression
   {
      private readonly Lazy<IEnumerable<BooleanExpression>> _expressions;

      protected ComposedBooleanExpressionImpl( SQLVendorImpl vendor, Func<BooleanExpression[]> composedExpressionsFunc )
         : base( vendor )
      {
         this._expressions = new Lazy<IEnumerable<BooleanExpression>>( () => composedExpressionsFunc().Skip( 0 ), LazyThreadSafetyMode.ExecutionAndPublication );
      }

      #region ComposedBooleanExpression Members

      public IEnumerable<BooleanExpression> ComposedExpressions
      {
         get
         {
            return this._expressions.Value;
         }
      }

      #endregion
   }

   public class BooleanTestImpl : ComposedBooleanExpressionImpl, BooleanTest
   {
      private readonly BooleanExpression _expression;
      private readonly Boolean? _value;

      public BooleanTestImpl( SQLVendorImpl vendor, BooleanExpression expression, Boolean? value )
         : base( vendor, () => new BooleanExpression[] { expression } )
      {
         ArgumentValidator.ValidateNotNull( nameof( expression ), expression );
         if ( expression.IsEmpty() )
         {
            throw new ArgumentException( "Boolean test must be on non-empty boolean expression." );
         }

         this._expression = expression;
         this._value = value;
      }

      #region BooleanTest Members

      public BooleanExpression Expression
      {
         get
         {
            return this._expression;
         }
      }

      public Boolean? TruthValue
      {
         get
         {
            return this._value;
         }
      }

      #endregion
   }

   public class ConjunctionImpl : ComposedBooleanExpressionImpl, Conjunction
   {
      private readonly BooleanExpression _left;
      private readonly BooleanExpression _right;

      public ConjunctionImpl( SQLVendorImpl vendor, BooleanExpression left, BooleanExpression right )
         : base( vendor, () => new BooleanExpression[] { left, right } )
      {
         ArgumentValidator.ValidateNotNull( nameof( left ), left );
         ArgumentValidator.ValidateNotNull( nameof( right ), right );

         this._left = left;
         this._right = right;
      }

      #region Conjunction Members

      public BooleanExpression Left
      {
         get
         {
            return this._left;
         }
      }

      public BooleanExpression Right
      {
         get
         {
            return this._right;
         }
      }

      #endregion
   }

   public class DisjunctionImpl : ComposedBooleanExpressionImpl, Disjunction
   {
      private readonly BooleanExpression _left;
      private readonly BooleanExpression _right;

      public DisjunctionImpl( SQLVendorImpl vendor, BooleanExpression left, BooleanExpression right )
         : base( vendor, () => new BooleanExpression[] { left, right } )
      {
         ArgumentValidator.ValidateNotNull( nameof( left ), left );
         ArgumentValidator.ValidateNotNull( nameof( right ), right );

         this._left = left;
         this._right = right;
      }

      #region Disjunction Members

      public BooleanExpression Left
      {
         get
         {
            return this._left;
         }
      }

      public BooleanExpression Right
      {
         get
         {
            return this._right;
         }
      }

      #endregion
   }

   public class NegationImpl : ComposedBooleanExpressionImpl, Negation
   {
      private readonly BooleanExpression _negated;

      public NegationImpl( SQLVendorImpl vendor, BooleanExpression negated )
         : base( vendor, () => new BooleanExpression[] { negated } )
      {
         ArgumentValidator.ValidateNotNull( nameof( negated ), negated );

         this._negated = negated;
      }

      #region Negation Members

      public BooleanExpression NegatedExpression
      {
         get
         {
            return this._negated;
         }
      }

      #endregion
   }

   public static class BooleanExpressions
   {
      public class True : SQLElementBase, BooleanExpression
      {
         public True( SQLVendorImpl vendor )
            : base( vendor )
         {
         }
      }

      public class False : SQLElementBase, BooleanExpression
      {
         public False( SQLVendorImpl vendor )
            : base( vendor )
         {
         }
      }

      public class EmptyPredicate : SQLElementBase, Predicate
      {
         public EmptyPredicate( SQLVendorImpl vendor )
            : base( vendor )
         {
         }
      }
   }
}
