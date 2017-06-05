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
   public class StringLiteralExpressionProcessor : AbstractProcessor<StringLiteral>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, StringLiteral obj, StringBuilder builder )
      {
         builder.Append( obj.String == null ? SQLConstants.NULL : ( "'" + obj.String + "'" ) );
      }
   }

   public class DirectLiteralProcessor : AbstractProcessor<DirectLiteral>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, DirectLiteral obj, StringBuilder builder )
      {
         builder.Append( obj.TextContents == null ? SQLConstants.NULL : obj.TextContents );
      }
   }

   public class TimestampLiteralProcessor : AbstractProcessor<TimestampLiteral>
   {
      private const String DEFAULT_FORMAT = "yyyy-MM-dd HH:mm:ss.FFF";

      private readonly String _format;

      public TimestampLiteralProcessor( String format = null )
      {
         this._format = "{0:" + ( format ?? DEFAULT_FORMAT ) + "}";
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, TimestampLiteral obj, StringBuilder builder )
      {
         if ( obj.Timestamp.HasValue )
         {
            this.DoProcessNonNull( aggregator, obj, builder );
         }
         else
         {
            builder.Append( SQLConstants.NULL );
         }
      }

      protected virtual void DoProcessNonNull( SQLProcessorAggregator aggregator, TimestampLiteral obj, StringBuilder builder )
      {
         builder.Append( "'" ).AppendFormat( this._format, obj.Timestamp.Value ).Append( "'" );
      }
   }

   public class SQLFunctionLiteralProcessor : AbstractProcessor<SQLFunctionLiteral>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SQLFunctionLiteral obj, StringBuilder builder )
      {
         builder.Append( obj.FunctionName ).Append( SQLConstants.OPEN_PARENTHESIS );
         ProcessorUtils.AppendEnumerable( aggregator, builder, obj.Parameters, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR );
         builder.Append( SQLConstants.CLOSE_PARENTHESIS );
      }
   }

   public class NumericLiteralProcessor<TLiteral, TNumber> : AbstractProcessor<TLiteral>
      where TLiteral : class, NumericLiteral<TNumber>
      where TNumber : struct
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, TLiteral obj, StringBuilder builder )
      {
         builder.Append( obj.Number.HasValue ? obj.Number.Value.ToString() : SQLConstants.NULL );
      }
   }
}
