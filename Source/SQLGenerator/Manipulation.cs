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
using SQLGenerator;

namespace SQLGenerator
{
   public interface SchemaManipulationStatement : SchemaStatement
   {
   }

   public interface AlterStatement : SchemaManipulationStatement
   {
   }

   public interface AlterTableStatement : AlterStatement
   {
      TableNameDirect TableName { get; }
      AlterTableAction AlterAction { get; }
   }
   public interface AlterTableAction : SQLElement
   {
   }
   public interface AddColumnDefinition : AlterTableAction
   {
      ColumnDefinition ColumnDefinition { get; }
   }
   public interface AddTableConstraintDefinition : AlterTableAction
   {
      TableConstraintDefinition ConstraintDefinition { get; }
   }
   public interface AlterColumnDefinition : AlterTableAction
   {
      String ColumnName { get; }
      AlterColumnAction AlterAction { get; }
   }
   public interface DropColumnDefinition : AlterTableAction, DropBehaviourContainer
   {
      String ColumnName { get; }
   }
   public interface DropTableConstraintDefinition : AlterTableAction, DropBehaviourContainer
   {
      String ConstraintName { get; }
   }

   public interface AlterColumnAction : ObjectWithVendor
   {
   }

   public interface SetColumnDefault : AlterColumnAction
   {
      //TODO is string enough?
      String DefaultValue { get; }
   }

   public interface DropBehaviourContainer
   {
      DropBehaviour DropBehaviour { get; }
   }
   public enum DropBehaviour { Cascade, Restrict, MaxValue }

   public interface DropStatement : SchemaManipulationStatement, DropBehaviourContainer
   {
   }
   public interface DropSchemaStatement : DropStatement
   {
      String SchemaName { get; }
   }
   public enum ObjectType { Table, View }
   public interface DropTableOrViewStatement : DropStatement
   {
      ObjectType WhatToDrop { get; }
      TableNameDirect TableName { get; }
   }
}
