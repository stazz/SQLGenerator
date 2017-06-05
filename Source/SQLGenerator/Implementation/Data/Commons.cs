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
   public abstract class SQLElementBase : SQLElement
   {
      private readonly SQLVendorImpl _vendor;

      protected SQLElementBase( SQLVendorImpl vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( vendor ), vendor );

         this._vendor = vendor;
      }

      public override String ToString()
      {
         return this._vendor.ToString( this );
      }

      #region ObjectWithVendor Members

      public SQLVendor SQLVendor
      {
         get
         {
            return this._vendor;
         }
      }

      #endregion
   }

   public abstract class ArithmeticExpressionImpl : SQLElementBase, ArithmeticExpression
   {
      private readonly ArithmeticOperator _op;

      protected ArithmeticExpressionImpl( SQLVendorImpl vendor, ArithmeticOperator op )
         : base( vendor )
      {
         this._op = op;
      }

      #region ArithmeticExpression Members

      public ArithmeticOperator Operator
      {
         get
         {
            return this._op;
         }
      }

      #endregion
   }

   public class BinaryArithmeticExpressionImpl : ArithmeticExpressionImpl, BinaryArithmeticExpression
   {

      private readonly NonBooleanExpression _left;
      private readonly NonBooleanExpression _right;

      public BinaryArithmeticExpressionImpl( SQLVendorImpl vendor, ArithmeticOperator op, NonBooleanExpression left, NonBooleanExpression right )
         : base( vendor, op )
      {
         ArgumentValidator.ValidateNotNull( nameof( left ), left );
         ArgumentValidator.ValidateNotNull( nameof( right ), right );

         this._left = left;
         this._right = right;
      }

      #region BinaryArithmeticExpression Members

      public NonBooleanExpression Left
      {
         get
         {
            return this._left;
         }
      }

      public NonBooleanExpression Right
      {
         get
         {
            return this._right;
         }
      }

      #endregion
   }

   public class UnaryArithmeticExpressionImpl : ArithmeticExpressionImpl, UnaryArithmeticExpression
   {
      private readonly NonBooleanExpression _expr;

      public UnaryArithmeticExpressionImpl( SQLVendorImpl vendor, ArithmeticOperator op, NonBooleanExpression expression )
         : base( vendor, op )
      {
         ArgumentValidator.ValidateNotNull( nameof( expression ), expression );
         if ( ArithmeticOperator.Minus != op || ArithmeticOperator.Plus != op )
         {
            throw new ArgumentException( "Unary arithmetic expression operator must be either plus or minus." );
         }

         this._expr = expression;
      }

      #region UnaryArithmeticExpression Members

      public NonBooleanExpression Expression
      {
         get
         {
            return this._expr;
         }
      }

      #endregion
   }

   public class ColumnNameListImpl : SQLElementBase, ColumnNameList
   {
      private readonly ImmutableArray<String> _names;
      private readonly Lazy<Int32> _hashCode;

      public ColumnNameListImpl( SQLVendorImpl vendor, ImmutableArray<String> columnNames )
         : base( vendor )
      {
         columnNames.ValidateNotEmpty( nameof( columnNames ) );
         foreach ( var columnName in columnNames )
         {
            ArgumentValidator.ValidateNotNull( nameof( columnName ), columnName );
         }

         this._names = columnNames;
         this._hashCode = new Lazy<Int32>( () => this._names.Aggregate( 0, ( cur, name ) => cur + name.GetHashCode() ), LazyThreadSafetyMode.ExecutionAndPublication );
      }

      #region ColumnNameList Members

      public ImmutableArray<String> ColumnNames
      {
         get
         {
            return this._names;
         }
      }

      #endregion

      public override Boolean Equals( Object obj )
      {
         return Object.ReferenceEquals( this, obj ) || ( obj is ColumnNameList && this._names.SequenceEqual( ( (ColumnNameList) obj ).ColumnNames ) );
      }

      public override Int32 GetHashCode()
      {
         return this._hashCode.Value;
      }
   }

   public abstract class TableNameImpl : SQLElementBase, TableName
   {
      private readonly String _schemaName;

      protected TableNameImpl( SQLVendorImpl vendor, String schemaName )
         : base( vendor )
      {
         this._schemaName = schemaName;
      }

      #region TableName Members

      public String SchemaName
      {
         get
         {
            return this._schemaName;
         }
      }

      #endregion

      public override abstract Boolean Equals( Object obj );
      public override abstract Int32 GetHashCode();

      protected Boolean DoesEqual( TableName obj )
      {
         return Object.Equals( this._schemaName, obj.SchemaName );
      }
   }

   public class TableNameDirectImpl : TableNameImpl, TableNameDirect
   {
      private readonly String _tableName;

      public TableNameDirectImpl( SQLVendorImpl vendor, String schemaName, String tableName )
         : base( vendor, schemaName )
      {
         ArgumentValidator.ValidateNotNull( nameof( tableName ), tableName );

         this._tableName = tableName;
      }

      public override Boolean Equals( Object obj )
      {
         return Object.ReferenceEquals( this, obj ) || ( obj is TableNameDirect && this.DoesEqual( obj as TableName ) && this._tableName.Equals( ( (TableNameDirect) obj ).TableName ) );
      }

      public override Int32 GetHashCode()
      {
         return this._tableName.GetHashCode();
      }

      #region TableNameDirect Members

      public String TableName
      {
         get
         {
            return this._tableName;
         }
      }

      #endregion
   }

   public class TableNameFunctionImpl : TableNameImpl, TableNameFunction
   {
      private readonly SQLFunctionLiteral _function;

      public TableNameFunctionImpl( SQLVendorImpl vendor, String schemaName, SQLFunctionLiteral sqlFunction )
         : base( vendor, schemaName )
      {
         ArgumentValidator.ValidateNotNull( nameof( sqlFunction ), sqlFunction );

         this._function = sqlFunction;
      }

      public override Boolean Equals( Object obj )
      {
         return Object.ReferenceEquals( this, obj ) || ( obj is TableNameFunction && this.DoesEqual( obj as TableName ) && this._function.Equals( ( (TableNameFunction) obj ).Function ) );
      }

      public override Int32 GetHashCode()
      {
         return this._function.GetHashCode();
      }

      #region TableNameFunction Members

      public SQLFunctionLiteral Function
      {
         get
         {
            return this._function;
         }
      }

      #endregion
   }
}
