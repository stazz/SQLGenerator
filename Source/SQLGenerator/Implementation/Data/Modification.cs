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

   public abstract class DynamicColumnSourceImpl : SQLElementBase, DynamicColumnSource
   {
      private readonly ColumnNameList _columns;

      protected DynamicColumnSourceImpl( SQLVendorImpl vendor, ColumnNameList columns )
         : base( vendor )
      {
         this._columns = columns;
      }

      #region DynamicColumnSource Members

      public ColumnNameList ColumnNames
      {
         get
         {
            return this._columns;
         }
      }

      #endregion
   }

   public class ColumnSourceByQueryImpl : DynamicColumnSourceImpl, ColumnSourceByQuery
   {
      private readonly QueryExpression _query;

      public ColumnSourceByQueryImpl( SQLVendorImpl vendor, ColumnNameList columns, QueryExpression query )
         : base( vendor, columns )
      {
         ArgumentValidator.ValidateNotNull( nameof( query ), query );

         this._query = query;
      }
      #region ColumnSourceByQuery Members

      public QueryExpression Query
      {
         get
         {
            return this._query;
         }
      }

      #endregion
   }

   public class ColumnSourceByValuesImpl : DynamicColumnSourceImpl, ColumnSourceByValues
   {
      private readonly ImmutableArray<ImmutableArray<ValueExpression>> _values;

      public ColumnSourceByValuesImpl( SQLVendorImpl vendor, ColumnNameList columns, ImmutableArray<ImmutableArray<ValueExpression>> values )
         : base( vendor, columns )
      {
         values.ValidateNotEmpty( nameof( values ) );

         this._values = values;
      }

      #region ColumnSourceByValues Members

      public ImmutableArray<ImmutableArray<ValueExpression>> Values
      {
         get
         {
            return this._values;
         }
      }

      #endregion
   }

   public class DeleteBySearchImpl : SQLElementBase, DeleteBySearch
   {
      private readonly TargetTable _targetTable;
      private readonly BooleanExpression _condition;

      public DeleteBySearchImpl( SQLVendorImpl vendor, TargetTable targetTable, BooleanExpression condition )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( targetTable ), targetTable );

         this._targetTable = targetTable;
         this._condition = condition;
      }

      #region DeleteBySearch Members

      public TargetTable TargetTable
      {
         get
         {
            return this._targetTable;
         }
      }

      public BooleanExpression Condition
      {
         get
         {
            return this._condition;
         }
      }

      #endregion
   }

   public class InsertStatementImpl : SQLElementBase, InsertStatement
   {
      private readonly TableNameDirect _table;
      private readonly ColumnSource _columnSource;

      public InsertStatementImpl( SQLVendorImpl vendor, TableNameDirect table, ColumnSource columnSource )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( table ), table );
         ArgumentValidator.ValidateNotNull( nameof( columnSource ), columnSource );

         this._table = table;
         this._columnSource = columnSource;
      }

      #region InsertStatement Members

      public TableNameDirect TableName
      {
         get
         {
            return this._table;
         }
      }

      public ColumnSource ColumnSource
      {
         get
         {
            return this._columnSource;
         }
      }

      #endregion
   }

   public class SetClauseImpl : SQLElementBase, SetClause
   {
      private readonly String _target;
      private readonly UpdateSource _source;

      public SetClauseImpl( SQLVendorImpl vendor, String target, UpdateSource source )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( target ), target );
         ArgumentValidator.ValidateNotNull( nameof( source ), source );

         this._target = target;
         this._source = source;
      }

      #region SetClause Members

      public String UpdateTarget
      {
         get
         {
            return this._target;
         }
      }

      public UpdateSource UpdateSource
      {
         get
         {
            return this._source;
         }
      }

      #endregion
   }

   public class TargetTableImpl : SQLElementBase, TargetTable
   {
      private readonly TableNameDirect _table;
      private readonly Boolean _isOnly;

      public TargetTableImpl( SQLVendorImpl vendor, TableNameDirect table, Boolean isOnly )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( table ), table );

         this._table = table;
         this._isOnly = isOnly;
      }

      #region TargetTable Members

      public Boolean IsOnly
      {
         get
         {
            return this._isOnly;
         }
      }

      public TableNameDirect TableName
      {
         get
         {
            return this._table;
         }
      }

      #endregion
   }

   public class UpdateBySearchImpl : SQLElementBase, UpdateBySearch
   {
      private readonly TargetTable _targetTable;
      private readonly BooleanExpression _condition;
      private readonly ImmutableArray<SetClause> _setClauses;

      public UpdateBySearchImpl( SQLVendorImpl vendor, TargetTable targetTable, BooleanExpression condition, ImmutableArray<SetClause> setClauses )
         : base( vendor )
      {
         setClauses.ValidateNotEmpty( nameof( setClauses ) );
         foreach ( var clause in setClauses )
         {
            ArgumentValidator.ValidateNotNull( nameof( clause ), clause );
         }

         this._targetTable = targetTable;
         this._condition = condition;
         this._setClauses = setClauses;
      }

      #region UpdateBySearch Members

      public TargetTable TargetTable
      {
         get
         {
            return this._targetTable;
         }
      }

      public BooleanExpression Condition
      {
         get
         {
            return this._condition;
         }
      }

      public ImmutableArray<SetClause> SetClauses
      {
         get
         {
            return this._setClauses;
         }
      }

      #endregion
   }

   public class UpdateSourceByExpressionImpl : SQLElementBase, UpdateSourceByExpression
   {
      private readonly ValueExpression _expression;

      public UpdateSourceByExpressionImpl( SQLVendorImpl vendor, ValueExpression expression )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( expression ), expression );

         this._expression = expression;
      }
      #region UpdateSourceByExpression Members

      public ValueExpression Expression
      {
         get
         {
            return this._expression;
         }
      }

      #endregion
   }

   public static class ColumnSources
   {
      public class Defaults : SQLElementBase, ColumnSource
      {
         public Defaults( SQLVendorImpl vendor )
            : base( vendor )
         {
         }
      }
   }

   public static class ValueSources
   {
      public class Default : SQLElementBase, ValueExpression
      {
         public Default( SQLVendorImpl vendor )
            : base( vendor )
         {
         }
      }
      public class Null : SQLElementBase, ValueExpression
      {
         public Null( SQLVendorImpl vendor )
            : base( vendor )
         {
         }
      }
   }
}
