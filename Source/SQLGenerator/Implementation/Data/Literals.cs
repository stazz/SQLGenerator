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
   public class DirectLiteralImpl : SQLElementBase, DirectLiteral
   {
      private readonly String _contents;

      public DirectLiteralImpl( SQLVendorImpl vendor, String literal )
         : base( vendor )
      {
         this._contents = literal;
      }

      #region DirectLiteral Members

      public String TextContents
      {
         get
         {
            return this._contents;
         }
      }

      #endregion
   }

   public abstract class NumericLiteralImpl<TNumber> : SQLElementBase, NumericLiteral<TNumber>
      where TNumber : struct
   {
      private readonly TNumber? _number;

      protected NumericLiteralImpl( SQLVendorImpl vendor, TNumber? number )
         : base( vendor )
      {
         this._number = number;
      }

      #region NumericLiteral<TNumber> Members

      public TNumber? Number
      {
         get
         {
            return this._number;
         }
      }

      #endregion
   }

   public class Int32NumericLiteralImpl : NumericLiteralImpl<Int32>, Int32NumericLiteral
   {
      public Int32NumericLiteralImpl( SQLVendorImpl vendor, Int32? i32 )
         : base( vendor, i32 )
      {
      }
   }

   public class Int64NumericLiteralImpl : NumericLiteralImpl<Int64>, Int64NumericLiteral
   {
      public Int64NumericLiteralImpl( SQLVendorImpl vendor, Int64? i64 )
         : base( vendor, i64 )
      {
      }
   }

   public class DoubleNumericLiteralImpl : NumericLiteralImpl<Double>, DoubleNumericLiteral
   {
      public DoubleNumericLiteralImpl( SQLVendorImpl vendor, Double? dbl )
         : base( vendor, dbl )
      {
      }
   }

   public class DecimalNumericLiteralImpl : NumericLiteralImpl<Decimal>, DecimalNumericLiteral
   {
      public DecimalNumericLiteralImpl( SQLVendorImpl vendor, Decimal? dcml )
         : base( vendor, dcml )
      {
      }
   }

   public class SQLFunctionLiteralImpl : SQLElementBase, SQLFunctionLiteral
   {
      private readonly String _name;
      private readonly ImmutableArray<ValueExpression> _parameters;

      public SQLFunctionLiteralImpl( SQLVendorImpl vendor, String name, ImmutableArray<ValueExpression> parameters )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( name ), name );
         ArgumentValidator.ValidateNotNull( nameof( parameters ), parameters );

         this._name = name;
         this._parameters = parameters;
      }

      #region SQLFunctionLiteral Members

      public String FunctionName
      {
         get
         {
            return this._name;
         }
      }

      public ImmutableArray<ValueExpression> Parameters
      {
         get
         {
            return this._parameters;
         }
      }

      #endregion

      public override Boolean Equals( Object obj )
      {
         return Object.ReferenceEquals( this, obj ) || ( obj is SQLFunctionLiteral && this._name.Equals( ( (SQLFunctionLiteral) obj ).FunctionName ) && this._parameters.SequenceEqual( ( (SQLFunctionLiteral) obj ).Parameters ) );
      }

      public override Int32 GetHashCode()
      {
         return this._name.GetHashCode();
      }
   }

   public class StringLiteralImpl : SQLElementBase, StringLiteral
   {
      private readonly String _str;

      public StringLiteralImpl( SQLVendorImpl vendor, String str )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( str ), str );

         this._str = str;
      }

      #region StringLiteral Members

      public String String
      {
         get
         {
            return this._str;
         }
      }

      #endregion
   }

   public class TimestampLiteralImpl : SQLElementBase, TimestampLiteral
   {
      private readonly DateTime? _timestamp;

      public TimestampLiteralImpl( SQLVendorImpl vendor, DateTime? timestamp )
         : base( vendor )
      {
         this._timestamp = timestamp;
      }

      #region TimestampLiteral Members

      public DateTime? Timestamp
      {
         get
         {
            return this._timestamp;
         }
      }

      #endregion
   }
}
