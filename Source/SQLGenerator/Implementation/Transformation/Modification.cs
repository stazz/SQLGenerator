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
   public abstract class DynamicColumnSourceProcessor<T> : AbstractProcessor<T>
      where T : class, DynamicColumnSource
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, T obj, StringBuilder builder )
      {
         if ( obj.ColumnNames != null )
         {
            aggregator.Process( obj.ColumnNames, builder );
         }
         this.DoProcessColumnSource( aggregator, obj, builder );
      }

      protected abstract void DoProcessColumnSource( SQLProcessorAggregator aggregator, T obj, StringBuilder builder );
   }

   public class ColumnSourceByQueryProcessor : DynamicColumnSourceProcessor<ColumnSourceByQuery>
   {
      protected override void DoProcessColumnSource( SQLProcessorAggregator aggregator, ColumnSourceByQuery obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.NEWLINE );
         aggregator.Process( obj.Query, builder );
      }
   }

   public class ColumnSourceByValuesProcessor : DynamicColumnSourceProcessor<ColumnSourceByValues>
   {
      protected override void DoProcessColumnSource( SQLProcessorAggregator aggregator, ColumnSourceByValues obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.NEWLINE );
         ProcessValues( aggregator, obj, builder );
      }

      public static void ProcessValues( SQLProcessorAggregator aggregator, AbstractValuesList values, StringBuilder builder )
      {
         builder.Append( "VALUES" );
         ProcessorUtils.AppendEnumerable( aggregator, builder, values.Values, SQLConstants.COMMA, ( agg, b, exp ) =>
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS );
            ProcessorUtils.AppendEnumerable( aggregator, builder, exp, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR, ( agg2, b2, exp2 ) =>
            {
               var needParenthesis = exp2 is QueryExpression;
               if ( needParenthesis )
               {
                  b2.Append( SQLConstants.OPEN_PARENTHESIS );
               }
               agg2.Process( exp2, b2 );
               if ( needParenthesis )
               {
                  b2.Append( SQLConstants.CLOSE_PARENTHESIS );
               }

            } );
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         } );
      }
   }

   public class DeleteBySearchProcessor : AbstractProcessor<DeleteBySearch>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, DeleteBySearch obj, StringBuilder builder )
      {
         builder.Append( "DELETE FROM " );
         aggregator.Process( obj.TargetTable, builder );
         ProcessorUtils.ProcessOptionalBooleanExpresssion( aggregator, builder, obj.Condition, SQLConstants.NEWLINE, SQLConstants.WHERE );
      }
   }

   public class InsertStatementProcessor : AbstractProcessor<InsertStatement>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, InsertStatement obj, StringBuilder builder )
      {
         builder.Append( "INSERT INTO " );
         aggregator.Process( obj.TableName, builder );
         builder.Append( SQLConstants.TOKEN_SEPARATOR );
         aggregator.Process( obj.ColumnSource, builder );
      }
   }

   public class SetClauseProcessor : AbstractProcessor<SetClause>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SetClause obj, StringBuilder builder )
      {
         builder.Append( obj.UpdateTarget ).Append( " = " );
         aggregator.Process( obj.UpdateSource, builder );
      }
   }

   public class TargetTableProcessor : AbstractProcessor<TargetTable>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, TargetTable obj, StringBuilder builder )
      {
         var isOnly = obj.IsOnly;
         if ( isOnly )
         {
            builder.Append( "ONLY" ).Append( SQLConstants.OPEN_PARENTHESIS );
         }
         aggregator.Process( obj.TableName, builder );
         if ( isOnly )
         {
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
      }
   }

   public class UpdateBySearchProcessor : AbstractProcessor<UpdateBySearch>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, UpdateBySearch obj, StringBuilder builder )
      {
         builder.Append( "UPDATE " );
         aggregator.Process( obj.TargetTable, builder );
         builder.Append( SQLConstants.NEWLINE ).Append( "SET " );
         ProcessorUtils.AppendEnumerable( aggregator, builder, obj.SetClauses, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR );
         ProcessorUtils.ProcessOptionalBooleanExpresssion( aggregator, builder, obj.Condition, SQLConstants.NEWLINE, SQLConstants.WHERE );
      }
   }

   public class UpdateSourceByExpressionProcessor : AbstractProcessor<UpdateSourceByExpression>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, UpdateSourceByExpression obj, StringBuilder builder )
      {
         aggregator.Process( obj.Expression, builder );
      }
   }
}
