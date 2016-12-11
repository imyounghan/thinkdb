using System;
using System.Collections;
using System.Collections.Generic;

namespace ThinkDb.Sql
{
    /// <summary>
    /// An SqlStatement is a literal SQL request, composed of different parts (SqlPart)
    /// each part being either a parameter or a literal string
    /// </summary>
    public class SqlStatementBuilder
    {
        public readonly List<SqlPart> Parts = new List<SqlPart>();

        /// <summary>
        /// Returns part at given index
        /// </summary>
        public SqlPart this[int index]
        {
            get { return Parts[index]; }
        }

        /// <summary>
        /// Creates a new SqlStatement based on the current and appending new SqlParts
        /// </summary>
        public void Append(IEnumerable<SqlPart> newParts)
        {
            foreach (var part in newParts)
                AddPart(Parts, part);
        }

        /// <summary>
        /// Appends a single part, including (useless) optimizations
        /// </summary>
        public static void InsertPart(IList<SqlPart> parts, int index, SqlPart part)
        {
            // optimization if top part is a literal, and the one we're adding is a literal too
            // in this case, we combine both
            // (this is useless, just pretty)
            if (part is SqlLiteralPart && index > 0 && parts[index - 1] is SqlLiteralPart) {
                parts[index - 1] = new SqlLiteralPart(parts[index - 1].Sql + part.Sql);
            }
            else
                parts.Insert(index, part);
        }

        /// <summary>
        /// Adds the part to the given parts list.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <param name="part">The part.</param>
        public static void AddPart(IList<SqlPart> parts, SqlPart part)
        {
            InsertPart(parts, parts.Count, part);
        }

        /// <summary>
        /// Joins statements, separated by a given statement
        /// </summary>
        public void AppendJoin(SqlStatement sqlStatement, IList<SqlStatement> sqlStatements)
        {
            for (int index = 0; index < sqlStatements.Count; index++) {
                if (index > 0)
                    Append(sqlStatement);
                Append(sqlStatements[index]);
            }
        }

        /// <summary>
        /// Creates an SQL statement based on a format string and SqlStatements as arguments
        /// </summary>
        public void AppendFormat(string format, IList<SqlStatement> sqlStatements)
        {
            var statements = new ArrayList { format };
            // the strategy divides each part containing the {0}, {1}, etc
            // and inserts the required argument
            for (int index = 0; index < sqlStatements.Count; index++) {
                var newStatements = new ArrayList();
                var literalIndex = "{" + index + "}";
                // then in each statement we look for the current literalIndex
                foreach (var statement in statements) {
                    // if we have a string, we split it around the literalIndex
                    // and insert the SqlStatement between new parts
                    var stringStatement = statement as string;
                    if (stringStatement != null) {
                        var parts = stringStatement.Split(new[] { literalIndex }, StringSplitOptions.None);
                        for (int partIndex = 0; partIndex < parts.Length; partIndex++) {
                            if (partIndex > 0)
                                newStatements.Add(sqlStatements[index]);
                            newStatements.Add(parts[partIndex]);
                        }
                    }
                    else // no match found? add the raw statement
                        newStatements.Add(statement);
                }
                statements = newStatements;
            }
            // finally, convert all remaining strings to SqlStatements
            foreach (var statement in statements) {
                var stringStatement = statement as string;
                if (stringStatement != null)
                    Append(new SqlStatement(stringStatement));
                else
                    Append((SqlStatement)statement);
            }
        }

        /// <summary>
        /// Formats an SqlStatement from a given string format
        /// </summary>
        public void AppendFormat(string format, params SqlStatement[] sqlStatements)
        {
            AppendFormat(format, (IList<SqlStatement>)sqlStatements);
        }

        /// <summary>
        /// Appends a bunch of sqlStatements to the current one
        /// </summary>
        public void Append(IEnumerable<SqlStatement> sqlStatements)
        {
            foreach (var sqlStatement in sqlStatements) {
                foreach (var sqlPart in sqlStatement) {
                    AddPart(Parts, sqlPart);
                }
            }
        }

        /// <summary>
        /// Appends sqlStatements to the current one
        /// </summary>
        public void Append(params SqlStatement[] newStatements)
        {
            Append((IEnumerable<SqlStatement>)newStatements);
        }

        /// <summary>
        /// Replaces the specified text, optionally ignoring the case.
        /// The method does not replace cross-parts text
        /// </summary>
        /// <param name="oldText">The old text.</param>
        /// <param name="newText">The new text.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        public void Replace(string oldText, string newText, bool ignoreCase)
        {
            for (int partIndex = 0; partIndex < Parts.Count; partIndex++) {
                var part = Parts[partIndex];
                if (part.Sql.ContainsCase(oldText, ignoreCase)) {
                    // we know how to process only on literal strings
                    if (part is SqlLiteralPart) {
                        Parts[partIndex] = new SqlLiteralPart(part.Sql.ReplaceCase(oldText, newText, ignoreCase));
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStatementBuilder"/> class.
        /// </summary>
        public SqlStatementBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStatementBuilder"/> class.
        /// </summary>
        /// <param name="sqlStatements">The SQL statements.</param>
        public SqlStatementBuilder(params SqlStatement[] sqlStatements)
        {
            Append(sqlStatements);
        }

        /// <summary>
        /// Gets the built SqlStatement.
        /// </summary>
        public SqlStatement ToSqlStatement()
        {
            return new SqlStatement(Parts);
        }
    }
}
