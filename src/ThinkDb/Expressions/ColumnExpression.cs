using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;

using ThinkDb.Dialect;
using ThinkDb.Sql;


namespace ThinkDb.Expressions
{
    public class ColumnExpression : TableExpression
    {
        internal ColumnExpression(string name)
            : this(name, ColumnExpressionType.None)
        { }

        internal ColumnExpression(string name, ColumnExpressionType columnExpressionType)
            : this(name, columnExpressionType, false, null)
        { }

        internal ColumnExpression(string name, ColumnExpressionType columnExpressionType, bool enableAlias, ReadOnlyCollection<Expression> arguments)
            : this(name, columnExpressionType != ColumnExpressionType.None && enableAlias ? name : null, columnExpressionType, arguments)
        { }
        internal ColumnExpression(string name, string alias, ColumnExpressionType columnExpressionType, ReadOnlyCollection<Expression> arguments)
            : base(name, alias)
        {            
            this.Arguments = arguments;
            this.ColumnExpressionType = columnExpressionType;
        }

        public override ExpressionType NodeType { get { return (ExpressionType)1013; } }

        ///// <summary>
        ///// 别名
        ///// </summary>
        //public string Alias { get; set; }
        ///// <summary>
        ///// 字段名称
        ///// </summary>
        //public string Name { get; private set; }

        public ColumnExpressionType ColumnExpressionType { get; private set; }

        public ReadOnlyCollection<Expression> Arguments { get; private set; }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {            
            var safeField = new SqlStatement(sqlProvider.GetColumn(Name));
            if (ColumnExpressionType != Expressions.ColumnExpressionType.None) {
                List<SqlStatement> args = new List<SqlStatement>();
                args.Add(safeField);

                safeField = sqlProvider.GetLiteral((SpecialExpressionType)ColumnExpressionType, args);
            }
            if (!string.IsNullOrWhiteSpace(Alias)) {
                safeField = sqlProvider.GetColumnAsAlias(safeField.ToString(), Alias);
            }
            return new SqlStatement[] { safeField };
        }
        //public override SqlStatement ToSqlStatement(ISqlProvider sqlProvider)
        //{
        //    var safeField = new SqlStatement(sqlProvider.GetColumn(Name));
        //    if (ColumnExpressionType != Expressions.ColumnExpressionType.None) {
        //        List<SqlStatement> args = new List<SqlStatement>();
        //        args.Add(safeField);

        //        return sqlProvider.GetLiteral((SpecialExpressionType)ColumnExpressionType, args);
        //    }
        //    //sqlProvider.GetLiteral(SpecialExpressionType

        //    return safeField;
        //}


        public override string ToString()
        {
            string format = "{0}";
            if (ColumnExpressionType != ColumnExpressionType.None) {
                format = string.Concat(ColumnExpressionType, "({0})");
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(format, Name);

            if (!string.IsNullOrWhiteSpace(Alias)) {
                sb.Append(" AS ").Append(Alias);
            }

            return sb.ToString();
        }
    }
}
