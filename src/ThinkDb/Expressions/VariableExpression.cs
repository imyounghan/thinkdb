using System;
using System.Linq.Expressions;
using ThinkDb.Dialect;
using ThinkDb.Sql;

namespace ThinkDb.Expressions
{
    public class VariableExpression : Expression
    {
        public object Value { get; private set; }
        /// <summary>
        /// 是否为常量
        /// </summary>
        public bool Constant { get; private set; }


        internal VariableExpression(object value)
            : this(value, false)
        { }

        internal VariableExpression(object value, bool constant)
            : base(ExpressionType.Constant)
        {
            this.Value = value;
            this.Constant = constant;
        }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            if (Constant) {
                return new SqlStatement[] { new SqlStatement(Value.ToString()) };
            }

            return new SqlStatement[] { new SqlStatement("?") };
        }

        public override object[] GetInputs()
        {
            if (!Constant) {
                return new object[] { Value };
            }

            return base.GetInputs();
        }


        public override string ToString()
        {
            if (Value == null) {
                return "null";
            }
            else if (Value is string) {
                return string.Concat("\"", Value, "\"");
            }
            else if (!HasStringRepresentation(Value)) {
                return string.Concat("value(", Value, ")");
            }
            else
                return Value.ToString();
        }

        static bool HasStringRepresentation(object obj)
        {
            return obj.ToString() != obj.GetType().ToString();
        }
    }
}
