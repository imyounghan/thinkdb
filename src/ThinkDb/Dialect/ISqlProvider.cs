using System.Collections.Generic;
using System.Linq.Expressions;
using ThinkDb.Sql;
using ThinkDb.Expressions;

namespace ThinkDb.Dialect
{
    public interface ISqlProvider
    {
        /// <summary>
        /// String values and function parameter first character index is 1
        /// </summary>
        bool StringIndexStartsAtOne { get; }

        /// <summary>
        /// Gets the new line string.
        /// </summary>
        string NewLine { get; }

        /// <summary>
        /// Converts a constant value to a literal representation
        /// </summary>
        SqlStatement GetLiteral(object literal);

        /// <summary>
        /// Converts a standard operator to an expression
        /// </summary>
        SqlStatement GetLiteral(ExpressionType operationType, IList<SqlStatement> p);

        /// <summary>
        /// Converts a special expression type to literal
        /// </summary>
        SqlStatement GetLiteral(SpecialExpressionType operationType, IList<SqlStatement> p);

        /// <summary>
        /// Places the expression into parenthesis
        /// </summary>
        SqlStatement GetParenthesis(SqlStatement a);

        /// <summary>
        /// Returns a column related to a table.
        /// Ensures about the right case
        /// </summary>
        string GetColumn(string table, string column);

        /// <summary>
        /// Returns a column related to a table.
        /// Ensures about the right case
        /// </summary>
        string GetColumn(string column);

        /// <summary>
        /// Returns a table alias
        /// Ensures about the right case
        /// </summary>
        string GetTableAsAlias(string table, string alias);

        /// <summary>
        /// Returns a subquery alias
        /// Ensures about the right case
        /// </summary>
        string GetSubQueryAsAlias(string table, string alias);

        /// <summary>
        /// Returns a table alias
        /// </summary>
        string GetTable(string table);

        /// <summary>
        /// Returns a literal parameter name
        /// </summary>
        string GetParameterName(string nameBase);

        
        /// <summary>
        /// Joins a list of table selection to make a FROM clause
        /// </summary>
        SqlStatement GetFromClause(SqlStatement[] tables);

        /// <summary>
        /// Joins a list of conditions to make a WHERE clause
        /// </summary>
        SqlStatement GetWhereClause(SqlStatement[] wheres);

        /// <summary>
        /// Returns a valid alias syntax for the given table
        /// </summary>
        string GetTableAlias(string nameBase);

        /// <summary>
        /// Joins a list of operands to make a SELECT clause
        /// </summary>
        SqlStatement GetSelectClause(SqlStatement[] selects);

        /// <summary>
        /// Joins a list of operands to make a SELECT clause
        /// </summary>
        SqlStatement GetSelectDistinctClause(SqlStatement[] selects);

        /// <summary>
        /// Returns all table columns (*)
        /// </summary>
        string GetColumns();

        SqlStatement GetColumnAsAlias(string column, string alias);

        /// <summary>
        /// Returns a LIMIT clause around a SELECT clause
        /// </summary>
        /// <param name="select">SELECT clause</param>
        /// <param name="limit">limit value (number of columns to be returned)</param>
        SqlStatement GetLiteralLimit(SqlStatement select, SqlStatement limit);

        /// <summary>
        /// Returns a LIMIT clause around a SELECT clause, with offset
        /// </summary>
        /// <param name="select">SELECT clause</param>
        /// <param name="limit">limit value (number of columns to be returned)</param>
        /// <param name="offset">first row to be returned (starting from 0)</param>
        /// <param name="offsetAndLimit">limit+offset</param>
        SqlStatement GetLiteralLimit(SqlStatement select, SqlStatement limit, SqlStatement offset, SqlStatement offsetAndLimit);

        /// <summary>
        /// Returns an ORDER criterium
        /// </summary>
        SqlStatement GetOrderByColumn(SqlStatement expression, bool descending);

        /// <summary>
        /// Joins a list of conditions to make a ORDER BY clause
        /// </summary>
        SqlStatement GetOrderByClause(SqlStatement[] orderBy);

