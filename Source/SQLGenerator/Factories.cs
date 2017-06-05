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
   public interface AbstractSQLFactory : ObjectWithVendor
   {
   }


   public interface CommonFactory : AbstractSQLFactory
   {
      EqualsPredicate Eq( ValueExpression left, ValueExpression right );
      NotEqualsPredicate Neq( ValueExpression left, ValueExpression right );
      LessThanPredicate Lt( ValueExpression left, ValueExpression right );
      LessOrEqualToPredicate Leq( ValueExpression left, ValueExpression right );
      GreaterThanPredicate Gt( ValueExpression left, ValueExpression right );
      GreaterOrEqualToPredicate Geq( ValueExpression left, ValueExpression right );
      IsNullPredicate IsNull( ValueExpression what );
      Negation Not( BooleanExpression expr );
      Conjunction And( BooleanExpression left, BooleanExpression right );
      Disjunction Or( BooleanExpression left, BooleanExpression right );
      BetweenPredicate Between( ValueExpression left, ValueExpression minimum, ValueExpression maximum );
      InPredicate In( ValueExpression what, ImmutableArray<ValueExpression> values );
      LikePredicate Like( ValueExpression what, ValueExpression pattern );
      RegexpPredicate Regexp( ValueExpression what, ValueExpression pattern );
      ExistsPredicate Exists( QueryExpression query );
      UniquePredicate Unique( QueryExpression query );
      BooleanTest Test( BooleanExpression expr, Boolean? truthValue );
      BooleanBuilder NewBooleanBuilder( BooleanExpression first = null );

      BinaryArithmeticExpression Plus( NonBooleanExpression left, NonBooleanExpression right );
      BinaryArithmeticExpression Minus( NonBooleanExpression left, NonBooleanExpression right );
      BinaryArithmeticExpression Mul( NonBooleanExpression left, NonBooleanExpression right );
      BinaryArithmeticExpression Div( NonBooleanExpression left, NonBooleanExpression right );
      UnaryArithmeticExpression UnaryPlus( NonBooleanExpression expr );
      UnaryArithmeticExpression UnaryMinus( NonBooleanExpression expr );


      BooleanExpression True { get; }
      BooleanExpression False { get; }
      Predicate Empty { get; }

      ValueExpression Default { get; }
      ValueExpression Null { get; }

      ColumnNameList ColumnNames( ImmutableArray<String> names );
      TableNameDirect TableNameDirect( String schemaName, String tableName );

      StringLiteral String( String str );
      DirectLiteral Param { get; }
      DirectLiteral Literal( String contents );
      TimestampLiteral Timestamp( DateTime? dt );
      Int32NumericLiteral I32( Int32? i32 );
      Int64NumericLiteral I64( Int64? i64 );
      DoubleNumericLiteral Double( Double? dbl );
      DecimalNumericLiteral Decimal( Decimal? dec );
      SQLFunctionLiteral Function( String name, ImmutableArray<ValueExpression> parameters );
   }

   public interface DefinitionFactory : AbstractSQLFactory
   {
      SQLDTBigInt BigInt();
      SQLDTDecimal Decimal( Int32? precision = null, Int32? scale = null );
      SQLDTDoublePrecision Double();
      SQLDTNumeric Numeric( Int32? precision = null, Int32? scale = null );
      SQLDTReal Real();
      SQLDTSmallInt SmallInt();
      SQLDTBoolean Boolean();
      SQLDTChar Char( Boolean varying, Int32? length = null );
      SQLDTDate Date();
      SQLDTFloat Float( Int32? precision = null );
      SQLDTInteger Integer();
      SQLDTInterval YearMonthInterval( IntervalDataTypes startField, Int32? startFieldPrecision = null, IntervalDataTypes? endField = null );
      SQLDTInterval DayTimeInterval( IntervalDataTypes startField, Int32? startFieldPrcision = null, IntervalDataTypes? endField = null, Int32? secondFracs = null );
      SQLDTTime Time( Boolean? includeTimeZone = null, Int32? precision = null );
      SQLDTTimestamp TimeStamp( Boolean? includeTimeZone = null, Int32? precision = null );
      SQLDTUserDefined UserDefined( String textualContent );

      SchemaDefinitionBuilder NewSchemaDefinitionBuilder();
      TableDefinitionBuilder NewTableDefinitionBuilder();
      TableElementListBuilder NewTableElementListBuilder();
      UniqueConstraintBuilder NewUniqueConstraintBuilder();
      ViewDefinitionBuilder NewViewDefinitionBuilder();
      ForeignKeyConstraintBuilder NewForeignKeyConstraintBuilder();
      ColumnDefinition NewColumnDefinition( String columnName, SQLDataType columnDataType, Boolean mayBeNull = true, String defaultValue = null, AutoGenerationPolicy? autoGenerationPolicy = null );
      LikeClause NewLikeClause( TableNameDirect tableName );
      TableConstraintDefinition NewTableConstraintDefinition( TableConstraint constraint, ConstraintCharacteristics? constraintChars = null, String name = null );
      CheckConstraint NewCheckConstraint( BooleanExpression check );
      RegularViewSpecification NewRegularViewSpecification( ImmutableArray<String> columnNames );
      ForeignKeyConstraint NewForeignKeyConstraint( ImmutableArray<String> sourceColumns, TableNameDirect targetTable, ImmutableArray<String> targetColumns, MatchType? matchType = null, ReferentialAction? onUpdate = null, ReferentialAction? onDelete = null );
      SchemaDefinition NewSchemaDefinition( String schemaName, String schemaCharSet = null, ImmutableArray<SchemaElement> elements = null );
      TableDefinition NewTableDefinition( TableNameDirect tableName, TableContentsSource contents, TableScope? tableScope = null, TableCommitAction? commitAction = null );
      UniqueConstraint NewUniqueConstraint( UniqueSpecification uniquenessKind, ImmutableArray<String> columns );
      ViewDefinition NewViewDefinition( TableNameDirect viewName, QueryExpression query, ViewSpecification viewSpec, ViewCheckOption? viewCheck = null, Boolean isRecursive = false );
      TableElementList NewTableElementList( ImmutableArray<TableElement> elements );
   }

   public interface ManipulationFactory : AbstractSQLFactory
   {
      AlterTableStatement NewAlterTableStatement( TableNameDirect tableName, AlterTableAction action );
      AddColumnDefinition NewAddColumnDefinition( ColumnDefinition definition );
      AddTableConstraintDefinition NewAddTableConstraintDefinition( TableConstraintDefinition constraintDefinition );
      AlterColumnDefinition NewAlterColumnDefinition( String columnName, AlterColumnAction action );
      SetColumnDefault NewSetColumnDefault( String newDefault );
      DropColumnDefinition NewDropColumnDefinition( String columnName, DropBehaviour dropBehaviour );
      DropTableConstraintDefinition NewDropTableConstraintDefinition( String constraintName, DropBehaviour dropBehaviour );
      DropSchemaStatement NewDropSchemaStatement( String schemaName, DropBehaviour dropBehaviour );
      DropTableOrViewStatement NewDropTableOrViewStatement( TableNameDirect tableName, ObjectType theType, DropBehaviour dropBehaviour );
      AlterColumnAction AlterColumn_DropDefault();
   }

   public interface ModificationFactory : AbstractSQLFactory
   {
      ColumnSourceByValuesBuilder NewColumnSourceByValuesBuilder();
      DeleteBySearchBuilder NewDeleteBySearchBuilder();
      InsertStatementBuilder NewInsertStatementBuilder();
      UpdateBySearchBuilder NewUpdateBySearchBuilder();
      ColumnSourceByQuery NewColumnSourceByQuery( QueryExpression query, ImmutableArray<String> columns = null );
      TargetTable NewTargetTable( TableNameDirect tableName, Boolean isOnly = false );
      UpdateSourceByExpression NewUpdateSourceByExpression( ValueExpression expression );
      SetClause NewSetClause( String updateTarget, UpdateSource updateSource );
      ColumnSourceByValues NewColumnSourceByValues( ImmutableArray<ImmutableArray<ValueExpression>> values, ColumnNameList names = null );
      DeleteBySearch NewDeleteBySearch( TargetTable targetTable, BooleanExpression condition = null );
      InsertStatement NewInsertStatement( TableNameDirect table, ColumnSource columnSource );
      UpdateBySearch NewUpdateBySearch( TargetTable target, ImmutableArray<SetClause> setClauses, BooleanExpression condition = null );

      ColumnSource Defaults { get; }
   }

   public interface QueryFactory : AbstractSQLFactory
   {
      QuerySpecificationBuilder NewQuerySpecificationBuilder();
      QueryExpressionBodyBuilder NewQueryExpressionBodyBuilder( QueryExpressionBody startingExpression = null );
      JoinedTableBuilder NewJoinedTableBuilder( TableReferencePrimary startingTable );

      QueryExpressionBody Empty { get; }
      GroupingElement GrandTotal { get; }

      ColumnReferenceByExpression ColumnExpression( ValueExpression expression );
      ColumnReferenceByName ColumnName( String tableName, String columnName );
      QueryExpression NewQuery( QueryExpressionBody body );
      OrdinaryGroupingSet NewGroupingElement( ImmutableArray<NonBooleanExpression> expressions );
      SortSpecification NewSortSpecification( ValueExpression expression, Ordering ordering );
      TableValueConstructor NewTableValueConstructor( ImmutableArray<RowValueConstructor> rows );
      RowSubQuery NewRowSubQuery( QueryExpression subQuery );
      RowDefinition NewRow( ImmutableArray<ValueExpression> elements );

      TableNameFunction NewTableNameFunction( SQLFunctionLiteral function, String schemaName = null );
      TableReferenceByName NewTableReferenceByName( TableName tableName, TableAlias alias = null );
      TableAlias NewTableAlias( String tableNameAlias, ImmutableArray<String> renamedColumns );
      TableReferenceByQuery NewTableReferenceByQuery( QueryExpression query, TableAlias alias = null );
      JoinCondition NewJoinCondition( BooleanExpression condition );
      NamedColumnsJoin NewNamedColumnsJoin( ImmutableArray<String> columnNames );

      AsteriskSelect NewSelectAllClause( SetQuantifier quantifier );
      ColumnReferences NewSelectClause( SetQuantifier quantifier, ImmutableArray<ColumnReferenceInfo> cols );
      FromClause NewFromClause( ImmutableArray<TableReference> tableRefs );
      QualifiedJoinedTable NewQualifiedJoinedTable( TableReference left, TableReference right, JoinSpecification joinSpec, JoinType jType = JoinType.Inner );
      CrossJoinedTable NewCrossJoinedTable( TableReference left, TableReference right );
      NaturalJoinedTable NewNaturalJoinedTable( TableReference left, TableReference right, JoinType jType = JoinType.Inner );
      UnionJoinedTable NewUnionJoinedTable( TableReference left, TableReference right );
      GroupByClause NewGroupByClause( ImmutableArray<GroupingElement> elements );
      OrderByClause NewOrderByClause( ImmutableArray<SortSpecification> sortSpecs );
      QueryExpressionBodyBinary NewBinaryQuery( QueryExpressionBody left, QueryExpressionBody right, SetOperations setOperation, SetQuantifier quantifier = SetQuantifier.Distinct, CorrespondingSpec correspondingSpec = null );
      QuerySpecification NewQuerySpecification( SelectColumnClause select, FromClause from = null, BooleanExpression where = null, GroupByClause groupBy = null, BooleanExpression having = null, OrderByClause orderBy = null, NonBooleanExpression offset = null, NonBooleanExpression limit = null );
   }
}

