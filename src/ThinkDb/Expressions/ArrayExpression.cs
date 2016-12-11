using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using ThinkDb.Dialect;
using ThinkDb.Sql;


namespace ThinkDb.Expressions
{
    public class ArrayExpression : Expression
    {
        public ReadOnlyCollection<Expression> Expressions { get; private set; }
        /// <summary>
        /// 集合分割符
        /// </summary>
        public string ListSeparator { get; set; }

        internal ArrayExpression(ReadOnlyCollection<Expression> expressions)
            : base(ExpressionType.NewArrayInit)
        {
            this.Expressions = expressions;
            this.ListSeparator = ", ";
        }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            return Expressions.SelectMany(expression => expression.ToSqlStatement(sqlProvider)).ToArray();
        }

        public override object[] GetInputs()
        {
            return Expressions.SelectMany(expression => expression.GetInputs()).ToArray();
        }

        //public override SqlStatement ToSqlStatement(ISqlProvider sqlProvider)
        //{
        //    throw new System.NotImplementedException();
        //}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Expressions.Count; i++) {
                if (i > 0)
                    sb.Append(ListSeparator);

                sb.Append(Expressions[i].ToString());
            }
            return sb.ToString();
        }
    }
}
