using System.Collections.Generic;
using System.Linq.Expressions;
using ThinkDb.Dialect;
using ThinkDb.Sql;

namespace ThinkDb.Expressions
{
    public class NotExpression : Expression
    {
        public Expression Operand { get; private set; }

        public NotExpression(Expression operand)
            : base(ExpressionType.Not)
        {
            this.Operand = operand;
        }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            var args = new List<SqlStatement>();
            args.AddRange(Operand.ToSqlStatement(sqlProvider));

            return new SqlStatement[] { sqlProvider.GetLiteral(ExpressionType.Not, args) };
            //throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return string.Concat("Not(", Operand.ToString(), ")");
        }
    }
}