public static partial class E_SQLGenerator
{
   public static Negation IsNotNull( this CommonFactory factory, NonBooleanExpression what )
   {
      return factory.Not( factory.IsNull( what ) );
   }

   public static Negation NotBetween( this CommonFactory factory, NonBooleanExpression left, NonBooleanExpression minimum, NonBooleanExpression maximum )
   {
      return factory.Not( factory.Between( left, minimum, maximum ) );
   }

   public static Negation NotIn( this CommonFactory factory, NonBooleanExpression what, params NonBooleanExpression[] values )
   {
      return factory.Not( factory.In( what, values ) );
   }

   public static Negation NotLike( this CommonFactory factory, NonBooleanExpression what, NonBooleanExpression pattern )
   {
      return factory.Not( factory.Like( what, pattern ) );
   }

   public static Negation NotRegexp( this CommonFactory factory, NonBooleanExpression what, NonBooleanExpression pattern )
   {
      return factory.Not( factory.Regexp( what, pattern ) );
   }

   public static Negation IsNot( this CommonFactory factory, BooleanExpression expr, Boolean? truthValue )
   {
      return factory.Not( factory.Test( expr, truthValue ) );
   }

   [CLSCompliant( false )]
   public static Int32NumericLiteral I8( this CommonFactory factory, SByte? sb )
   {
      return factory.I32( sb );
   }