        /// <summary>
        /// Joins a list of conditions to make a GROUP BY clause
        /// </summary>
        SqlStatement GetGroupByClause(SqlStatement[] groupBy);

        /// <summary>
        /// Joins a list of conditions to make a HAVING clause
        /// </summary>
        SqlStatement GetHavingClause(SqlStatement[] havings);

        /// <summary>
        /// Returns an operation between two SELECT clauses (UNION, UNION ALL, etc.)
        /// </summary>
        SqlStatement GetLiteral(SelectOperatorType selectOperator, SqlStatement selectA, SqlStatement selectB);

        /// <summary>
        /// Builds an insert clause
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="inputColumns">Columns to be inserted</param>
        /// <param name="inputValues">Values to be inserted into columns</param>
        SqlStatement GetInsert(SqlStatement table, IList<SqlStatement> inputColumns, IList<SqlStatement> inputValues);

        /// <summary>
        /// Builds the statements that gets back the IDs for the inserted statement
        /// </summary>
        /// <param name="outputParameters">Expected output parameters</param>
        /// <param name="outputExpressions">Expressions (to help generate output parameters)</param>
        SqlStatement GetInsertIds(SqlStatement table, IList<SqlStatement> autoPKColumn, IList<SqlStatement> inputPKColumns, IList<SqlStatement> inputPKValues, IList<SqlStatement> outputColumns, IList<SqlStatement> outputParameters, IList<SqlStatement> outputExpressions);

        /// <summary>
        /// Builds an update clause
        /// </summary>
        /// <param name="table"></param>
        /// <param name="inputColumns">Columns to be inserted</param>
        /// <param name="inputValues">Values to be inserted into columns</param>
        /// <param name="outputParameters">Expected output parameters</param>
        /// <param name="outputExpressions">Expressions (to help generate output parameters)</param>
        /// <param name="inputPKColumns">PK columns for reference</param>
        /// <param name="inputPKValues">PK values for reference</param>
        SqlStatement GetUpdate(SqlStatement table, IList<SqlStatement> inputColumns, IList<SqlStatement> inputValues,
                                         IList<SqlStatement> outputParameters, IList<SqlStatement> outputExpressions,
                                         IList<SqlStatement> inputPKColumns, IList<SqlStatement> inputPKValues);

        /// <summary>
        /// Builds a delete clause
        /// </summary>
        /// <param name="table"></param>
        /// <param name="inputPKColumns">PK columns for reference</param>
        /// <param name="inputPKValues">PK values for reference</param>
        SqlStatement GetDelete(SqlStatement table, IList<SqlStatement> inputPKColumns, IList<SqlStatement> inputPKValues);

        /// <summary>
        /// given 'User', return '[User]' to prevent a SQL keyword conflict
        /// </summary>
        string GetSafeName(string name);

        /// <summary>
        /// Returns a case safe query, converting quoted names &lt;&ltMixedCaseName>> to "MixedCaseName"
        /// </summary>
        string GetSafeQuery(string sqlString);

        ///<summary>
        ///Returns a SqlStatement with a conversion of an expression(value) to a type(newType)
        ///</summary>
        /// <example>
        /// In sqlServer: 
        /// value= OrderDetail.Quantity
        /// newType= boolean
        /// 
        /// it should return CONVERT(bit,OrderDetail.Quantity)
        /// </example>
        SqlStatement GetLiteralConvert(SqlStatement value, System.Type newType);

        /// <summary>
        /// Returns an INNER JOIN syntax
        /// </summary>
        SqlStatement GetInnerJoinClause(SqlStatement joinedTable, SqlStatement joinExpression);

        /// <summary>
        /// Returns a LEFT JOIN syntax
        /// </summary>
        SqlStatement GetLeftOuterJoinClause(SqlStatement joinedTable, SqlStatement joinExpression);

        /// <summary>
        /// Returns a RIGHT JOIN syntax
        /// </summary>
        SqlStatement GetRightOuterJoinClause(SqlStatement joinedTable, SqlStatement joinExpression);

        /// <summary>
        /// Concatenates all join clauses
        /// </summary>
        SqlStatement GetJoinClauses(SqlStatement[] joins);
    }
}
