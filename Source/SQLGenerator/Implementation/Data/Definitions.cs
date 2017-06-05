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
   public class SchemaDefinitionImpl : SQLElementBase, SchemaDefinition
   {
      private readonly String _schemaName;
      private readonly String _charset;
      private readonly ImmutableArray<SchemaElement> _elements;

      public SchemaDefinitionImpl( SQLVendorImpl vendor, String schemaName, String charSet, ImmutableArray<SchemaElement> elements )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( schemaName ), schemaName );
         ArgumentValidator.ValidateNotNull( nameof( elements ), elements );

         this._schemaName = schemaName;
         this._charset = charSet;
         this._elements = elements;
      }

      #region SchemaDefinition Members

      public String SchemaName
      {
         get
         {
            return this._schemaName;
         }
      }

      public String SchemaCharset
      {
         get
         {
            return this._charset;
         }
      }

      public ImmutableArray<SchemaElement> SchemaElements
      {
         get
         {
            return this._elements;
         }
      }

      #endregion
   }

   public class CheckConstraintImpl : SQLElementBase, CheckConstraint
   {
      private readonly BooleanExpression _condition;

      public CheckConstraintImpl( SQLVendorImpl vendor, BooleanExpression condition )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( condition ), condition );

         this._condition = condition;
      }
      #region CheckConstraint Members

      public BooleanExpression CheckCondition
      {
         get
         {
            return this._condition;
         }
      }

      #endregion
   }

   public class ColumnDefinitionImpl : SQLElementBase, ColumnDefinition
   {
      private readonly String _name;
      private readonly SQLDataType _dataType;
      private readonly String _default;
      private readonly Boolean _mayBeNull;
      private readonly AutoGenerationPolicy? _autoGenPolicy;

      public ColumnDefinitionImpl( SQLVendorImpl vendor, String name, SQLDataType dt, String defaultVal, Boolean mayBeNull, AutoGenerationPolicy? autoGenPolicy )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( name ), name );
         ArgumentValidator.ValidateNotNull( nameof( dt ), dt );

         this._name = name;
         this._dataType = dt;
         this._default = defaultVal;
         this._mayBeNull = mayBeNull;
         this._autoGenPolicy = autoGenPolicy;
      }

      #region ColumnDefinition Members

      public String ColumnName
      {
         get
         {
            return this._name;
         }
      }

      public SQLDataType DataType
      {
         get
         {
            return this._dataType;
         }
      }

      public String Default
      {
         get
         {
            return this._default;
         }
      }

      public Boolean MayBeNull
      {
         get
         {
            return this._mayBeNull;
         }
      }

      public AutoGenerationPolicy? AutoGenerationPolicy
      {
         get
         {
            return this._autoGenPolicy;
         }
      }

      #endregion
   }

   public class ForeignKeyConstraintImpl : SQLElementBase, ForeignKeyConstraint
   {

      private readonly ColumnNameList _sourceColumns;
      private readonly TableNameDirect _targeTableName;
      private readonly ColumnNameList _targetColumns;
      private readonly MatchType? _matchType;
      private readonly ReferentialAction? _onDelete;
      private readonly ReferentialAction? _onUpdate;

      public ForeignKeyConstraintImpl( SQLVendorImpl vendor, ColumnNameList sourceColumns, TableNameDirect targetTable, ColumnNameList targetColumns, MatchType? matchType, ReferentialAction? onDelete, ReferentialAction? onUpdate )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( sourceColumns ), sourceColumns );
         ArgumentValidator.ValidateNotNull( nameof( targetTable ), targetTable );

         this._sourceColumns = sourceColumns;
         this._targeTableName = targetTable;
         this._targetColumns = targetColumns;
         this._matchType = matchType;
         this._onDelete = onDelete;
         this._onUpdate = onUpdate;
      }

      #region ForeignKeyConstraint Members

      public ColumnNameList SourceColumns
      {
         get
         {
            return this._sourceColumns;
         }
      }

      public TableNameDirect TargetTable
      {
         get
         {
            return this._targeTableName;
         }
      }

      public ColumnNameList TargetColumns
      {
         get
         {
            return this._targetColumns;
         }
      }

      public MatchType? MatchType
      {
         get
         {
            return this._matchType;
         }
      }

      public ReferentialAction? OnUpdate
      {
         get
         {
            return this._onUpdate;
         }
      }

      public ReferentialAction? OnDelete
      {
         get
         {
            return this._onDelete;
         }
      }

      #endregion
   }

   public class LikeClauseImpl : SQLElementBase, LikeClause
   {
      private readonly TableNameDirect _tableName;

      public LikeClauseImpl( SQLVendorImpl vendor, TableNameDirect tableName )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( tableName ), tableName );

         this._tableName = tableName;
      }

      #region LikeClause Members

      public TableNameDirect TableName
      {
         get
         {
            return this._tableName;
         }
      }

      #endregion
   }

   public class TableConstraintDefinitionImpl : SQLElementBase, TableConstraintDefinition
   {
      private readonly String _name;
      private readonly ConstraintCharacteristics? _chars;
      private readonly TableConstraint _constraint;

      public TableConstraintDefinitionImpl( SQLVendorImpl vendor, String name, ConstraintCharacteristics? chars, TableConstraint constraint )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( constraint ), constraint );

         this._name = name;
         this._chars = chars;
         this._constraint = constraint;
      }

      #region TableConstraintDefinition Members

      public String ConstraintName
      {
         get
         {
            return this._name;
         }
      }

      public ConstraintCharacteristics? ConstraintCharacteristics
      {
         get
         {
            return this._chars;
         }
      }

      public TableConstraint Constraint
      {
         get
         {
            return this._constraint;
         }
      }

      #endregion
   }

   public class TableDefinitionImpl : SQLElementBase, TableDefinition
   {
      private readonly TableCommitAction? _commitAction;
      private readonly TableContentsSource _contents;
      private readonly TableNameDirect _name;
      private readonly TableScope? _scope;

      public TableDefinitionImpl( SQLVendorImpl vendor, TableCommitAction? commitAction, TableContentsSource contents, TableNameDirect name, TableScope? scope )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( name ), name );
         ArgumentValidator.ValidateNotNull( nameof( contents ), contents );

         this._commitAction = commitAction;
         this._contents = contents;
         this._name = name;
         this._scope = scope;
      }

      #region TableDefinition Members

      public TableScope? TableScope
      {
         get
         {
            return this._scope;
         }
      }

      public TableNameDirect TableName
      {
         get
         {
            return this._name;
         }
      }

      public TableCommitAction? CommitAction
      {
         get
         {
            return this._commitAction;
         }
      }

      public TableContentsSource Contents
      {
         get
         {
            return this._contents;
         }
      }

      #endregion
   }

   public class TableElementListImpl : SQLElementBase, TableElementList
   {
      private readonly ImmutableArray<TableElement> _elements;

      public TableElementListImpl( SQLVendorImpl vendor, ImmutableArray<TableElement> elements )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( elements ), elements );

         this._elements = elements;
      }

      #region TableElementList Members

      public ImmutableArray<TableElement> Elements
      {
         get
         {
            return this._elements;
         }
      }

      #endregion
   }

   public class UniqueConstraintImpl : SQLElementBase, UniqueConstraint
   {
      private readonly ColumnNameList _columns;
      private readonly UniqueSpecification _uniqueSpec;

      public UniqueConstraintImpl( SQLVendorImpl vendor, ColumnNameList columns, UniqueSpecification spec )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( columns ), columns );

         this._columns = columns;
         this._uniqueSpec = spec;
      }

      #region UniqueConstraint Members

      public UniqueSpecification UniquenessKind
      {
         get
         {
            return this._uniqueSpec;
         }
      }

      public ColumnNameList ColumnNames
      {
         get
         {
            return this._columns;
         }
      }

      #endregion
   }

   public class RegularViewSpecificationImpl : SQLElementBase, RegularViewSpecification
   {
      private readonly ColumnNameList _columns;

      public RegularViewSpecificationImpl( SQLVendorImpl vendor, ColumnNameList columns )
         : base( vendor )
      {
         this._columns = columns;
      }

      #region RegularViewSpecification Members

      public ColumnNameList Columns
      {
         get
         {
            return this._columns;
         }
      }

      #endregion
   }

   public class ViewDefinitionImpl : SQLElementBase, ViewDefinition
   {
      private readonly TableNameDirect _name;
      private readonly QueryExpression _query;
      private readonly ViewSpecification _spec;
      private readonly ViewCheckOption? _viewCheck;
      private readonly Boolean _isRecursive;

      public ViewDefinitionImpl( SQLVendorImpl vendor, TableNameDirect name, QueryExpression query, ViewSpecification spec, ViewCheckOption? check, Boolean isRecursive )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( name ), name );
         ArgumentValidator.ValidateNotNull( nameof( query ), query );
         ArgumentValidator.ValidateNotNull( nameof( spec ), spec );

         this._name = name;
         this._query = query;
         this._spec = spec;
         this._viewCheck = check;
         this._isRecursive = isRecursive;
      }

      #region ViewDefinition Members

      public Boolean IsRecursive
      {
         get
         {
            return this._isRecursive;
         }
      }

      public TableNameDirect ViewName
      {
         get
         {
            return this._name;
         }
      }

      public ViewCheckOption? ViewCheckOption
      {
         get
         {
            return this._viewCheck;
         }
      }

      public ViewSpecification ViewSpecification
      {
         get
         {
            return this._spec;
         }
      }

      public QueryExpression ViewQuery
      {
         get
         {
            return this._query;
         }
      }

      #endregion
   }
}