   public static Int32NumericLiteral U8( this CommonFactory factory, Byte? b )
   {
      return factory.I32( b );
   }

   public static Int32NumericLiteral I16( this CommonFactory factory, Int16? i16 )
   {
      return factory.I32( i16 );
   }

   [CLSCompliant( false )]
   public static Int32NumericLiteral U16( this CommonFactory factory, UInt16? u16 )
   {
      return factory.I32( u16 );
   }

   // TODO U64
   public static DoubleNumericLiteral Single( this CommonFactory factory, Single? s )
   {
      return factory.Double( s );
   }

   public static ColumnReferenceByName ColumnName( this QueryFactory factory, String columnName )
   {
      return factory.ColumnName( null, columnName );
   }

   public static TableNameDirect TableNameDirect( this CommonFactory factory, String tableName )
   {
      return factory.TableNameDirect( null, tableName );
   }

   public static QueryExpression CallFunction( this QueryFactory factory, String schemaName, SQLFunctionLiteral function )
   {
      var b = factory.NewQuerySpecificationBuilder();
      b.ColumnsBuilder.SelectAll();
      b.FromBuilder.AddTableNamesP( Tuple.Create<String, TableName>( null, factory.NewTableNameFunction( function, schemaName ) ) );
      return factory.NewQuery( b.CreateExpression() );
   }

