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
   public abstract class AbstractSQLFactoryImpl : AbstractSQLFactory
   {
      protected static readonly ImmutableArray<String> EMPTY_STRING_AQ = new String[] { }.NewAQ();

      protected readonly SQLVendorImpl vendor;

      public AbstractSQLFactoryImpl( SQLVendorImpl vendor )
      {
         ArgumentValidator.ValidateNotNull( nameof( vendor ), vendor );

         this.vendor = vendor;
      }

      #region ObjectWithVendor Members

      public SQLVendor SQLVendor
      {
         get
         {
            return this.vendor;
         }
      }

      #endregion
   }

   public class CommonFactoryImpl : AbstractSQLFactoryImpl, CommonFactory
   {
      private readonly DirectLiteral _param;
      private readonly BooleanExpression _true;
      private readonly BooleanExpression _false;
      private readonly Predicate _empty;
      private readonly ValueExpression _default;
      private readonly ValueExpression _null;

      public CommonFactoryImpl( SQLVendorImpl vendor, DirectLiteral param = null, BooleanExpression trueExpression = null, BooleanExpression falseExpression = null, Predicate emptyBoolean = null, ValueExpression defaultExpr = null, ValueExpression nullExpr = null )
         : base( vendor )
      {
         this._param = param ?? new DirectLiteralImpl( vendor, SQLConstants.QUESTION_MARK );
         this._true = trueExpression ?? new BooleanExpressions.True( vendor );
         this._false = falseExpression ?? new BooleanExpressions.False( vendor );
         this._empty = emptyBoolean ?? new BooleanExpressions.EmptyPredicate( vendor );
         this._default = defaultExpr ?? new ValueSources.Default( vendor );
         this._null = nullExpr ?? new ValueSources.Null( vendor );
      }

      #region CommonFactory Members

      public virtual EqualsPredicate Eq( ValueExpression left, ValueExpression right )
      {
         return new EqualsPredicateImpl( this.vendor, left, right );
      }

      public virtual NotEqualsPredicate Neq( ValueExpression left, ValueExpression right )
      {
         return new NotEqualsPredicateImpl( this.vendor, left, right );
      }

      public virtual LessThanPredicate Lt( ValueExpression left, ValueExpression right )
      {
         return new LessThanPredicateImpl( this.vendor, left, right );
      }

      public virtual LessOrEqualToPredicate Leq( ValueExpression left, ValueExpression right )
      {
         return new LessOrEqualToPredicateImpl( this.vendor, left, right );
      }

      public virtual GreaterThanPredicate Gt( ValueExpression left, ValueExpression right )
      {
         return new GreaterThanPredicateImpl( this.vendor, left, right );
      }

      public virtual GreaterOrEqualToPredicate Geq( ValueExpression left, ValueExpression right )
      {
         return new GreaterOrEqualToPredicateImpl( this.vendor, left, right );
      }

      public virtual IsNullPredicate IsNull( ValueExpression what )
      {
         return new IsNullPredicateImpl( this.vendor, what );
      }

      public virtual Negation Not( BooleanExpression expr )
      {
         return new NegationImpl( this.vendor, expr );
      }

      public virtual Conjunction And( BooleanExpression left, BooleanExpression right )
      {
         return new ConjunctionImpl( this.vendor, left, right );
      }

      public virtual Disjunction Or( BooleanExpression left, BooleanExpression right )
      {
         return new DisjunctionImpl( this.vendor, left, right );
      }

      public virtual BetweenPredicate Between( ValueExpression left, ValueExpression minimum, ValueExpression maximum )
      {
         return new BetweenPredicateImpl( this.vendor, left, new ValueExpression[] { minimum, maximum }.NewAQ( false ) );
      }

      public virtual InPredicate In( ValueExpression what, ImmutableArray<ValueExpression> values )
      {
         return new InPredicateImpl( this.vendor, what, values );
      }

      public virtual LikePredicate Like( ValueExpression what, ValueExpression pattern )
      {
         return new LikePredicateImpl( this.vendor, what, pattern );
      }

      public virtual RegexpPredicate Regexp( ValueExpression what, ValueExpression pattern )
      {
         return new RegexpPredicateImpl( this.vendor, what, pattern );
      }

      public virtual ExistsPredicate Exists( QueryExpression query )
      {
         return new ExistsPredicateImpl( this.vendor, query );
      }

      public virtual UniquePredicate Unique( QueryExpression query )
      {
         return new UniquePredicateImpl( this.vendor, query );
      }

      public virtual BooleanTest Test( BooleanExpression expr, Boolean? truthValue )
      {
         return new BooleanTestImpl( this.vendor, expr, truthValue );
      }

      public virtual BooleanBuilder NewBooleanBuilder( BooleanExpression first = null )
      {
         return new BooleanBuilderImpl( this.vendor, first );
      }

      public virtual BinaryArithmeticExpression Plus( NonBooleanExpression left, NonBooleanExpression right )
      {
         return new BinaryArithmeticExpressionImpl( this.vendor, ArithmeticOperator.Plus, left, right );
      }

      public virtual BinaryArithmeticExpression Minus( NonBooleanExpression left, NonBooleanExpression right )
      {
         return new BinaryArithmeticExpressionImpl( this.vendor, ArithmeticOperator.Minus, left, right );
      }

      public virtual BinaryArithmeticExpression Mul( NonBooleanExpression left, NonBooleanExpression right )
      {
         return new BinaryArithmeticExpressionImpl( this.vendor, ArithmeticOperator.Multiplication, left, right );
      }

      public virtual BinaryArithmeticExpression Div( NonBooleanExpression left, NonBooleanExpression right )
      {
         return new BinaryArithmeticExpressionImpl( this.vendor, ArithmeticOperator.Division, left, right );
      }

      public virtual UnaryArithmeticExpression UnaryPlus( NonBooleanExpression expr )
      {
         return new UnaryArithmeticExpressionImpl( this.vendor, ArithmeticOperator.Plus, expr );
      }

      public virtual UnaryArithmeticExpression UnaryMinus( NonBooleanExpression expr )
      {
         return new UnaryArithmeticExpressionImpl( this.vendor, ArithmeticOperator.Minus, expr );
      }

      public virtual BooleanExpression True
      {
         get
         {
            return this._true;
         }
      }

      public virtual BooleanExpression False
      {
         get
         {
            return this._false;
         }
      }

      public virtual Predicate Empty
      {
         get
         {
            return this._empty;
         }
      }

      public virtual ValueExpression Default
      {
         get
         {
            return this._default;
         }
      }

      public virtual ValueExpression Null
      {
         get
         {
            return this._null;
         }
      }

      public virtual ColumnNameList ColumnNames( ImmutableArray<String> names )
      {
         return new ColumnNameListImpl( this.vendor, names );
      }

      public virtual TableNameDirect TableNameDirect( String schemaName, String tableName )
      {
         return new TableNameDirectImpl( this.vendor, schemaName, tableName );
      }

      public virtual StringLiteral String( String str )
      {
         return new StringLiteralImpl( this.vendor, str );
      }

      public virtual DirectLiteral Param
      {
         get
         {
            return this._param;
         }
      }

      public virtual DirectLiteral Literal( String contents )
      {
         return new DirectLiteralImpl( this.vendor, contents );
      }

      public virtual TimestampLiteral Timestamp( DateTime? dt )
      {
         return new TimestampLiteralImpl( this.vendor, dt );
      }

      public virtual Int32NumericLiteral I32( Int32? i32 )
      {
         return new Int32NumericLiteralImpl( this.vendor, i32 );
      }

      public virtual Int64NumericLiteral I64( Int64? i64 )
      {
         return new Int64NumericLiteralImpl( this.vendor, i64 );
      }

      public virtual DoubleNumericLiteral Double( Double? dbl )
      {
         return new DoubleNumericLiteralImpl( this.vendor, dbl );
      }

      public virtual DecimalNumericLiteral Decimal( Decimal? dec )
      {
         return new DecimalNumericLiteralImpl( this.vendor, dec );
      }

      public virtual SQLFunctionLiteral Function( String name, ImmutableArray<ValueExpression> parameters )
      {
         return new SQLFunctionLiteralImpl( this.vendor, name, parameters );
      }

      #endregion
   }

   public class DefinitionFactoryImpl : AbstractSQLFactoryImpl, DefinitionFactory
   {
      protected static readonly ImmutableArray<SchemaElement> EMPTY_SCHEMA_ELEMENTS = new SchemaElement[] { }.NewAQ();

      private readonly SQLDTBigInt _bigInt;
      private readonly SQLDTDoublePrecision _double;
      private readonly SQLDTReal _real;
      private readonly SQLDTSmallInt _smallInt;
      private readonly SQLDTBoolean _boolean;
      private readonly SQLDTDate _date;
      private readonly SQLDTInteger _int;
      private readonly SQLDTDecimal _plainDecimal;
      private readonly SQLDTNumeric _plainNumeric;
      private readonly SQLDTFloat _plainFloat;
      private readonly SQLDTChar _plainCharFixed;
      private readonly SQLDTChar _plainCharVarying;
      private readonly SQLDTTime _plainTime;
      private readonly SQLDTTime _plainTimeWithTZ;
      private readonly SQLDTTime _plainTimeWithoutTZ;
      private readonly SQLDTTimestamp _plainTS;
      private readonly SQLDTTimestamp _plainTSWithTZ;
      private readonly SQLDTTimestamp _plainTSWithoutTZ;

      public DefinitionFactoryImpl(
         SQLVendorImpl vendor,
         SQLDTBigInt bigInt = null,
         SQLDTDoublePrecision dbl = null,
         SQLDTReal real = null,
         SQLDTSmallInt smallInt = null,
         SQLDTBoolean bol = null,
         SQLDTDate date = null,
         SQLDTInteger sqlInt = null,
         SQLDTDecimal plainDecimal = null,
         SQLDTNumeric plainNumeric = null,
         SQLDTFloat plainFloat = null,
         SQLDTChar plainCharFixed = null,
         SQLDTChar plainCharVarying = null,
         SQLDTTime plainTime = null,
         SQLDTTime plainTimeWithTZ = null,
         SQLDTTime plainTimeWithoutTZ = null,
         SQLDTTimestamp plainTS = null,
         SQLDTTimestamp plainTSWithTZ = null,
         SQLDTTimestamp plainTSWithoutTZ = null
         )
         : base( vendor )
      {
         this._bigInt = bigInt ?? new SQLDTBigIntImpl( this.vendor );
         this._double = dbl ?? new SQLDTDoublePrecisionImpl( this.vendor );
         this._real = real ?? new SQLDTRealImpl( this.vendor );
         this._smallInt = smallInt ?? new SQLDTSmallIntImpl( this.vendor );
         this._boolean = bol ?? new SQLDTBooleanImpl( this.vendor );
         this._date = date ?? new SQLDTDateImpl( this.vendor );
         this._int = sqlInt ?? new SQLDTIntegerImpl( this.vendor );
         this._plainDecimal = plainDecimal ?? new SQLDTDecimalImpl( this.vendor, null, null );
         this._plainNumeric = plainNumeric ?? new SQLDTNumericImpl( this.vendor, null, null );
         this._plainFloat = plainFloat ?? new SQLDTFloatImpl( this.vendor, null );
         this._plainCharFixed = plainCharFixed ?? new SQLDTCharImpl( this.vendor, false, null );
         this._plainCharVarying = plainCharVarying ?? new SQLDTCharImpl( this.vendor, true, null );
         this._plainTime = plainTime ?? new SQLDTTimeImpl( this.vendor, null, null );
         this._plainTimeWithTZ = plainTimeWithTZ ?? new SQLDTTimeImpl( this.vendor, null, true );
         this._plainTimeWithoutTZ = plainTimeWithoutTZ ?? new SQLDTTimeImpl( this.vendor, null, false );
         this._plainTS = plainTS ?? new SQLDTTimeStampImpl( this.vendor, null, null );
         this._plainTSWithTZ = plainTSWithTZ ?? new SQLDTTimeStampImpl( this.vendor, null, true );
         this._plainTSWithoutTZ = plainTSWithoutTZ ?? new SQLDTTimeStampImpl( this.vendor, null, false );
      }

      #region DefinitionFactory Members

      public virtual SQLDTBigInt BigInt()
      {
         return this._bigInt;
      }

      public virtual SQLDTDecimal Decimal( Int32? precision = null, Int32? scale = null )
      {
         return precision.HasValue ? new SQLDTDecimalImpl( this.vendor, precision, scale ) : this._plainDecimal;
      }

      public virtual SQLDTDoublePrecision Double()
      {
         return this._double;
      }

      public virtual SQLDTNumeric Numeric( Int32? precision = null, Int32? scale = null )
      {
         return precision.HasValue ? new SQLDTNumericImpl( this.vendor, precision, scale ) : this._plainNumeric;
      }

      public virtual SQLDTReal Real()
      {
         return this._real;
      }

      public virtual SQLDTSmallInt SmallInt()
      {
         return this._smallInt;
      }

      public virtual SQLDTBoolean Boolean()
      {
         return this._boolean;
      }

      public virtual SQLDTChar Char( Boolean varying, Int32? length = null )
      {
         return length.HasValue ? new SQLDTCharImpl( this.vendor, varying, length ) : ( varying ? this._plainCharVarying : this._plainCharFixed );
      }

      public virtual SQLDTDate Date()
      {
         return this._date;
      }

      public virtual SQLDTFloat Float( Int32? precision = null )
      {
         return precision.HasValue ? new SQLDTFloatImpl( this.vendor, precision ) : this._plainFloat;
      }

      public virtual SQLDTInteger Integer()
      {
         return this._int;
      }

      public virtual SQLDTInterval YearMonthInterval( IntervalDataTypes startField, Int32? startFieldPrecision = null, IntervalDataTypes? endField = null )
      {
         if ( ( IntervalDataTypes.Year == startField || IntervalDataTypes.Month == startField )
            && ( !endField.HasValue || IntervalDataTypes.Year == endField.Value && IntervalDataTypes.Month == endField.Value ) )
         {
            return new SQLDTIntervalImpl( this.vendor, startField, startFieldPrecision, endField, null );
         }
         else
         {
            throw new ArgumentException( "Interval data types must be either " + IntervalDataTypes.Year + " or " + IntervalDataTypes.Month + "." );
         }
      }

      public virtual SQLDTInterval DayTimeInterval( IntervalDataTypes startField, Int32? startFieldPrcision = null, IntervalDataTypes? endField = null, Int32? secondFracs = null )
      {
         if ( IntervalDataTypes.Year != startField
            && IntervalDataTypes.Month != startField
            && ( !endField.HasValue || ( IntervalDataTypes.Year != endField.Value && IntervalDataTypes.Month != endField.Value && IntervalDataTypes.Second != startField ) )
            )
         {
            if ( secondFracs.HasValue && ( IntervalDataTypes.Second != startField || ( endField.HasValue && IntervalDataTypes.Second != endField.Value ) ) )
            {
               // Trying to set second fractionals, even when not needed
               secondFracs = null;
            }
            if ( !endField.HasValue && secondFracs.HasValue && !startFieldPrcision.HasValue )
            {
               throw new ArgumentException( "When specifying second fracs for single day-time intervals, the start field precision must be specified also." );
            }

            return new SQLDTIntervalImpl( this.vendor, startField, startFieldPrcision, endField, secondFracs );
         }
         else
         {
            throw new ArgumentException( "Interval data types must be either " + IntervalDataTypes.Day + ", " + IntervalDataTypes.Hour + ", " + IntervalDataTypes.Minute + ", or " + IntervalDataTypes.Second + ". For single day-time intervals, the start field must not be " + IntervalDataTypes.Second + " if end field is non-null." );
         }
      }

      public virtual SQLDTTime Time( Boolean? includeTimeZone = null, Int32? precision = null )
      {
         return precision.HasValue ? new SQLDTTimeImpl( this.vendor, precision, includeTimeZone ) : ( includeTimeZone.HasValue ? ( includeTimeZone.Value ? this._plainTimeWithTZ : this._plainTimeWithoutTZ ) : this._plainTime );
      }

      public virtual SQLDTTimestamp TimeStamp( Boolean? includeTimeZone = null, Int32? precision = null )
      {
         return precision.HasValue ? new SQLDTTimeStampImpl( this.vendor, precision, includeTimeZone ) : ( includeTimeZone.HasValue ? ( includeTimeZone.Value ? this._plainTSWithTZ : this._plainTSWithoutTZ ) : this._plainTS );
      }

      public virtual SQLDTUserDefined UserDefined( String textualContent )
      {
         return new SQLDTUserDefinedImpl( this.vendor, textualContent );
      }

      public virtual SchemaDefinitionBuilder NewSchemaDefinitionBuilder()
      {
         return new SchemaDefinitionBuilderImpl( this.vendor );
      }

      public virtual TableDefinitionBuilder NewTableDefinitionBuilder()
      {
         return new TableDefinitionBuilderImpl( this.vendor );
      }

      public virtual TableElementListBuilder NewTableElementListBuilder()
      {
         return new TableElementListBuilderImpl( this.vendor );
      }

      public virtual UniqueConstraintBuilder NewUniqueConstraintBuilder()
      {
         return new UniqueConstraintBuilderImpl( this.vendor );
      }

      public virtual ViewDefinitionBuilder NewViewDefinitionBuilder()
      {
         return new ViewDefinitionBuilderImpl( this.vendor );
      }

      public virtual ForeignKeyConstraintBuilder NewForeignKeyConstraintBuilder()
      {
         return new ForeignKeyConstraintBuilderImpl( this.vendor );
      }

      public virtual ColumnDefinition NewColumnDefinition( string columnName, SQLDataType columnDataType, Boolean mayBeNull = true, String defaultValue = null, AutoGenerationPolicy? autoGenerationPolicy = null )
      {
         return new ColumnDefinitionImpl( this.vendor, columnName, columnDataType, defaultValue, mayBeNull, autoGenerationPolicy );
      }

      public virtual LikeClause NewLikeClause( TableNameDirect tableName )
      {
         return new LikeClauseImpl( this.vendor, tableName );
      }

      public virtual TableConstraintDefinition NewTableConstraintDefinition( TableConstraint constraint, ConstraintCharacteristics? constraintChars = null, String name = null )
      {
         return new TableConstraintDefinitionImpl( this.vendor, name, constraintChars, constraint );
      }

      public virtual CheckConstraint NewCheckConstraint( BooleanExpression check )
      {
         return new CheckConstraintImpl( this.vendor, check );
      }

      public virtual RegularViewSpecification NewRegularViewSpecification( ImmutableArray<String> columnNames )
      {
         return new RegularViewSpecificationImpl( this.vendor, this.vendor.CommonFactory.ColumnNames( columnNames ) );
      }

      public virtual ForeignKeyConstraint NewForeignKeyConstraint( ImmutableArray<String> sourceColumns, TableNameDirect targetTable, ImmutableArray<String> targetColumns, MatchType? matchType = null, ReferentialAction? onUpdate = null, ReferentialAction? onDelete = null )
      {
         return new ForeignKeyConstraintImpl( this.vendor, this.vendor.CommonFactory.ColumnNames( sourceColumns ), targetTable, this.vendor.CommonFactory.ColumnNames( targetColumns ), matchType, onDelete, onUpdate );
      }

      public virtual SchemaDefinition NewSchemaDefinition( String schemaName, String schemaCharSet = null, ImmutableArray<SchemaElement> elements = null )
      {
         return new SchemaDefinitionImpl( this.vendor, schemaName, schemaCharSet, elements ?? EMPTY_SCHEMA_ELEMENTS );
      }

      public virtual TableDefinition NewTableDefinition( TableNameDirect tableName, TableContentsSource contents, TableScope? tableScope = null, TableCommitAction? commitAction = null )
      {
         return new TableDefinitionImpl( this.vendor, commitAction, contents, tableName, tableScope );
      }

      public virtual UniqueConstraint NewUniqueConstraint( UniqueSpecification uniquenessKind, ImmutableArray<String> columns )
      {
         return new UniqueConstraintImpl( this.vendor, this.vendor.CommonFactory.ColumnNames( columns ), uniquenessKind );
      }

      public virtual ViewDefinition NewViewDefinition( TableNameDirect viewName, QueryExpression query, ViewSpecification viewSpec, ViewCheckOption? viewCheck = null, bool isRecursive = false )
      {
         return new ViewDefinitionImpl( this.vendor, viewName, query, viewSpec, viewCheck, isRecursive );
      }

      public virtual TableElementList NewTableElementList( ImmutableArray<TableElement> elements )
      {
         return new TableElementListImpl( this.vendor, elements );
      }

      #endregion
   }

   public class ManipulationFactoryImpl : AbstractSQLFactoryImpl, ManipulationFactory
   {
      public sealed class DropDefault : SQLElementBase, AlterColumnAction
      {

         public DropDefault( SQLVendorImpl vendor )
            : base( vendor )
         {

         }
      }

      private readonly DropDefault _dropDefault;

      public ManipulationFactoryImpl( SQLVendorImpl vendor )
         : base( vendor )
      {
         this._dropDefault = new DropDefault( vendor );
      }

      #region ManipulationFactory Members

      public virtual AlterTableStatement NewAlterTableStatement( TableNameDirect tableName, AlterTableAction action )
      {
         return new AlterTableStatementImpl( this.vendor, tableName, action );
      }

      public virtual AddColumnDefinition NewAddColumnDefinition( ColumnDefinition definition )
      {
         return new AddColumnDefinitionImpl( this.vendor, definition );
      }

      public virtual AddTableConstraintDefinition NewAddTableConstraintDefinition( TableConstraintDefinition constraintDefinition )
      {
         return new AddTableConstraintDefinitionImpl( this.vendor, constraintDefinition );
      }

      public virtual AlterColumnDefinition NewAlterColumnDefinition( String columnName, AlterColumnAction action )
      {
         return new AlterColumnDefinitionImpl( this.vendor, columnName, action );
      }

      public virtual SetColumnDefault NewSetColumnDefault( String newDefault )
      {
         return new SetColumnDefaultImpl( this.vendor, newDefault );
      }

      public virtual DropColumnDefinition NewDropColumnDefinition( String columnName, DropBehaviour dropBehaviour )
      {
         return new DropColumnDefinitionImpl( this.vendor, dropBehaviour, columnName );
      }

      public virtual DropTableConstraintDefinition NewDropTableConstraintDefinition( String constraintName, DropBehaviour dropBehaviour )
      {
         return new DropTableConstraintDefinitionImpl( this.vendor, dropBehaviour, constraintName );
      }

      public virtual DropSchemaStatement NewDropSchemaStatement( String schemaName, DropBehaviour dropBehaviour )
      {
         return new DropSchemaStatementImpl( this.vendor, dropBehaviour, schemaName );
      }

      public virtual DropTableOrViewStatement NewDropTableOrViewStatement( TableNameDirect tableName, ObjectType theType, DropBehaviour dropBehaviour )
      {
         return new DropTableOrViewStatementImpl( this.vendor, dropBehaviour, theType, tableName );
      }

      public virtual AlterColumnAction AlterColumn_DropDefault()
      {
         return this._dropDefault;
      }

      #endregion
   }

   public class ModificationFactoryImpl : AbstractSQLFactoryImpl, ModificationFactory
   {
      private readonly ColumnSource _defaults;

      public ModificationFactoryImpl( SQLVendorImpl vendor, ColumnSource defaults = null )
         : base( vendor )
      {
         this._defaults = defaults ?? new ColumnSources.Defaults( vendor );
      }

      #region ModificationFactory Members

      public virtual ColumnSourceByValuesBuilder NewColumnSourceByValuesBuilder()
      {
         return new ColumnSourceByValuesBuilderImpl( this.vendor );
      }

      public virtual DeleteBySearchBuilder NewDeleteBySearchBuilder()
      {
         return new DeleteBySearchBuilderImpl( this.vendor );
      }

      public virtual InsertStatementBuilder NewInsertStatementBuilder()
      {
         return new InsertStatementBuilderImpl( this.vendor );
      }

      public virtual UpdateBySearchBuilder NewUpdateBySearchBuilder()
      {
         return new UpdateBySearchBuilderImpl( this.vendor );
      }

      public virtual ColumnSourceByQuery NewColumnSourceByQuery( QueryExpression query, ImmutableArray<String> columns = null )
      {
         return new ColumnSourceByQueryImpl( this.vendor, columns == null ? null : this.vendor.CommonFactory.ColumnNames( columns ), query );
      }

      public virtual TargetTable NewTargetTable( TableNameDirect tableName, bool isOnly = false )
      {
         return new TargetTableImpl( this.vendor, tableName, isOnly );
      }

      public virtual UpdateSourceByExpression NewUpdateSourceByExpression( ValueExpression expression )
      {
         return new UpdateSourceByExpressionImpl( this.vendor, expression );
      }

      public virtual SetClause NewSetClause( String updateTarget, UpdateSource updateSource )
      {
         return new SetClauseImpl( this.vendor, updateTarget, updateSource );
      }

      public virtual ColumnSourceByValues NewColumnSourceByValues( ImmutableArray<ImmutableArray<ValueExpression>> values, ColumnNameList names = null )
      {
         return new ColumnSourceByValuesImpl( this.vendor, names, values );
      }

      public virtual DeleteBySearch NewDeleteBySearch( TargetTable targetTable, BooleanExpression condition = null )
      {
         return new DeleteBySearchImpl( this.vendor, targetTable, condition );
      }

      public virtual InsertStatement NewInsertStatement( TableNameDirect table, ColumnSource columnSource )
      {
         return new InsertStatementImpl( this.vendor, table, columnSource );
      }

      public virtual UpdateBySearch NewUpdateBySearch( TargetTable target, ImmutableArray<SetClause> setClauses, BooleanExpression condition = null )
      {
         return new UpdateBySearchImpl( this.vendor, target, condition, setClauses );
      }

      public ColumnSource Defaults
      {
         get
         {
            return this._defaults;
         }
      }

      #endregion
   }

   public class QueryFactoryImpl : AbstractSQLFactoryImpl, QueryFactory
   {
      private readonly QueryExpressionBody _empty;
      private readonly GroupingElement _grandTotal;

      public QueryFactoryImpl( SQLVendorImpl vendor, QueryExpressionBody emptyQuery = null, GroupingElement grandTotal = null )
         : base( vendor )
      {
         this._empty = emptyQuery ?? new QueryExpressionBodies.EmptyExpressionBody( vendor );
         this._grandTotal = grandTotal ?? new GroupingElements.GrandTotal( vendor );
      }

      #region QueryFactory Members

      public virtual QuerySpecificationBuilder NewQuerySpecificationBuilder()
      {
         return new QuerySpecificationBuilderImpl(
            this.vendor,
            this.NewSelectBuilder(),
            this.NewFromBuilder(),
            this.vendor.CommonFactory.NewBooleanBuilder(),
            this.NewGroupByBuilder(),
            this.vendor.CommonFactory.NewBooleanBuilder(),
            this.NewOrderByBuilder()
            );
      }

      public virtual QueryExpressionBodyBuilder NewQueryExpressionBodyBuilder( QueryExpressionBody current = null )
      {
         return new QueryExpressionBodyBuilderImpl( this.vendor, current );
      }

      public virtual QueryExpressionBody Empty
      {
         get
         {
            return this._empty;
         }
      }

      public virtual GroupingElement GrandTotal
      {
         get
         {
            return this._grandTotal;
         }
      }

      public virtual JoinedTableBuilder NewJoinedTableBuilder( TableReferencePrimary startingTable )
      {
         return new JoinedTableBuilderImpl( this.vendor, startingTable );
      }

      public virtual ColumnReferenceByExpression ColumnExpression( ValueExpression expression )
      {
         return new ColumnReferenceByExpressionImpl( this.vendor, expression );
      }

      public virtual ColumnReferenceByName ColumnName( String tableName, String columnName )
      {
         return new ColumnReferenceByNameImpl( this.vendor, tableName, columnName );
      }

      public virtual QueryExpression NewQuery( QueryExpressionBody body )
      {
         return new QueryExpressionImpl( this.vendor, body );
      }

      public virtual OrdinaryGroupingSet NewGroupingElement( ImmutableArray<NonBooleanExpression> expressions )
      {
         return new OrdinaryGroupingSetImpl( this.vendor, expressions );
      }

      public virtual SortSpecification NewSortSpecification( ValueExpression expression, Ordering ordering )
      {
         return new SortSpecificationImpl( this.vendor, ordering, expression );
      }

      public virtual TableValueConstructor NewTableValueConstructor( ImmutableArray<RowValueConstructor> rows )
      {
         return new TableValueConstructorImpl( this.vendor, rows );
      }

      public virtual RowSubQuery NewRowSubQuery( QueryExpression subQuery )
      {
         return new RowSubQueryImpl( this.vendor, subQuery );
      }

      public virtual RowDefinition NewRow( ImmutableArray<ValueExpression> elements )
      {
         return new RowDefinitionImpl( this.vendor, elements );
      }

      public virtual TableNameFunction NewTableNameFunction( SQLFunctionLiteral function, String schemaName = null )
      {
         return new TableNameFunctionImpl( this.vendor, schemaName, function );
      }

      public virtual TableReferenceByName NewTableReferenceByName( TableName tableName, TableAlias alias = null )
      {
         return new TableReferenceByNameImpl( this.vendor, alias, tableName );
      }

      public virtual TableAlias NewTableAlias( String tableNameAlias, ImmutableArray<String> renamedColumns )
      {
         return new TableAliasImpl( this.vendor, tableNameAlias, renamedColumns != null && renamedColumns.Length > 0 ? this.vendor.CommonFactory.ColumnNames( renamedColumns ) : null );
      }

      public virtual TableReferenceByQuery NewTableReferenceByQuery( QueryExpression query, TableAlias alias = null )
      {
         return new TableReferenceByQueryImpl( this.vendor, alias, query );
      }

      public virtual JoinCondition NewJoinCondition( BooleanExpression condition )
      {
         return new JoinConditionImpl( this.vendor, condition );
      }

      public virtual NamedColumnsJoin NewNamedColumnsJoin( ImmutableArray<String> columnNames )
      {
         return new NamedColumnsJoinImpl( this.vendor, this.vendor.CommonFactory.ColumnNames( columnNames ) );
      }

      public virtual AsteriskSelect NewSelectAllClause( SetQuantifier quantifier )
      {
         return new AsteriskSelectImpl( this.vendor, quantifier );
      }

      public virtual ColumnReferences NewSelectClause( SetQuantifier quantifier, ImmutableArray<ColumnReferenceInfo> cols )
      {
         return new ColumnReferencesImpl( this.vendor, quantifier, cols );
      }

      public virtual FromClause NewFromClause( ImmutableArray<TableReference> tableRefs )
      {
         return new FromClauseImpl( this.vendor, tableRefs );
      }

      public virtual QualifiedJoinedTable NewQualifiedJoinedTable( TableReference left, TableReference right, JoinSpecification joinSpec, JoinType jType = JoinType.Inner )
      {
         return new QualifiedJoinedTableImpl( this.vendor, left, right, jType, joinSpec );
      }

      public virtual CrossJoinedTable NewCrossJoinedTable( TableReference left, TableReference right )
      {
         return new CrossJoinedTableImpl( this.vendor, left, right );
      }

      public virtual NaturalJoinedTable NewNaturalJoinedTable( TableReference left, TableReference right, JoinType jType = JoinType.Inner )
      {
         return new NaturalJoinedTableImpl( this.vendor, left, right, jType );
      }

      public virtual UnionJoinedTable NewUnionJoinedTable( TableReference left, TableReference right )
      {
         return new UnionJoinedTableImpl( this.vendor, left, right );
      }

      public virtual GroupByClause NewGroupByClause( ImmutableArray<GroupingElement> elements )
      {
         return new GroupByClauseImpl( this.vendor, elements );
      }

      public virtual OrderByClause NewOrderByClause( ImmutableArray<SortSpecification> sortSpecs )
      {
         return new OrderByClauseImpl( this.vendor, sortSpecs );
      }

      public virtual QueryExpressionBodyBinary NewBinaryQuery( QueryExpressionBody left, QueryExpressionBody right, SetOperations setOperation, SetQuantifier quantifier = SetQuantifier.Distinct, CorrespondingSpec correspondingSpec = null )
      {
         return new QueryExpressionBodyBinaryImpl( this.vendor, setOperation, correspondingSpec, left, right, quantifier );
      }

      public virtual QuerySpecification NewQuerySpecification( SelectColumnClause select, FromClause from = null, BooleanExpression where = null, GroupByClause groupBy = null, BooleanExpression having = null, OrderByClause orderBy = null, NonBooleanExpression offset = null, NonBooleanExpression limit = null )
      {
         return new QuerySpecificationImpl( this.vendor, select, from, where, groupBy, having, orderBy, offset, limit );
      }

      #endregion

      protected virtual SelectColumnClauseBuilderImpl NewSelectBuilder()
      {
         return new SelectColumnClauseBuilderImpl( this.vendor );
      }

      protected virtual FromBuilderImpl NewFromBuilder()
      {
         return new FromBuilderImpl( this.vendor );
      }

      protected virtual GroupByBuilderImpl NewGroupByBuilder()
      {
         return new GroupByBuilderImpl( this.vendor );
      }

      protected virtual OrderByBuilderImpl NewOrderByBuilder()
      {
         return new OrderByBuilderImpl( this.vendor );
      }
   }
}
