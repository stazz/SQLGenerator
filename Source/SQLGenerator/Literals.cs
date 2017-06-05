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
   public interface LiteralExpression : NonBooleanExpression
   {
   }

   public interface SQLFunctionLiteral : LiteralExpression
   {
      String FunctionName { get; }
      ImmutableArray<ValueExpression> Parameters { get; }
   }

   public interface DirectLiteral : LiteralExpression
   {
      [OptionalSQLElement]
      String TextContents { get; }
   }

   public interface NumericLiteral<T> : LiteralExpression
      where T : struct
   {
      T? Number { get; }
   }
   public interface Int32NumericLiteral : NumericLiteral<Int32>
   {
   }
   public interface Int64NumericLiteral : NumericLiteral<Int64>
   {
   }
   public interface DoubleNumericLiteral : NumericLiteral<Double>
   {
   }
   public interface DecimalNumericLiteral : NumericLiteral<Decimal>
   {
   }

   public interface StringLiteral : LiteralExpression
   {
      String String { get; }
   }

   public interface TemporalLiteral : LiteralExpression
   {
   }

   public interface TimestampLiteral : TemporalLiteral
   {
      DateTime? Timestamp { get; }
   }
}
