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

   public abstract class SQLDTWithPrecisionImpl : SQLElementBase, SQLDataTypeWithPrecision
   {
      private readonly Int32? _precision;

      protected SQLDTWithPrecisionImpl( SQLVendorImpl vendor, Int32? precision )
         : base( vendor )
      {
         this._precision = precision;
      }

      #region SQLDataTypeWithPrecision Members

      public Int32? Precision
      {
         get
         {
            return this._precision;
         }
      }

      #endregion

      //protected override bool DoesEqual( TDT another )
      //{
      //   return this.Precision == another.Precision;
      //}
   }

   public abstract class SQLDTWithPrecisionAndScaleImpl : SQLDTWithPrecisionImpl, SQLDataTypeWithPrecisionAndScale
   {
      private readonly Int32? _scale;

      protected SQLDTWithPrecisionAndScaleImpl( SQLVendorImpl vendor, Int32? precision, Int32? scale )
         : base( vendor, precision )
      {
         this._scale = scale;
      }

      #region SQLDataTypeWithPrecisionAndScale Members

      public Int32? Scale
      {
         get
         {
            return this._scale;
         }
      }

      #endregion

      //protected override Boolean DoesEqual( TDT another )
      //{
      //   return base.DoesEqual( another ) && this.Scale == another.Scale;
      //}
   }

   public abstract class SQLDTAbstractTimeImpl : SQLDTWithPrecisionImpl, SQLDTAbstractTime
   {
      private readonly Boolean? _isWithTimeZone;

      protected SQLDTAbstractTimeImpl( SQLVendorImpl vendor, Int32? precision, Boolean? isWithTimeZone )
         : base( vendor, precision )
      {
         this._isWithTimeZone = isWithTimeZone;
      }

      #region SQLDTAbstractTime Members

      public Boolean? IsWithTimeZone
      {
         get
         {
            return this._isWithTimeZone;
         }
      }

      #endregion

      //protected override Boolean DoesEqual( TDT another )
      //{
      //   return base.DoesEqual( another ) && this._isWithTimeZone == another.IsWithTimeZone;
      //}
   }

   public class SQLDTBigIntImpl : SQLElementBase, SQLDTBigInt
   {
      public SQLDTBigIntImpl( SQLVendorImpl vendor )
         : base( vendor )
      {
      }
   }

   public class SQLDTDecimalImpl : SQLDTWithPrecisionAndScaleImpl, SQLDTDecimal
   {
      public SQLDTDecimalImpl( SQLVendorImpl vendor, Int32? precision, Int32? scale )
         : base( vendor, precision, scale )
      {

      }
   }

   public class SQLDTDoublePrecisionImpl : SQLElementBase, SQLDTDoublePrecision
   {
      public SQLDTDoublePrecisionImpl( SQLVendorImpl vendor )
         : base( vendor )
      {
      }
   }

   public class SQLDTNumericImpl : SQLDTWithPrecisionAndScaleImpl, SQLDTNumeric
   {
      public SQLDTNumericImpl( SQLVendorImpl vendor, Int32? precision, Int32? scale )
         : base( vendor, precision, scale )
      {
      }
   }

   public class SQLDTRealImpl : SQLElementBase, SQLDTReal
   {
      public SQLDTRealImpl( SQLVendorImpl vendor )
         : base( vendor )
      {
      }
   }

   public class SQLDTSmallIntImpl : SQLElementBase, SQLDTSmallInt
   {
      public SQLDTSmallIntImpl( SQLVendorImpl vendor )
         : base( vendor )
      {
      }
   }

   public class SQLDTBooleanImpl : SQLElementBase, SQLDTBoolean
   {
      public SQLDTBooleanImpl( SQLVendorImpl vendor )
         : base( vendor )
      {
      }
   }

   public class SQLDTCharImpl : SQLElementBase, SQLDTChar
   {
      private readonly Boolean _isVarying;
      private readonly Int32? _length;

      public SQLDTCharImpl( SQLVendorImpl vendor, Boolean isVarying, Int32? lenght )
         : base( vendor )
      {
         this._isVarying = isVarying;
         this._length = lenght;
      }

      #region SQLDTChar Members

      public Boolean IsVarying
      {
         get
         {
            return this._isVarying;
         }
      }

      public Int32? Length
      {
         get
         {
            return this._length;
         }
      }

      #endregion
   }

   public class SQLDTDateImpl : SQLElementBase, SQLDTDate
   {
      public SQLDTDateImpl( SQLVendorImpl vendor )
         : base( vendor )
      {
      }
   }

   public class SQLDTFloatImpl : SQLDTWithPrecisionImpl, SQLDTFloat
   {
      public SQLDTFloatImpl( SQLVendorImpl vendor, Int32? precision )
         : base( vendor, precision )
      {
      }
   }

   public class SQLDTIntegerImpl : SQLElementBase, SQLDTInteger
   {
      public SQLDTIntegerImpl( SQLVendorImpl vendor )
         : base( vendor )
      {
      }
   }

   public class SQLDTIntervalImpl : SQLElementBase, SQLDTInterval
   {
      private readonly IntervalDataTypes _startFieldType;
      private readonly Int32? _startFieldPrecision;
      private readonly IntervalDataTypes? _endFieldType;
      private readonly Int32? _secondFracs;

      public SQLDTIntervalImpl( SQLVendorImpl vendor, IntervalDataTypes startFieldType, Int32? startFieldPrecision, IntervalDataTypes? endFieldType, Int32? secondFracs )
         : base( vendor )
      {
         this._startFieldType = startFieldType;
         this._startFieldPrecision = startFieldPrecision;
         this._endFieldType = endFieldType;
         this._secondFracs = secondFracs;
      }

      #region SQLDTInterval Members

      public IntervalDataTypes StartFieldType
      {
         get
         {
            return this._startFieldType;
         }
      }

      public Int32? StartFieldPrecision
      {
         get
         {
            return this._startFieldPrecision;
         }
      }

      public IntervalDataTypes? EndFieldType
      {
         get
         {
            return this._endFieldType;
         }
      }

      public Int32? SecondFracs
      {
         get
         {
            return this._secondFracs;
         }
      }

      #endregion
   }

   public class SQLDTTimeImpl : SQLDTAbstractTimeImpl, SQLDTTime
   {
      public SQLDTTimeImpl( SQLVendorImpl vendor, Int32? precision, Boolean? isWithTimeZone )
         : base( vendor, precision, isWithTimeZone )
      {
      }
   }

   public class SQLDTTimeStampImpl : SQLDTAbstractTimeImpl, SQLDTTimestamp
   {
      public SQLDTTimeStampImpl( SQLVendorImpl vendor, Int32? precision, Boolean? isWithTimeZone )
         : base( vendor, precision, isWithTimeZone )
      {
      }
   }

   public class SQLDTUserDefinedImpl : SQLElementBase, SQLDTUserDefined
   {
      private readonly String _textualRepresentation;

      public SQLDTUserDefinedImpl( SQLVendorImpl vendor, String textualRepresentation )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( textualRepresentation ), textualRepresentation );

         this._textualRepresentation = textualRepresentation;
      }

      #region SQLDTUserDefined Members

      public String TextualRepresentation
      {
         get
         {
            return this._textualRepresentation;
         }
      }

      #endregion
   }
}
