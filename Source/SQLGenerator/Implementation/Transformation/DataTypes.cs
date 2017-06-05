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
   public class SQLDTDecimalProcessor : AbstractProcessor<SQLDTDecimal>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SQLDTDecimal obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.DECIMAL );
         if ( obj.Precision.HasValue )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS ).Append( obj.Precision.Value );
            if ( obj.Scale.HasValue )
            {
               builder.Append( SQLConstants.COMMA ).Append( SQLConstants.TOKEN_SEPARATOR ).Append( obj.Scale.Value );
            }
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
      }
   }

   public class SQLDTNumericProcessor : AbstractProcessor<SQLDTNumeric>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SQLDTNumeric obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.NUMERIC );
         if ( obj.Precision.HasValue )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS ).Append( obj.Precision.Value );
            if ( obj.Scale.HasValue )
            {
               builder.Append( SQLConstants.COMMA ).Append( SQLConstants.TOKEN_SEPARATOR ).Append( obj.Scale.Value );
            }
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
      }
   }

   public class SQLDTCharProcessor : AbstractProcessor<SQLDTChar>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SQLDTChar obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.CHARACTER );
         if ( obj.IsVarying )
         {
            builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( SQLConstants.VARYING );
         }
         if ( obj.Length.HasValue )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS ).Append( obj.Length.Value ).Append( SQLConstants.CLOSE_PARENTHESIS );
         }
      }
   }

   public class SQLDTFloatProcessor : AbstractProcessor<SQLDTFloat>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SQLDTFloat obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.FLOAT );
         if ( obj.Precision.HasValue )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS ).Append( obj.Precision.Value ).Append( SQLConstants.CLOSE_PARENTHESIS );
         }
      }
   }

   public class SQLDTIntervalProcessor : AbstractProcessor<SQLDTInterval>
   {
      private static readonly IDictionary<IntervalDataTypes, String> DEFAULT_INTERVAL_DATA_TYPES = new Dictionary<IntervalDataTypes, String>
      {
         { IntervalDataTypes.Year, "YEAR" },
         { IntervalDataTypes.Month, "MONTH" },
         { IntervalDataTypes.Day, "DAY" },
         { IntervalDataTypes.Hour, "HOUR" },
         { IntervalDataTypes.Minute, "MINUTE" },
         { IntervalDataTypes.Second, "SECOND" }
      };

      private readonly IDictionary<IntervalDataTypes, String> _intervalDataTypes;

      public SQLDTIntervalProcessor( IDictionary<IntervalDataTypes, String> intervalDataTypes = null )
      {
         this._intervalDataTypes = intervalDataTypes ?? DEFAULT_INTERVAL_DATA_TYPES;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, SQLDTInterval obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.INTERVAL ).Append( SQLConstants.TOKEN_SEPARATOR ).Append( this._intervalDataTypes[obj.StartFieldType] );
         if ( obj.StartFieldPrecision.HasValue )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS ).Append( obj.StartFieldPrecision.Value );
            if ( !obj.EndFieldType.HasValue && obj.SecondFracs.HasValue )
            {
               builder.Append( SQLConstants.COMMA ).Append( SQLConstants.TOKEN_SEPARATOR ).Append( obj.SecondFracs.Value );
            }
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }

         if ( obj.EndFieldType.HasValue )
         {
            builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( SQLConstants.TO ).Append( SQLConstants.TOKEN_SEPARATOR ).Append( this._intervalDataTypes[obj.EndFieldType.Value] );
            if ( obj.SecondFracs.HasValue )
            {
               builder.Append( SQLConstants.OPEN_PARENTHESIS ).Append( obj.SecondFracs.Value ).Append( SQLConstants.CLOSE_PARENTHESIS );
            }
         }
      }
   }

   public class SQLDTGenericTimeProcessor<T> : AbstractProcessor<T>
      where T : class, SQLDTAbstractTime
   {
      private readonly String _dataType;

      public SQLDTGenericTimeProcessor( String dt )
      {
         ArgumentValidator.ValidateNotNull( nameof( dt ), dt );

         this._dataType = dt;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, T obj, StringBuilder builder )
      {
         builder.Append( this._dataType );
         if ( obj.Precision.HasValue )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS ).Append( obj.Precision.Value ).Append( SQLConstants.CLOSE_PARENTHESIS );
         }

         if ( obj.IsWithTimeZone.HasValue )
         {
            builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( obj.IsWithTimeZone.Value ? SQLConstants.WITH : SQLConstants.WITHOUT ).Append( SQLConstants.TIMEZONE );
         }
      }
   }

   public class SQLDTUserDefinedProcessor : AbstractProcessor<SQLDTUserDefined>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SQLDTUserDefined obj, StringBuilder builder )
      {
         builder.Append( obj.TextualRepresentation );
      }
   }
}
