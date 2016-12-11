using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThinkDb.Sql;

namespace ThinkDb.Dialect
{
    public class MySqlSqlProvider : SqlProvider
    {
        public override string GetParameterName(string nameBase)
        {
            return string.Format("?{0}", nameBase);
        }

        protected override SqlStatement GetLiteralCount(SqlStatement a)
        {
            return "COUNT(*)";
        }

        protected override SqlStatement GetLiteralStringConcat(SqlStatement a, SqlStatement b)
        {
            return SqlStatement.Format("CONCAT({0}, {1})", a, b);
        }

        public virtual string GetBulkInsert(string table, IList<string> columns, IList<IList<string>> valuesLists)
        {
            if (columns.Count == 0)
                return string.Empty;

            var insertBuilder = new StringBuilder("INSERT INTO ");
            insertBuilder.Append(table);
            insertBuilder.AppendFormat(" ({0})", string.Join(", ", columns.ToArray()));
            insertBuilder.Append(" VALUES ");
            var literalValuesLists = new List<string>();
            foreach (var values in valuesLists)
                literalValuesLists.Add(string.Format("({0})", string.Join(", ", values.ToArray())));
            insertBuilder.Append(string.Join(", ", literalValuesLists.ToArray()));
            return insertBuilder.ToString();
        }

        protected override char SafeNameStartQuote { get { return '`'; } }
        protected override char SafeNameEndQuote { get { return '`'; } }
    }
}