   public static QueryExpression CallFunction( this QueryFactory factory, SQLFunctionLiteral function )
   {
      return CallFunction( factory, null, function );
   }

   public static InPredicate In( this CommonFactory factory, ValueExpression what, params ValueExpression[] values )
   {
      return factory.In( what, ArrayQueryHelper.NewAQ( values, false ) );
   }

   public static ColumnNameList ColumnNames( this CommonFactory factory, params String[] names )
   {
      return factory.ColumnNames( ArrayQueryHelper.NewAQ( names, false ) );
   }

   public static SQLFunctionLiteral Function( this CommonFactory factory, String name, params ValueExpression[] parameters )
   {
      return factory.Function( name, ArrayQueryHelper.NewAQ( parameters, false ) );
   }

   public static RegularViewSpecification NewRegularViewSpecification( this DefinitionFactory factory, params String[] columnNames )
   {
      return factory.NewRegularViewSpecification( ArrayQueryHelper.NewAQ( columnNames, false ) );
   }

   public static ColumnSourceByValues NewColumnSourceByValues( this ModificationFactory factory, params ValueExpression[] values )
   {
      return NewColumnSourceByValues( factory, null, values );
   }

   public static ColumnSourceByValues NewColumnSourceByValues( this ModificationFactory factory, ColumnNameList names, params ValueExpression[] values )
   {
      return factory.NewColumnSourceByValues( new[] { values.NewAQ() }.NewAQ( false ), names );
   }

   public static ColumnSourceByValues NewColumnSourceByValuesMultiple( this ModificationFactory factory, ColumnNameList names, params ValueExpression[][] values )
   {
      return factory.NewColumnSourceByValues( values.Select( v => v.NewAQ() ).NewAQ() );
   }

   public static UpdateBySearch NewUpdateBySearch( this ModificationFactory factory, TargetTable target, BooleanExpression condition = null, params SetClause[] setClauses )
   {
      return factory.NewUpdateBySearch( target, ArrayQueryHelper.NewAQ( setClauses, false ), condition );
   }

   public static OrdinaryGroupingSet NewGroupingElement( this QueryFactory factory, params NonBooleanExpression[] expressions )
   {
      return factory.NewGroupingElement( ArrayQueryHelper.NewAQ( expressions, false ) );
   }

