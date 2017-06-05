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
using SQLGenerator.Implementation;
using SQLGenerator;
using UtilPack;

namespace SQLGenerator
{
   public interface ValueExpression : SQLElement
   {
   }

   public interface NonBooleanExpression : ValueExpression
   {
   }

   public interface ArithmeticExpression : NonBooleanExpression
   {
      ArithmeticOperator Operator { get; }
   }

   public interface BinaryArithmeticExpression : ArithmeticExpression
   {
      NonBooleanExpression Left { get; }
      NonBooleanExpression Right { get; }
   }

   public interface UnaryArithmeticExpression : ArithmeticExpression
   {
      NonBooleanExpression Expression { get; }
   }

   public enum ArithmeticOperator { Plus, Minus, Multiplication, Division, MaxValue }

   public interface SQLStatement : SQLElement
   {

   }

   public interface SchemaStatement : SQLStatement
   {

   }

   public interface TableName : SQLElement
   {
      [OptionalSQLElement]
      String SchemaName { get; }
   }

   public interface TableNameDirect : TableName
   {
      String TableName { get; }
   }

   public interface TableNameFunction : TableName
   {
      SQLFunctionLiteral Function { get; }
   }

   public interface ColumnNameList : SQLElement
   {
      ImmutableArray<String> ColumnNames { get; }
   }

   public interface ObjectWithVendor
   {
      SQLVendor SQLVendor { get; }
   }

   public interface SQLElement : ObjectWithVendor
   {
      // TODO Boolean ContentsEqual(Object other);
   }

   public sealed class ImmutableArray<T> : IEnumerable<T>
   {
      private readonly T[] _array;

      public ImmutableArray( T[] array, Boolean copy = false )
      {
         ArgumentValidator.ValidateNotNull( nameof( array ), array );

         if ( copy )
         {
            this._array = new T[array.Length];
            Array.Copy( array, this._array, array.Length );
         }
         else
         {
            this._array = array;
         }
      }

      public T this[Int32 idx]
      {
         get
         {
            return this._array[idx];
         }
      }

      public Int32 Length
      {
         get
         {
            return this._array.Length;
         }
      }

      #region IEnumerable<T> Members

      public IEnumerator<T> GetEnumerator()
      {
         return ( (IEnumerable<T>) this._array ).GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return this._array.GetEnumerator();
      }

      #endregion

      internal T[] ArrayObject
      {
         get
         {
            return this._array;
         }
      }

      public void ForEach( Action<Int32, T> action )
      {
         for ( var i = 0; i < this._array.Length; ++i )
         {
            action( i, this._array[i] );
         }
      }
   }

   [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
   public sealed class OptionalSQLElementAttribute : Attribute
   {

   }

   public static class ArrayQueryHelper
   {
      public static ImmutableArray<T> NewAQ<T>( this T[] array, Boolean copy = false )
      {
         return new ImmutableArray<T>( array, copy );
      }

      public static ImmutableArray<T> NewAQ<T>( this List<T> list )
      {
         return NewAQ( list.ToArray(), false );
      }

      public static ImmutableArray<T> NewAQ<T>( this IEnumerable<T> enumerable )
      {
         return NewAQ( enumerable.ToArray(), false );
      }

      public static ImmutableArray<T> NewAQ<T>( params T[] array )
      {
         return NewAQ( array, false );
      }

      public static void ValidateNotEmpty<T>( this ImmutableArray<T> array, String name )
      {
         ArgumentValidator.ValidateNotNull( nameof( array ), array );
         if ( array.Length <= 0 )
         {
            throw new ArgumentException( name + " was empty." );
         }
      }
   }

   public static class SQLFunctions
   {
      public const String AVG = "AVG";
      public const String MAX = "MAX";
      public const String MIN = "MIN";
      public const String SUM = "SUM";
      public const String EVERY = "EVERY";
      public const String ANY = "ANY";
      public const String COUNT = "COUNT";
   }
}

public static partial class E_SQLGenerator
{
   public static TableReferenceByName AsReference( this TableNameDirect tableName, TableAlias alias = null )
   {
      return tableName.SQLVendor.QueryFactory.NewTableReferenceByName( tableName, alias );
   }

   public static TableReferenceByName AsReference( this TableNameDirect tableName, String alias, params String[] renamedColumns )
   {
      return tableName.SQLVendor.QueryFactory.NewTableReferenceByName( tableName, tableName.SQLVendor.QueryFactory.NewTableAlias( alias, renamedColumns ) );
   }
}
