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
   public abstract class AbstractArithmeticExpressionProcessor<T> : AbstractProcessor<T>
      where T : class, ArithmeticExpression
   {
      private static readonly IDictionary<ArithmeticOperator, String> OPERATORS = new Dictionary<ArithmeticOperator, String>
      {
         { ArithmeticOperator.Plus, "+" },
         { ArithmeticOperator.Minus, "-" },
         { ArithmeticOperator.Multiplication, "*" },
         { ArithmeticOperator.Division, "/" }
      };

      private IDictionary<ArithmeticOperator, String> _operators;

      protected AbstractArithmeticExpressionProcessor( IDictionary<ArithmeticOperator, String> operators = null )
      {
         this._operators = operators ?? OPERATORS;
      }

      protected void ProcessArithmeticExpression( T expression, StringBuilder builder )
      {
         builder.Append( this._operators[expression.Operator] );
      }
   }

   public class BinaryArithmeticExpressionProcessor : AbstractArithmeticExpressionProcessor<BinaryArithmeticExpression>
   {
      public BinaryArithmeticExpressionProcessor( IDictionary<ArithmeticOperator, String> operators = null )
         : base( operators )
      {

      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, BinaryArithmeticExpression obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.OPEN_PARENTHESIS );

         var currentIsQuery = ExpressionIsQuery( obj.Left );
         if ( currentIsQuery )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS );
         }
         aggregator.Process( obj.Left, builder );
         if ( currentIsQuery )
         {
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
         builder.Append( SQLConstants.TOKEN_SEPARATOR );
         this.ProcessArithmeticExpression( obj, builder );
         builder.Append( SQLConstants.TOKEN_SEPARATOR );

         currentIsQuery = ExpressionIsQuery( obj.Right );
         if ( currentIsQuery )
         {
            builder.Append( SQLConstants.OPEN_PARENTHESIS );
         }
         aggregator.Process( obj.Right, builder );
         if ( currentIsQuery )
         {
            builder.Append( SQLConstants.CLOSE_PARENTHESIS );
         }
         builder.Append( SQLConstants.CLOSE_PARENTHESIS );
      }

      public static Boolean ExpressionIsQuery( ValueExpression expression )
      {
         return expression is QueryExpressionBody || expression is QueryExpression;
      }
   }

   public class UnaryArithmeticExpressionProcessor : AbstractArithmeticExpressionProcessor<UnaryArithmeticExpression>
   {
      public UnaryArithmeticExpressionProcessor( IDictionary<ArithmeticOperator, String> operators = null )
         : base( operators )
      {

      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, UnaryArithmeticExpression obj, StringBuilder builder )
      {
         this.ProcessArithmeticExpression( obj, builder );
         builder.Append( SQLConstants.OPEN_PARENTHESIS );
         aggregator.Process( obj.Expression, builder );
         builder.Append( SQLConstants.CLOSE_PARENTHESIS );
      }
   }

   public class ColumnNamesProcessor : AbstractProcessor<ColumnNameList>
   {

      protected override void DoProcess( SQLProcessorAggregator aggregator, ColumnNameList obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.OPEN_PARENTHESIS );
         ProcessorUtils.AppendEnumerable( aggregator, builder, obj.ColumnNames, SQLConstants.COMMA + SQLConstants.TOKEN_SEPARATOR, ( agg, b, str ) => b.Append( str ) );
         builder.Append( SQLConstants.CLOSE_PARENTHESIS );
      }
   }

   public abstract class AbstractTableNameProcessor<TName> : AbstractProcessor<TName>
      where TName : class, TableName
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, TName obj, StringBuilder builder )
      {
         if ( !String.IsNullOrEmpty( obj.SchemaName ) )
         {
            builder.Append( obj.SchemaName ).Append( SQLConstants.SCHEMA_TABLE_SEPARATOR );
         }
         this.DoProcessTableName( aggregator, obj, builder );
      }

      protected abstract void DoProcessTableName( SQLProcessorAggregator aggregator, TName obj, StringBuilder builder );
   }

   public class TableNameFunctionProcessor : AbstractTableNameProcessor<TableNameFunction>
   {

      protected override void DoProcessTableName( SQLProcessorAggregator aggregator, TableNameFunction obj, StringBuilder builder )
      {
         aggregator.Process( obj.Function, builder );
      }
   }

   public class TableNameDirectProcessor : AbstractTableNameProcessor<TableNameDirect>
   {

      protected override void DoProcessTableName( SQLProcessorAggregator aggregator, TableNameDirect obj, StringBuilder builder )
      {
         builder.Append( obj.TableName );
      }
   }

   public class ConstantProcessor : SQLProcessor
   {
      private readonly String _constant;

      public ConstantProcessor( String constant )
      {
         this._constant = constant ?? "";
      }

      #region SQLProcessor Members

      public void Process( SQLProcessorAggregator aggregator, Object obj, StringBuilder builder, Boolean negationActive )
      {
         builder.Append( this._constant );
      }

      #endregion
   }

   public class NoOpProcessor : SQLProcessor
   {
      #region SQLProcessor Members

      public void Process( SQLProcessorAggregator aggregator, object obj, StringBuilder builder, Boolean negationActive )
      {
         // Do nothing
      }

      #endregion
   }

   //internal static class DictionaryHelper
   //{
   //   internal static IDictionary<U, V> CreateMappingDictionary<U, V>( params Tuple<U, V>[] contents )
   //   {
   //      var dic = new Dictionary<U, V>( contents.Length );
   //      foreach ( var tuple in contents )
   //      {
   //         dic.Add( tuple.Item1, tuple.Item2 );
   //      }
   //      return dic;
   //   }
   //}
}
