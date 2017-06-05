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
   public class AddColumnDefinitionImpl : SQLElementBase, AddColumnDefinition
   {
      private readonly ColumnDefinition _columnDef;

      public AddColumnDefinitionImpl( SQLVendorImpl vendor, ColumnDefinition columnDef )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( columnDef ), columnDef );

         this._columnDef = columnDef;
      }

      #region AddColumnDefinition Members

      public ColumnDefinition ColumnDefinition
      {
         get
         {
            return this._columnDef;
         }
      }

      #endregion
   }

   public class AddTableConstraintDefinitionImpl : SQLElementBase, AddTableConstraintDefinition
   {
      private readonly TableConstraintDefinition _constraint;

      public AddTableConstraintDefinitionImpl( SQLVendorImpl vendor, TableConstraintDefinition constraint )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( constraint ), constraint );

         this._constraint = constraint;
      }

      #region AddTableConstraintDefinition Members

      public TableConstraintDefinition ConstraintDefinition
      {
         get
         {
            return this._constraint;
         }
      }

      #endregion
   }

   public class AlterColumnDefinitionImpl : SQLElementBase, AlterColumnDefinition
   {
      private readonly String _columnName;
      private readonly AlterColumnAction _alterAction;

      public AlterColumnDefinitionImpl( SQLVendorImpl vendor, String colName, AlterColumnAction action )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( colName ), colName );
         ArgumentValidator.ValidateNotNull( nameof( action ), action );

         this._columnName = colName;
         this._alterAction = action;
      }

      #region AlterColumnDefinition Members

      public String ColumnName
      {
         get
         {
            return this._columnName;
         }
      }

      public AlterColumnAction AlterAction
      {
         get
         {
            return this._alterAction;
         }
      }

      #endregion
   }

   public class AlterTableStatementImpl : SQLElementBase, AlterTableStatement
   {

      private readonly TableNameDirect _tableName;
      private readonly AlterTableAction _action;

      public AlterTableStatementImpl( SQLVendorImpl vendor, TableNameDirect tableName, AlterTableAction action )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( tableName ), tableName );
         ArgumentValidator.ValidateNotNull( nameof( action ), action );

         this._tableName = tableName;
         this._action = action;
      }

      #region AlterTableStatement Members

      public TableNameDirect TableName
      {
         get
         {
            return this._tableName;
         }
      }

      public AlterTableAction AlterAction
      {
         get
         {
            return this._action;
         }
      }

      #endregion
   }

   public abstract class DropBehaviourContainerImpl : SQLElementBase, DropBehaviourContainer
   {
      private readonly DropBehaviour _dropBehaviour;

      protected DropBehaviourContainerImpl( SQLVendorImpl vendor, DropBehaviour db )
         : base( vendor )
      {
         this._dropBehaviour = db;
      }

      #region DropBehaviourContainer Members

      public DropBehaviour DropBehaviour
      {
         get
         {
            return this._dropBehaviour;
         }
      }

      #endregion
   }

   public class DropColumnDefinitionImpl : DropBehaviourContainerImpl, DropColumnDefinition
   {
      private readonly String _colName;

      public DropColumnDefinitionImpl( SQLVendorImpl vendor, DropBehaviour db, String colName )
         : base( vendor, db )
      {
         ArgumentValidator.ValidateNotNull( nameof( colName ), colName );

         this._colName = colName;
      }

      #region DropColumnDefinition Members

      public String ColumnName
      {
         get
         {
            return this._colName;
         }
      }

      #endregion
   }

   public class DropSchemaStatementImpl : DropBehaviourContainerImpl, DropSchemaStatement
   {
      private readonly String _name;

      public DropSchemaStatementImpl( SQLVendorImpl vendor, DropBehaviour db, String schemaName )
         : base( vendor, db )
      {
         ArgumentValidator.ValidateNotNull( nameof( schemaName ), schemaName );

         this._name = schemaName;
      }

      #region DropSchemaStatement Members

      public String SchemaName
      {
         get
         {
            return this._name;
         }
      }

      #endregion
   }

   public class DropTableConstraintDefinitionImpl : DropBehaviourContainerImpl, DropTableConstraintDefinition
   {
      private readonly String _constraintName;

      public DropTableConstraintDefinitionImpl( SQLVendorImpl vendor, DropBehaviour db, String constraintName )
         : base( vendor, db )
      {
         ArgumentValidator.ValidateNotNull( nameof( constraintName ), constraintName );

         this._constraintName = constraintName;
      }

      #region DropTableConstraintDefinition Members

      public String ConstraintName
      {
         get
         {
            return this._constraintName;
         }
      }

      #endregion
   }

   public class DropTableOrViewStatementImpl : DropBehaviourContainerImpl, DropTableOrViewStatement
   {
      private readonly TableNameDirect _tableName;
      private readonly ObjectType _whatToDrop;

      public DropTableOrViewStatementImpl( SQLVendorImpl vendor, DropBehaviour db, ObjectType whatToDrop, TableNameDirect table )
         : base( vendor, db )
      {
         ArgumentValidator.ValidateNotNull( nameof( table ), table );

         this._whatToDrop = whatToDrop;
         this._tableName = table;
      }

      #region DropTableOrViewStatement Members

      public TableNameDirect TableName
      {
         get
         {
            return this._tableName;
         }
      }


      public ObjectType WhatToDrop
      {
         get
         {
            return this._whatToDrop;
         }
      }

      #endregion
   }

   public class SetColumnDefaultImpl : SQLElementBase, SetColumnDefault
   {
      private readonly String _default;

      public SetColumnDefaultImpl( SQLVendorImpl vendor, String defValue )
         : base( vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( defValue ), defValue );

         this._default = defValue;
      }
      #region SetColumnDefault Members

      public String DefaultValue
      {
         get
         {
            return this._default;
         }
      }

      #endregion
   }
}
