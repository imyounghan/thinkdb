using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using ThinkDb.Dialect;
using ThinkDb.Sql;

namespace ThinkDb.Expressions
{
    public class OrderExpression : Expression
    {
        public SortOrder SortOrder { get; private set; }

        public string Column { get; private set; }

        public OrderExpression(string column, SortOrder sortOrder)
            : base((ExpressionType)1030)
        {
            this.Column = column;
            this.SortOrder = sortOrder;
        }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            return new SqlStatement[] { sqlProvider.GetOrderByColumn(new SqlStatement(Column), SortOrder == System.Data.SqlClient.SortOrder.Descending) };
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Column);
            if (SortOrder == System.Data.SqlClient.SortOrder.Descending) {
                sb.Append(" DESC");
            }
            else {
                sb.Append(" ASC");
            }

            return sb.ToString();
        }
    }
}
