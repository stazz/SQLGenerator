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
   public class SchemaDefinitionProcessor : AbstractProcessor<SchemaDefinition>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, SchemaDefinition obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.CREATE ).Append( SQLConstants.SCHEMA ).Append( obj.SchemaName );
         if ( obj.SchemaCharset != null )
         {
            builder.Append( " DEFAULT CHARSET " ).Append( obj.SchemaCharset );
         }
         this.ProcessSchemaElements( aggregator, obj, builder );
      }

      protected void ProcessSchemaElements( SQLProcessorAggregator aggregator, SchemaDefinition obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.NEWLINE );
         foreach ( var el in obj.SchemaElements )
         {
            aggregator.Process( el, builder );
            builder.Append( SQLConstants.NEWLINE );
         }
      }
   }

   public class TableDefinitionProcessor : AbstractProcessor<TableDefinition>
   {
      private static readonly IDictionary<TableScope, String> DEFAULT_TABLE_SCOPES = new Dictionary<TableScope, String>
      {
         { TableScope.GlobalTemporary, "GLOBAL TEMPORARY" },
         { TableScope.LocalTemporary, "LOCAL TEMPORARY" }
      };
      private static readonly IDictionary<TableCommitAction, String> DEFAULT_COMMIT_ACTIONS = new Dictionary<TableCommitAction, String>
      {
         { TableCommitAction.OnCommitDeleteRows, "DELETE ROWS" },
         { TableCommitAction.OnCommitPreserveRows, "PRESERVE ROWS" }
      };

      private readonly IDictionary<TableScope, String> _tableScopes;
      private readonly IDictionary<TableCommitAction, String> _commitActions;

      public static IDictionary<TableCommitAction, String> NewCopyOfDefaultCommitActions()
      {
         return new Dictionary<TableCommitAction, String>( DEFAULT_COMMIT_ACTIONS );
      }

      public TableDefinitionProcessor( IDictionary<TableScope, String> tableScopes = null, IDictionary<TableCommitAction, String> tableCommitActions = null )
      {
         this._tableScopes = tableScopes ?? DEFAULT_TABLE_SCOPES;
         this._commitActions = tableCommitActions ?? DEFAULT_COMMIT_ACTIONS;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, TableDefinition obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.CREATE );
         if ( obj.TableScope.HasValue )
         {
            builder.Append( this._tableScopes[obj.TableScope.Value] ).Append( SQLConstants.TOKEN_SEPARATOR );
         }
         builder.Append( SQLConstants.TABLE );

         aggregator.Process( obj.TableName, builder );
         builder.Append( SQLConstants.NEWLINE );

         aggregator.Process( obj.Contents, builder );
         builder.Append( SQLConstants.NEWLINE );

         if ( obj.CommitAction.HasValue )
         {
            builder.Append( "ON COMMIT " ).Append( this._commitActions[obj.CommitAction.Value] );
         }
      }
   }

   public class TableElementListProcessor : AbstractProcessor<TableElementList>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, TableElementList obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.OPEN_PARENTHESIS ).Append( SQLConstants.NEWLINE );
         ProcessorUtils.AppendEnumerable( aggregator, builder, obj.Elements, SQLConstants.COMMA + SQLConstants.NEWLINE );
         builder.Append( SQLConstants.NEWLINE ).Append( SQLConstants.CLOSE_PARENTHESIS );
      }
   }

   public class ColumnDefinitionProcessor : AbstractProcessor<ColumnDefinition>
   {
      private static readonly IDictionary<AutoGenerationPolicy, String> AUTOGEN_POLICIES = new Dictionary<AutoGenerationPolicy, String>
      {
         { AutoGenerationPolicy.Always, "ALWAYS" },
         { AutoGenerationPolicy.ByDefault, "BY DEFAULT" }
      };

      private readonly IDictionary<AutoGenerationPolicy, String> _autoGenPolicies;

      public ColumnDefinitionProcessor( IDictionary<AutoGenerationPolicy, String> autoGenPolicies = null )
      {
         this._autoGenPolicies = autoGenPolicies ?? AUTOGEN_POLICIES;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, ColumnDefinition obj, StringBuilder builder )
      {
         builder.Append( obj.ColumnName ).Append( SQLConstants.TOKEN_SEPARATOR );

         this.ProcessDataType( aggregator, obj, builder );

         if ( obj.Default != null )
         {
            builder.Append( " DEFAULT " ).Append( obj.Default );
         }

         this.ProcessMayBeNull( obj, builder );

         if ( obj.AutoGenerationPolicy.HasValue )
         {
            this.ProcessAutoGenerationPolicy( obj, builder );
         }
      }

      protected virtual void ProcessMayBeNull( ColumnDefinition obj, StringBuilder builder )
      {
         if ( !obj.MayBeNull )
         {
            builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( "NOT NULL" );
         }
      }

      protected virtual void ProcessDataType( SQLProcessorAggregator aggregator, ColumnDefinition obj, StringBuilder builder )
      {
         aggregator.Process( obj.DataType, builder );
      }

      protected virtual void ProcessAutoGenerationPolicy( ColumnDefinition obj, StringBuilder builder )
      {
         builder.Append( " GENERATED " ).Append( this._autoGenPolicies[obj.AutoGenerationPolicy.Value] ).Append( " AS IDENTITY" );
      }
   }

   public class LikeClauseProcessor : AbstractProcessor<LikeClause>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, LikeClause obj, StringBuilder builder )
      {
         builder.Append( "LIKE " );
         aggregator.Process( obj.TableName, builder );
      }
   }

   public class TableConstraintDefinitionProcessor : AbstractProcessor<TableConstraintDefinition>
   {
      private static readonly IDictionary<ConstraintCharacteristics, String> DEFAULT_CHARACTERISTICS = new Dictionary<ConstraintCharacteristics, String>
      {
         { ConstraintCharacteristics.InitiallyDeferred_Deferrable, "INITIALLY DEFERRED DEFERRABLE" },
         { ConstraintCharacteristics.InitiallyImmediate_Deferrable, "INITIALLY IMMEDIATE DEFERRABLE" },
         { ConstraintCharacteristics.NotDeferrable, "NOT DEFERRABLE" }
      };

      private readonly IDictionary<ConstraintCharacteristics, String> _characteristics;

      public TableConstraintDefinitionProcessor( IDictionary<ConstraintCharacteristics, String> characteristics = null )
      {
         this._characteristics = characteristics ?? DEFAULT_CHARACTERISTICS;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, TableConstraintDefinition obj, StringBuilder builder )
      {
         if ( obj.ConstraintName != null )
         {
            builder.Append( "CONSTRAINT " ).Append( obj.ConstraintName ).Append( SQLConstants.TOKEN_SEPARATOR );
         }

         aggregator.Process( obj.Constraint, builder );

         if ( obj.ConstraintCharacteristics.HasValue )
         {
            builder.Append( SQLConstants.TOKEN_SEPARATOR ).Append( this._characteristics[obj.ConstraintCharacteristics.Value] );
         }
      }
   }

   public class CheckConstraintProcessor : AbstractProcessor<CheckConstraint>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, CheckConstraint obj, StringBuilder builder )
      {
         builder.Append( "CHECK " ).Append( SQLConstants.OPEN_PARENTHESIS );
         aggregator.Process( obj.CheckCondition, builder );
         builder.Append( SQLConstants.CLOSE_PARENTHESIS );
      }
   }

   public class UniqueConstraintProcessor : AbstractProcessor<UniqueConstraint>
   {
      private static readonly IDictionary<UniqueSpecification, String> DEFAULT_UNIQUE_SPECS = new Dictionary<UniqueSpecification, String>
      {
         { UniqueSpecification.PrimaryKey, "PRIMARY KEY" },
         { UniqueSpecification.Unique, "UNIQUE" }
      };

      private readonly IDictionary<UniqueSpecification, String> _uniqueSpecs;

      public UniqueConstraintProcessor( IDictionary<UniqueSpecification, String> uniqueSpecs = null )
      {
         this._uniqueSpecs = uniqueSpecs ?? DEFAULT_UNIQUE_SPECS;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, UniqueConstraint obj, StringBuilder builder )
      {
         builder.Append( this._uniqueSpecs[obj.UniquenessKind] );
         aggregator.Process( obj.ColumnNames, builder );
      }
   }

   public class ForeignKeyConstraintProcessor : AbstractProcessor<ForeignKeyConstraint>
   {
      private static readonly IDictionary<ReferentialAction, String> DEFAULT_REFERENTIAL_ACTIONS = new Dictionary<ReferentialAction, String>
      {
         { ReferentialAction.Cascade, "CASCADE" },
         { ReferentialAction.NoAction, "NO ACTION" },
         { ReferentialAction.Restrict, "RESTRICT" },
         { ReferentialAction.SetDefault, "SET DEFAULT" },
         { ReferentialAction.SetNull, "SET NULL" }
      };

      private static readonly IDictionary<MatchType, String> DEFAULT_MATCH_TYPES = new Dictionary<MatchType, String>
      {
         { MatchType.Full, "FULL" },
         { MatchType.Partial, "PARTIAL" },
         { MatchType.Simple, "SIMPLE" }
      };

      private readonly IDictionary<ReferentialAction, String> _referentialActions;
      private readonly IDictionary<MatchType, String> _matchTypes;

      public ForeignKeyConstraintProcessor( IDictionary<ReferentialAction, String> referentialActions = null, IDictionary<MatchType, String> matchTypes = null )
      {
         this._referentialActions = referentialActions ?? DEFAULT_REFERENTIAL_ACTIONS;
         this._matchTypes = matchTypes ?? DEFAULT_MATCH_TYPES;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, ForeignKeyConstraint obj, StringBuilder builder )
      {
         builder.Append( "FOREIGN KEY" );
         aggregator.Process( obj.SourceColumns, builder );

         builder.Append( SQLConstants.NEWLINE ).Append( "REFERENCES " );
         aggregator.Process( obj.TargetTable, builder );

         if ( obj.TargetColumns != null )
         {
            aggregator.Process( obj.TargetColumns, builder );
         }

         if ( obj.MatchType.HasValue )
         {
            builder.Append( " MATCH " ).Append( this._matchTypes[obj.MatchType.Value] );
         }
         builder.Append( SQLConstants.NEWLINE );

         this.HandleReferentialAction( "ON UPDATE ", obj.OnUpdate, builder );
         builder.Append( SQLConstants.TOKEN_SEPARATOR );
         this.HandleReferentialAction( "ON DELETE ", obj.OnDelete, builder );
      }

      protected virtual void HandleReferentialAction( String prefix, ReferentialAction? action, StringBuilder builder )
      {
         if ( action.HasValue )
         {
            builder.Append( prefix ).Append( this._referentialActions[action.Value] );
         }
      }
   }

   public class ViewDefinitionProcessor : AbstractProcessor<ViewDefinition>
   {
      private static readonly IDictionary<ViewCheckOption, String> DEFAULT_VIEW_CHECK_OPTIONS = new Dictionary<ViewCheckOption, String>
      {
         { ViewCheckOption.Cascaded, "CASCADED" },
         { ViewCheckOption.Local, "LOCAL" }
      };

      private readonly IDictionary<ViewCheckOption, String> _viewCheckOptions;

      public ViewDefinitionProcessor( IDictionary<ViewCheckOption, String> viewCheckOptions = null )
      {
         this._viewCheckOptions = viewCheckOptions ?? DEFAULT_VIEW_CHECK_OPTIONS;
      }

      protected override void DoProcess( SQLProcessorAggregator aggregator, ViewDefinition obj, StringBuilder builder )
      {
         builder.Append( SQLConstants.CREATE );
         if ( obj.IsRecursive )
         {
            builder.Append( "RECURSIVE " );
         }
         builder.Append( SQLConstants.VIEW );

         aggregator.Process( obj.ViewName, builder );
         aggregator.Process( obj.ViewSpecification, builder );
         builder.Append( SQLConstants.ALIAS_DEFINER ).Append( SQLConstants.NEWLINE );
         aggregator.Process( obj.ViewQuery, builder );

         if ( obj.ViewCheckOption.HasValue )
         {
            builder.Append( SQLConstants.NEWLINE ).Append( SQLConstants.WITH ).Append( this._viewCheckOptions[obj.ViewCheckOption.Value] ).Append( " CHECK OPTION" );
         }
      }
   }

   public class RegularViewSpecificationProcessor : AbstractProcessor<RegularViewSpecification>
   {
      protected override void DoProcess( SQLProcessorAggregator aggregator, RegularViewSpecification obj, StringBuilder builder )
      {
         if ( obj.Columns != null )
         {
            aggregator.Process( obj.Columns, builder );
         }
      }
   }
}
