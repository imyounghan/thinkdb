using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ThinkDb.Dialect;
using ThinkDb.Sql;

namespace ThinkDb.Expressions
{
    public class LogicalExpression : Expression
    {
        public Expression Left { get; private set; }

        public Expression Right { get; private set; }

        //protected LogicalExpression(ExpressionType nodeType)
        //    : base(nodeType, null)
        //{ }

        internal LogicalExpression(Expression lhs, ExpressionType nodeType, Expression rhs)
            : base(nodeType)
        {
            this.Left = lhs;
            this.Right = rhs;
        }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            if (NodeType == ExpressionType.And) {
                return Left.ToSqlStatement(sqlProvider).Concat(Right.ToSqlStatement(sqlProvider)).ToArray();
            }

            List<SqlStatement> args = new List<SqlStatement>();
            args.AddRange(Left.ToSqlStatement(sqlProvider));
            args.AddRange(Right.ToSqlStatement(sqlProvider));

            return new SqlStatement[] { sqlProvider.GetLiteral(NodeType, args) };
        }

        public override object[] GetInputs()
        {
            return Left.GetInputs().Concat(Right.GetInputs()).ToArray();
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", Left.ToString(), OperatorToString(NodeType), Right.ToString());
        }
    }
}
