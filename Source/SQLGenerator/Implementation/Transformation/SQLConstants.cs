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
   public static class SQLConstants
   {
      public const String SELECT = "SELECT";

      public const String FROM = "FROM";

      public const String WHERE = "WHERE";

      public const String GROUP_BY = "GROUP BY";

      public const String HAVING = "HAVING";

      public const String ORDER_BY = "ORDER BY";

      public const String TABLE_COLUMN_SEPARATOR = ".";

      public const String SCHEMA_TABLE_SEPARATOR = ".";

      public const String TOKEN_SEPARATOR = " ";

      public const String AND = "AND";

      public const String OR = "OR";

      public const String NOT = "NOT";

      public const String ASTERISK = "*";

      public const String COMMA = ",";

      public const String PERIOD = ".";

      public const String QUESTION_MARK = "?";

      public const String OPEN_PARENTHESIS = "(";

      public const String CLOSE_PARENTHESIS = ")";

      public const String ALIAS_DEFINER = "AS";

      public const String NEWLINE = "\n";

      public const String NULL = "NULL";

      public const String IS = "IS";

      public const String CREATE = "CREATE ";

      public const String OFFSET_PREFIX = "OFFSET";

      public const String OFFSET_POSTFIX = "ROWS";

      public const String LIMIT_PREFIX = "FETCH FIRST";

      public const String LIMIT_POSTFIX = "ROWS ONLY";

      public const String TRUE = "TRUE";

      public const String FALSE = "FALSE";

      public const String UNKNOWN = "UNKNOWN";

      public const String DECIMAL = "DECIMAL";

      public const String NUMERIC = "NUMERIC";

      public const String CHARACTER = "CHARACTER";

      public const String VARYING = "VARYING";

      public const String FLOAT = "FLOAT";

      public const String INTERVAL = "INTERVAL";

      public const String TO = "TO";

      public const String TIME = "TIME";

      public const String TIMESTAMP = "TIMESTAMP";

      public const String WITH = "WITH ";

      public const String WITHOUT = "WITHOUT ";

      public const String TIMEZONE = "TIME ZONE";

      public const String SCHEMA = "SCHEMA ";

      public const String TABLE = "TABLE ";

      public const String VIEW = "VIEW ";

      public const String DROP = "DROP ";
   }
}
