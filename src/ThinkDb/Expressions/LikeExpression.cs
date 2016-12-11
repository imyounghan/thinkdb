using System.Collections.Generic;
using System.Linq.Expressions;
using ThinkDb.Dialect;
using ThinkDb.Sql;

namespace ThinkDb.Expressions
{
    public class LikeExpression : LogicalExpression
    {
        public enum MatchType
        {
            Contains,
            StartsWith,
            EndsWith
        }

        public MatchType MatchMode { get; private set; }

        internal LikeExpression(ColumnExpression column, VariableExpression value, MatchType matchMode = LikeExpression.MatchType.Contains)
            : base(column, (ExpressionType)1052, value)
        {
            this.MatchMode = matchMode;
        }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            List<SqlStatement> args = new List<SqlStatement>();

            args.AddRange(Left.ToSqlStatement(sqlProvider));
            
            if (MatchMode == MatchType.Contains || MatchMode == MatchType.EndsWith) {
                args.Add(sqlProvider.GetLiteral("%"));
            }
            args.AddRange(Right.ToSqlStatement(sqlProvider));
            if (MatchMode == MatchType.Contains || MatchMode == MatchType.StartsWith) {
                args.Add(sqlProvider.GetLiteral("%"));
            }

            return new SqlStatement[] { sqlProvider.GetLiteral(SpecialExpressionType.Like, args) };
        }

        public override string ToString()
        {
            return string.Format("({0}.{1}({2}))", Left.ToString(), MatchMode.ToString(), Right.ToString());
        }
    }
}
