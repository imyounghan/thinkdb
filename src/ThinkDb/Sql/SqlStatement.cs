﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ThinkDb.Sql
{
    /// <summary>
    /// An SqlStatement is a literal SQL request, composed of different parts (SqlPart)
    /// each part being either a parameter or a literal string
    /// </summary>
    public class SqlStatement : IEnumerable<SqlPart>
    {
        private readonly List<SqlPart> parts = new List<SqlPart>();

        /// <summary>
        /// Empty SqlStatement, used to build new statements
        /// </summary>
        public static readonly SqlStatement Empty = new SqlStatement();

        /// <summary>
        /// Returns the number of parts present
        /// </summary>
        public int Count { get { return parts.Count; } }

        /// <summary>
        /// Enumerates all parts
        /// </summary>
        public IEnumerator<SqlPart> GetEnumerator()
        {
            return parts.GetEnumerator();
        }

        /// <summary>
        /// Enumerates all parts
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns part at given index
        /// </summary>
        public SqlPart this[int index]
        {
            get { return parts[index]; }
        }

        /// <summary>
        /// Combines all parts, in correct order
        /// </summary>
        public override string ToString()
        {
            return string.Join(string.Empty, (from part in parts select part.Sql).ToArray());
        }

        /// <summary>
        /// Joins SqlStatements into a new SqlStatement
        /// </summary>
        public static SqlStatement Join(SqlStatement sqlStatement, IList<SqlStatement> sqlStatements)
        {
            // optimization: if we have only one statement to join, we return the statement itself
            if (sqlStatements.Count == 1)
                return sqlStatements[0];
            var builder = new SqlStatementBuilder();
            builder.AppendJoin(sqlStatement, sqlStatements);
            return builder.ToSqlStatement();
        }

        /// <summary>
        /// Joins SqlStatements into a new SqlStatement
        /// </summary>
        public static SqlStatement Join(SqlStatement sqlStatement, params SqlStatement[] sqlStatements)
        {
            return Join(sqlStatement, (IList<SqlStatement>)sqlStatements);
        }

        /// <summary>
        /// Formats an SqlStatement
        /// </summary>
        public static SqlStatement Format(string format, IList<SqlStatement> sqlStatements)
        {
            var builder = new SqlStatementBuilder();
            builder.AppendFormat(format, sqlStatements);
            return builder.ToSqlStatement();
        }

        /// <summary>
        /// Formats the specified text.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="sqlStatements">The SQL statements.</param>
        public static SqlStatement Format(string format, params SqlStatement[] sqlStatements)
        {
            return Format(format, (IList<SqlStatement>)sqlStatements);
        }

        /// <summary>
        /// Replaces all text occurrences in the SqlStatement
        /// </summary>
        public SqlStatement Replace(string find, string replace, bool ignoreCase)
        {
            var builder = new SqlStatementBuilder(this);
            builder.Replace(find, replace, ignoreCase);
            return builder.ToSqlStatement();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStatement"/> class.
        /// </summary>
        public SqlStatement()
        {
        }

        /// <summary>
        /// Builds an SqlStatement by concatenating several statements
        /// </summary>
        public SqlStatement(IEnumerable<SqlStatement> sqlStatements)
        {
            foreach (var sqlStatement in sqlStatements) {
                parts.AddRange(sqlStatement.parts);
            }
        }

        /// <summary>
        /// Builds SqlStatement
        /// </summary>
        public SqlStatement(params SqlStatement[] sqlStatements)
            : this((IEnumerable<SqlStatement>)sqlStatements)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStatement"/> class.
        /// </summary>
        /// <param name="sqlParts">The SQL parts.</param>
        public SqlStatement(params SqlPart[] sqlParts)
            : this((IList<SqlPart>)sqlParts)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStatement"/> class.
        /// </summary>
        /// <param name="sqlParts">The SQL parts.</param>
        public SqlStatement(IEnumerable<SqlPart> sqlParts)
        {
            foreach (var sqlPart in sqlParts)
                SqlStatementBuilder.AddPart(parts, sqlPart);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStatement"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public SqlStatement(string sql)
        {
            parts.Add(new SqlLiteralPart(sql));
        }

        /// <summary>
        /// Converts a string to an SqlStatement
        /// </summary>
        public static implicit operator SqlStatement(string sql)
        {
            return new SqlStatement(sql);
        }
    }
}
