using System;
using System.Linq.Expressions;

using ThinkDb.Dialect;
using ThinkDb.Sql;


namespace ThinkDb.Expressions
{
    public class NullExpression : Expression
    {
        public ColumnExpression Column { get; private set; }

        public NullExpression(ColumnExpression column)
            : base((ExpressionType)1060)
        {
            this.Column = column;
        }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            return new SqlStatement[] { sqlProvider.GetLiteral(SpecialExpressionType.IsNull, Column.ToSqlStatement(sqlProvider)) };
        }

        public override string ToString()
        {
            return Column.ToString() + " IS NULL";
        }
    }
}
