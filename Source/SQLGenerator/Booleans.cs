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
   public interface BooleanExpression : ValueExpression
   {
   }

   public interface ComposedBooleanExpression : BooleanExpression
   {
      IEnumerable<BooleanExpression> ComposedExpressions { get; }
   }

   public interface Predicate : BooleanExpression
   {
   }

   public interface BooleanTest : ComposedBooleanExpression
   {
      BooleanExpression Expression { get; }
      Boolean? TruthValue { get; }
   }

   public interface Conjunction : ComposedBooleanExpression
   {
      BooleanExpression Left { get; }
      BooleanExpression Right { get; }
   }

   public interface Disjunction : ComposedBooleanExpression
   {
      BooleanExpression Left { get; }
      BooleanExpression Right { get; }
   }

   public interface Negation : ComposedBooleanExpression
   {
      BooleanExpression NegatedExpression { get; }
   }

   public interface UnaryPredicate<out TExpression> : Predicate
      where TExpression : ValueExpression
   {
      TExpression Expression { get; }
   }

   public interface BinaryPredicate : Predicate
   {
      ValueExpression Left { get; }
      ValueExpression Right { get; }
   }

   public interface MultiPredicate : Predicate
   {
      ValueExpression Left { get; }
      ImmutableArray<ValueExpression> Rights { get; }
   }

   public interface BetweenPredicate : MultiPredicate
   {
   }

   public interface EqualsPredicate : BinaryPredicate
   {
   }

   public interface NotEqualsPredicate : BinaryPredicate
   {
   }

   public interface ExistsPredicate : UnaryPredicate<QueryExpression>
   {
   }

   public interface GreaterOrEqualToPredicate : BinaryPredicate
   {
   }

   public interface GreaterThanPredicate : BinaryPredicate
   {
   }

   public interface InPredicate : MultiPredicate
   {
   }

   //public interface IsNotNullPredicate : UnaryPredicate<NonBooleanExpression>
   //{
   //}

   public interface IsNullPredicate : UnaryPredicate<ValueExpression>
   {
   }

   public interface LessOrEqualToPredicate : BinaryPredicate
   {
   }

   public interface LessThanPredicate : BinaryPredicate
   {
   }

   public interface LikePredicate : BinaryPredicate
   {
   }

   public interface RegexpPredicate : BinaryPredicate
   {
   }

   public interface UniquePredicate : UnaryPredicate<QueryExpression>
   {
   }


}

public static partial class E_SQLGenerator
{
   public static ValueExpression GetMaximum( this BetweenPredicate predicate )
   {
      return predicate.Rights[1];
   }

   public static ValueExpression GetMinimum( this BetweenPredicate predicate )
   {
      return predicate.Rights[0];
   }

   public static Boolean IsEmpty( this BooleanExpression expression )
   {
      return expression == expression.SQLVendor.CommonFactory.Empty ||
         ( expression is ComposedBooleanExpression && ( (ComposedBooleanExpression) expression ).ComposedExpressions.All( e => IsEmpty( e ) ) );
   }
}