   public static TableValueConstructor NewTableValueConstructor( this QueryFactory factory, params RowValueConstructor[] rows )
   {
      return factory.NewTableValueConstructor( ArrayQueryHelper.NewAQ( rows, false ) );
   }

   public static RowDefinition NewRow( this QueryFactory factory, params ValueExpression[] elements )
   {
      return factory.NewRow( ArrayQueryHelper.NewAQ( elements, false ) );
   }

   public static TableAlias NewTableAlias( this QueryFactory factory, String tableNameAlias, params String[] renamedColumns )
   {
      return factory.NewTableAlias( tableNameAlias, renamedColumns.Length > 0 ? ArrayQueryHelper.NewAQ( renamedColumns, false ) : null );
   }

   public static NamedColumnsJoin NewNamedColumnsJoin( this QueryFactory factory, params String[] columnNames )
   {
      return factory.NewNamedColumnsJoin( ArrayQueryHelper.NewAQ( columnNames, false ) );
   }

   public static ColumnReferences NewSelectClause( this QueryFactory factory, SetQuantifier quantifier, params ColumnReferenceInfo[] cols )
   {
      return factory.NewSelectClause( quantifier, ArrayQueryHelper.NewAQ( cols, false ) );
   }

   public static ColumnReferences NewSelectClause( this QueryFactory factory, params ColumnReferenceInfo[] cols )
   {
      return NewSelectClause( factory, SetQuantifier.All, cols );
   }

   public static ColumnReferences NewSelectClause( this QueryFactory factory, params ColumnReference[] cols )
   {
      return NewSelectClause( factory, SetQuantifier.All, cols.Select( col => new ColumnReferenceInfo( col ) ).ToArray() );
   }

   public static FromClause NewFromClause( this QueryFactory factory, params TableReference[] tableRefs )
   {
      return factory.NewFromClause( ArrayQueryHelper.NewAQ( tableRefs, false ) );
   }

   public static GroupByClause NewGroupByClause( this QueryFactory factory, params GroupingElement[] elements )
   {
      return factory.NewGroupByClause( ArrayQueryHelper.NewAQ( elements, false ) );
   }

   public static OrderByClause NewOrderByClause( this QueryFactory factory, params SortSpecification[] sortSpecs )
   {
      return factory.NewOrderByClause( ArrayQueryHelper.NewAQ( sortSpecs, false ) );
   }

   public static TableElementList NewTableElementList( this DefinitionFactory factory, params TableElement[] elements )
   {
      return factory.NewTableElementList( elements.NewAQ( false ) );
   }

   public static UniqueConstraint NewUniqueConstraint( this DefinitionFactory factory, UniqueSpecification uniquenessKind, params String[] columns )
   {
      return factory.NewUniqueConstraint( uniquenessKind, columns.NewAQ( false ) );
   }

   public static ColumnSourceByQuery NewColumnSourceByQuery( this ModificationFactory factory, QueryExpression query, params String[] columns )
   {
      return factory.NewColumnSourceByQuery( query, columns.NewAQ( false ) );
   }

   public static SQLDTChar Varchar( this DefinitionFactory factory, Int32? length = null )
   {
      return factory.Char( true, length );
   }

   public static SQLDTChar Char( this DefinitionFactory factory, Int32? length = null )
   {
      return factory.Char( false, length );
   }

   public static TableNameDirect TableNameDirect( this QueryFactory q, String tableName )
   {
      return q.TableNameDirect( null, tableName );
   }

   public static TableNameDirect TableNameDirect( this QueryFactory q, String schemaName, String tableName )
   {
      return q.SQLVendor.CommonFactory.TableNameDirect( schemaName, tableName );
   }

   public static BooleanExpression AndMultiple( this CommonFactory c, params BooleanExpression[] expressions )
   {
      // If we want to return conjunction, then for empty expressions need to explicitly do c.And(c.Empty, c.Empty);
      var builda = c.NewBooleanBuilder();
      foreach ( var e in expressions )
      {
         builda.And( e );
      }
      return builda.CreateExpression();
   }

   public static BooleanExpression OrMultiple( this CommonFactory c, params BooleanExpression[] expressions )
   {
      var builda = c.NewBooleanBuilder();
      foreach ( var e in expressions )
      {
         builda.Or( e );
      }
      return builda.CreateExpression();
   }
}
