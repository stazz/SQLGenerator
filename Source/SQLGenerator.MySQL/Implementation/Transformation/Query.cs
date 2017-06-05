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
using SQLGenerator.Implementation.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGenerator.MySQL.Implementation.Transformation
{
   internal class MySQLQuerySpecificationProcessor : QuerySpecificationProcessor
   {
      private const String MYSQL_LIMIT_PREFIX = "LIMIT";
      private const String MYSQL_OFFSET_PREFIX = "OFFSET";

      protected override void ProcessOffsetAndLimit( SQLProcessorAggregator aggregator, NonBooleanExpression offset, NonBooleanExpression limit, StringBuilder builder )
      {
         if ( ( (MySQLVendor) aggregator.SQLVendor ).LegacyLimitSyntax )
         {
            // LIMIT X(,Y)
            builder.Append( SQLConstants.NEWLINE + MYSQL_LIMIT_PREFIX + SQLConstants.TOKEN_SEPARATOR );
            if ( offset != null )
            {
               aggregator.Process( offset, builder );
               builder.Append( SQLConstants.COMMA );
            }
            if ( limit != null )
            {
               aggregator.Process( limit, builder );
            }
            else if ( offset != null )
            {
               builder.Append( Int64.MaxValue );
            }
         }
         else
         {
            if ( limit == null )
            {
               limit = aggregator.SQLVendor.CommonFactory.I64( Int64.MaxValue );
            }
            base.ProcessOffsetAndLimit( aggregator, offset, limit, builder );
         }
      }

      protected override String GetOffsetPrefix( SQLVendor vendor )
      {
         return MYSQL_OFFSET_PREFIX;
      }

      protected override String GetOffsetPostfix( SQLVendor vendor )
      {
         return null;
      }

      protected override String GetLimitPrefix( SQLVendor vendor )
      {
         return MYSQL_LIMIT_PREFIX;
      }

      protected override String GetLimitPostfix( SQLVendor vendor )
      {
         return null;
      }

      protected override Boolean IsOffsetBeforeLimit( SQLVendor vendor )
      {
         return false;
      }
   }
}
