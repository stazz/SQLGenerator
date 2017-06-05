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
   public class AlterTableStatementProcessor : AbstractProcessor<AlterTableStatement>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, AlterTableStatement obj, StringBuilder builder )
      {
         builder.Append( "ALTER TABLE " );
         aggregator.Process( obj.TableName, builder );
         builder.Append( SQLConstants.NEWLINE );
         aggregator.Process( obj.AlterAction, builder );
      }
   }

   public class AddColumnDefinitionProcessor : AbstractProcessor<AddColumnDefinition>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, AddColumnDefinition obj, StringBuilder builder )
      {
         builder.Append( "ADD COLUMN " );
         aggregator.Process( obj.ColumnDefinition, builder );
      }
   }

   public class AddTableConstraintDefinitionProcessor : AbstractProcessor<AddTableConstraintDefinition>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, AddTableConstraintDefinition obj, StringBuilder builder )
      {
         builder.Append( "ADD " );
         aggregator.Process( obj.ConstraintDefinition, builder );
      }
   }

   public class AlterColumnDefinitionProcessor : AbstractProcessor<AlterColumnDefinition>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, AlterColumnDefinition obj, StringBuilder builder )
      {
         builder.Append( "ALTER COLUMN " ).Append( obj.ColumnName ).Append( SQLConstants.TOKEN_SEPARATOR );
         aggregator.Process( obj.AlterAction, builder );
      }
   }

   public class SetColumnDefaultProcessor : AbstractProcessor<SetColumnDefault>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SetColumnDefault obj, StringBuilder builder )
      {
         builder.Append( "SET " ).Append( obj.DefaultValue );
      }
   }

   public abstract class AbstractDropBehaviourContainerProcessor<T> : AbstractProcessor<T>
      where T : class, DropBehaviourContainer
   {
      private static readonly IDictionary<DropBehaviour, String> DEFAULT_DROP_BEHAVIOURS = new Dictionary<DropBehaviour, String>
      {
         { DropBehaviour.Cascade, "CASCADE" },
         { DropBehaviour.Restrict, "RESTRICT" }
      };

      private readonly IDictionary<DropBehaviour, String> _dropBehaviours;

      protected AbstractDropBehaviourContainerProcessor( IDictionary<DropBehaviour, String> dropBehaviours = null )
      {
         this._dropBehaviours = dropBehaviours ?? DEFAULT_DROP_BEHAVIOURS;
      }

      protected void ProcessDropBehaviour( DropBehaviour db, StringBuilder builder )
      {
         builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( this._dropBehaviours[db] );
      }
   }

   public class DropColumnDefinitionProcessor : AbstractDropBehaviourContainerProcessor<DropColumnDefinition>
   {
      public DropColumnDefinitionProcessor( IDictionary<DropBehaviour, String> dropBehaviours = null )
         : base( dropBehaviours )
      {
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, DropColumnDefinition obj, StringBuilder builder )
      {
         builder.Append( "DROP COLUMN " ).Append( obj.ColumnName );
         this.ProcessDropBehaviour( obj.DropBehaviour, builder );
      }
   }

   public class DropTableConstraintDefinitionProcessor : AbstractDropBehaviourContainerProcessor<DropTableConstraintDefinition>
   {
      public DropTableConstraintDefinitionProcessor( IDictionary<DropBehaviour, String> dropBehaviours = null )
         : base( dropBehaviours )
      {
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, DropTableConstraintDefinition obj, StringBuilder builder )
      {
         builder.Append( "DROP CONSTRAINT " ).Append( obj.ConstraintName );
         this.ProcessDropBehaviour( obj.DropBehaviour, builder );
      }
   }

   public class DropSchemaStatementProcessor : AbstractDropBehaviourContainerProcessor<DropSchemaStatement>
   {
      public DropSchemaStatementProcessor( IDictionary<DropBehaviour, String> dropBehaviours = null )
         : base( dropBehaviours )
      {
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, DropSchemaStatement obj, StringBuilder builder )
      {
         builder.Append( "DROP SCHEMA " ).Append( obj.SchemaName );
         this.ProcessDropBehaviour( obj.DropBehaviour, builder );
      }
   }

   public class DropTableOrViewStatementProcessor : AbstractDropBehaviourContainerProcessor<DropTableOrViewStatement>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, DropTableOrViewStatement obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.DROP + SQLConstants.TOKEN_SEPARATOR );
         switch ( obj.WhatToDrop )
         {
            case ObjectType.Table:
               builder.Append( SQLConstants.TABLE );
               break;
            case ObjectType.View:
               builder.Append( SQLConstants.VIEW );
               break;
         }
         builder.Append( SQLConstants.TOKEN_SEPARATOR );

         this.BeforeTableName( aggregator, obj, builder );

         aggregator.Process( obj.TableName, builder );
         this.ProcessDropBehaviour( obj.DropBehaviour, builder );
      }

      protected virtual void BeforeTableName( SQLProcessorAggregator aggregator, DropTableOrViewStatement obj, StringBuilder builder )
      {

      }
   }
}
