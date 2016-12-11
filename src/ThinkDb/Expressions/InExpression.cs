using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ThinkDb.Dialect;
using ThinkDb.Sql;


namespace ThinkDb.Expressions
{
    public class InExpression : LogicalExpression
    {
        internal InExpression(ColumnExpression column, ArrayExpression array)
            : base(column, (ExpressionType)1051, array)
        { }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            List<SqlStatement> args = new List<SqlStatement>();
            args.AddRange(Left.ToSqlStatement(sqlProvider));
            args.AddRange(Right.ToSqlStatement(sqlProvider));
            

            return new SqlStatement[] { sqlProvider.GetLiteral(SpecialExpressionType.In, args) };
        }

        public override string ToString()
        {
            return string.Format("({0} In ({1}))", Left.ToString(), Right.ToString());
        }
    }
}
