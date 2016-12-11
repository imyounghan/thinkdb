using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using ThinkDb.Dialect;
using ThinkDb.Sql;

namespace ThinkDb.Expressions
{
    public abstract class Expression
    {
        class EmptyExpression : Expression
        {
            public EmptyExpression()
                : base(ExpressionType.Default)
            { }
        }

        protected static string OperatorToString(ExpressionType expressionType)
        {
            switch (expressionType) {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.AndAlso:
                    return "&&";
                case ExpressionType.Coalesce:
                    return "??";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.OrElse:
                    return "||";
                case ExpressionType.Power:
                    return "^";
                case ExpressionType.RightShift:
                    return ">>";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.And:
                    return "And";
                case ExpressionType.Or:
                    return "Or";
                default:
                    return string.Empty;
            }
        }

        //class ExpressionPrinter
        //{
        //    static string OperatorToString(Expression binary)
        //    {
        //        switch (binary.NodeType) {
        //            case ExpressionType.Add:
        //            case ExpressionType.AddChecked:
        //                return "+";
        //            case ExpressionType.AndAlso:
        //                return "&&";
        //            case ExpressionType.Coalesce:
        //                return "??";
        //            case ExpressionType.Divide:
        //                return "/";
        //            case ExpressionType.Equal:
        //                return "=";
        //            case ExpressionType.ExclusiveOr:
        //                return "^";
        //            case ExpressionType.GreaterThan:
        //                return ">";
        //            case ExpressionType.GreaterThanOrEqual:
        //                return ">=";
        //            case ExpressionType.LeftShift:
        //                return "<<";
        //            case ExpressionType.LessThan:
        //                return "<";
        //            case ExpressionType.LessThanOrEqual:
        //                return "<=";
        //            case ExpressionType.Modulo:
        //                return "%";
        //            case ExpressionType.Multiply:
        //            case ExpressionType.MultiplyChecked:
        //                return "*";
        //            case ExpressionType.NotEqual:
        //                return "!=";
        //            case ExpressionType.OrElse:
        //                return "||";
        //            case ExpressionType.Power:
        //                return "^";
        //            case ExpressionType.RightShift:
        //                return ">>";
        //            case ExpressionType.Subtract:
        //            case ExpressionType.SubtractChecked:
        //                return "-";
        //            case ExpressionType.And:
        //                return "And";
        //            case ExpressionType.Or:
        //                return "Or";
        //            default:
        //                return null;
        //        }
        //    }

        //    static bool HasStringRepresentation(object obj)
        //    {
        //        return obj.ToString() != obj.GetType().ToString();
        //    }

        //    public static string ToString(Expression expression)
        //    {
        //        var printer = new ExpressionPrinter();
        //        printer.Visit(expression);
        //        return printer.builder.ToString();
        //    }

        //    StringBuilder builder;

        //    const string ListSeparator = ", ";

        //    ExpressionPrinter(StringBuilder builder)
        //    {
        //        this.builder = builder;
        //    }

        //    ExpressionPrinter()
        //        : this(new StringBuilder())
        //    { }

        //    void Print(string str)
        //    {
        //        builder.Append(str);
        //    }

        //    void Print(object obj)
        //    {
        //        builder.Append(obj);
        //    }

        //    void Print(string str, params object[] objs)
        //    {
        //        builder.AppendFormat(str, objs);
        //    }

        //    protected virtual void VisitUnary(UnaryExpression unary)
        //    {
        //        switch (unary.NodeType) {
        //            case ExpressionType.ArrayLength:
        //            case ExpressionType.Convert:
        //            case ExpressionType.ConvertChecked:
        //            case ExpressionType.Not:
        //                Print("{0}(", unary.NodeType);
        //                Visit(unary.Operand);
        //                Print(")");
        //                return;
        //            case ExpressionType.Negate:
        //                Print("-");
        //                Visit(unary.Operand);
        //                return;
        //            case ExpressionType.Quote:
        //                Visit(unary.Operand);
        //                return;
        //            //case ExpressionType.TypeAs:
        //            //    Print("(");
        //            //    Visit(unary.Operand);
        //            //    Print(" As {0})", unary.Type.Name);
        //            //    return;
        //            case ExpressionType.UnaryPlus:
        //                Print("+");
        //                Visit(unary.Operand);
        //                return;
        //        }

        //        throw new NotImplementedException();
        //    }

        //    protected virtual void VisitConstant(ConstantExpression constant)
        //    {
        //        var value = constant.Value;

        //        if (value == null) {
        //            Print("null");
        //        }
        //        else if (value is string) {
        //            Print("\"");
        //            Print(value);
        //            Print("\"");
        //        }
        //        else if (!HasStringRepresentation(value)) {
        //            Print("value(");
        //            Print(value);
        //            Print(")");
        //        }
        //        else
        //            Print(value);
        //    }
        //    void PrintArrayIndex(BinaryExpression index)
        //    {
        //        Visit(index.Left);
        //        Print("[");
        //        Visit(index.Right);
        //        Print("]");
        //    }
        //    protected virtual void VisitBinary(BinaryExpression binary)
        //    {
        //        switch (binary.NodeType) {
        //            case ExpressionType.ArrayIndex:
        //                PrintArrayIndex(binary);
        //                return;
        //            default:
        //                Print("(");
        //                Visit(binary.Left);
        //                Print(" {0} ", OperatorToString(binary));
        //                Visit(binary.Right);
        //                Print(")");
        //                return;
        //        }
        //    }

        //    protected virtual void VisitColumn(ColumnExpression column)
        //    {
        //        string format = "{0}";
        //        if (column.ColumnExpressionType != ColumnExpressionType.None) {
        //            format = string.Concat(column.ColumnExpressionType, "({0}) AS {1}");
        //        }
        //        Print(format, column.Name, column.Alias);
        //    }

        //    protected virtual void VisitMemberAccess(MemberExpression member)
        //    {
        //        Print(member.Name);
        //    }

        //    protected virtual void VisitMethodCall(MethodCallExpression call)
        //    {
        //        //int start = 0;
        //        //Expression ob = null;
        //        //if (call.Method== SpecialExpressionType.In) {
        //        //    start = 1;
        //        //    ob = call.Arguments[0];
        //        //}
        //        if (call.Object != null) {
        //            Visit(call.Object);
        //            //Print(".");
        //        }
        //        //Print(call.Method.Name);
        //        switch (call.Method) {
        //            case SpecialExpressionType.In:
        //            case SpecialExpressionType.Like:
        //                Print(" {0} ", call.Method);
        //                break;
        //            default:
        //                Print(call.Method);
        //                break;
        //        }
                
        //        Print("(");
        //        VisitExpressionList(call.Arguments);
        //        Print(")");
        //    }
        //    protected virtual void VisitList<T>(IList<T> list, Action<T> visitor)
        //    {
        //        for (int i = 0; i < list.Count; i++) {
        //            if (i > 0)
        //                Print(ListSeparator);

        //            visitor(list[i]);
        //        }
        //    }

        //    protected virtual void VisitExpressionList(IList<Expression> list)
        //    {
        //        VisitList(list, Visit);
        //    }

        //    protected void VisitNewArray(NewArrayExpression newArray)
        //    {
        //        //Print("new ");
        //        switch (newArray.NodeType) {
        //            case ExpressionType.NewArrayInit:
        //                //Print("[] {");
        //                //VisitExpressionList(newArray.Expressions);
        //                //Print("}");
        //                //return;
        //            case ExpressionType.NewArrayBounds:
        //                //Print(newArray.Type);
        //                //Print("(");
        //                VisitExpressionList(newArray.Expressions);
        //                //Print(")");
        //                return;                    
        //        }

        //        throw new NotSupportedException();
        //    }

        //    protected virtual void Visit(Expression expression)
        //    {
        //        if (expression == null)
        //            return;

        //        switch (expression.NodeType) {
        //            case ExpressionType.Negate:
        //            case ExpressionType.NegateChecked:
        //            case ExpressionType.Not:
        //            case ExpressionType.Convert:
        //            case ExpressionType.ConvertChecked:
        //            case ExpressionType.ArrayLength:
        //            case ExpressionType.Quote:
        //            case ExpressionType.TypeAs:
        //            case ExpressionType.UnaryPlus:
        //                VisitUnary((UnaryExpression)expression);
        //                break;
        //            case ExpressionType.Add:
        //            case ExpressionType.AddChecked:
        //            case ExpressionType.Subtract:
        //            case ExpressionType.SubtractChecked:
        //            case ExpressionType.Multiply:
        //            case ExpressionType.MultiplyChecked:
        //            case ExpressionType.Divide:
        //            case ExpressionType.Modulo:
        //            case ExpressionType.Power:
        //            case ExpressionType.And:
        //            case ExpressionType.AndAlso:
        //            case ExpressionType.Or:
        //            case ExpressionType.OrElse:
        //            case ExpressionType.LessThan:
        //            case ExpressionType.LessThanOrEqual:
        //            case ExpressionType.GreaterThan:
        //            case ExpressionType.GreaterThanOrEqual:
        //            case ExpressionType.Equal:
        //            case ExpressionType.NotEqual:
        //            case ExpressionType.Coalesce:
        //            case ExpressionType.ArrayIndex:
        //            case ExpressionType.RightShift:
        //            case ExpressionType.LeftShift:
        //            case ExpressionType.ExclusiveOr:
        //                VisitBinary((BinaryExpression)expression);
        //                break;
        //            case ExpressionType.TypeIs:
        //                //VisitTypeIs((TypeBinaryExpression)expression);
        //                //break;
        //                goto default;
        //            case ExpressionType.Conditional:
        //                //VisitConditional((ConditionalExpression)expression);
        //                break;
        //            case ExpressionType.Constant:
        //                VisitConstant((ConstantExpression)expression);
        //                break;
        //            case ExpressionType.Parameter:
        //                //VisitParameter((ParameterExpression)expression);
        //                break;
        //            case ExpressionType.MemberAccess:
        //                VisitMemberAccess((MemberExpression)expression);
        //                break;
        //            case ExpressionType.Call:
        //                VisitMethodCall((MethodCallExpression)expression);
        //                break;
        //            case ExpressionType.Lambda:
        //                //VisitLambda((LambdaExpression)expression);
        //                break;
        //            case ExpressionType.New:
        //                //VisitNew((NewExpression)expression);
        //                break;
        //            case ExpressionType.NewArrayInit:
        //            case ExpressionType.NewArrayBounds:
        //                VisitNewArray((NewArrayExpression)expression);
        //                break;
        //            case ExpressionType.Invoke:
        //                //VisitInvocation((InvocationExpression)expression);
        //                break;
        //            case ExpressionType.MemberInit:
        //                //VisitMemberInit((MemberInitExpression)expression);
        //                break;
        //            case ExpressionType.ListInit:
        //                //VisitListInit((ListInitExpression)expression);
        //                break;
        //            case (ExpressionType)1012:
        //                VisitColumn((ColumnExpression)expression);
        //                break;
        //            default:
        //                throw new ArgumentException(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
        //        }
        //    }
        //}

        //protected Expression(ExpressionType nodeType, Type type)
        //{
        //    this.NodeType = nodeType;
        //    this.Type = type;
        //}


        protected Expression(ExpressionType nodeType)
        {
            this.NodeType = nodeType;
        }
        public virtual ExpressionType NodeType { get; private set; }

        //public virtual Type Type { get; private set; }

        /// <summary>
        /// 输出Sql语句
        /// </summary>
        public virtual SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            return new SqlStatement[0];
        }
        /// <summary>
        /// 获取输入参数
        /// </summary>
        public virtual object[] GetInputs()
        {
            return new object[0];
        }
        

        public static Expression operator &(Expression lhs, Expression rhs)
        {
            return new LogicalExpression(lhs, ExpressionType.And, rhs);
        }

        public static Expression operator |(Expression lhs, Expression rhs)
        {
            return new LogicalExpression(lhs, ExpressionType.Or, rhs);
        }        
        
        public static bool operator false(Expression expression)
        {
            return false;
        }

        public static bool operator true(Expression expression)
        {
            return false;
        }

        public static readonly Expression Empty = new EmptyExpression();

        private static LogicalExpression CreateSimpleExpression(ColumnExpressionType columnExpressionType, string column, ExpressionType expressionType, object value)
        {
            //Expression expression = new ColumnExpression(column, type, columnExpressionType);
            //if (columnExpressionType != ColumnExpressionType.None) {
            //    expression = new MethodCallExpression((SpecialExpressionType)columnExpressionType, null,
            //        new ReadOnlyCollection<Expression>(new[] { expression }), type);
            //}

            return new LogicalExpression(new ColumnExpression(column, columnExpressionType), 
                expressionType, new VariableExpression(value));
        }

        ///// <summary>
        ///// Apply an "equals" constraint to each column in the key set of a IDictionary
        ///// </summary>
        //public static Expression[] AllEqual(IDictionary columnNameValues)
        //{
        //    List<Expression> expressions = new List<Expression>();
            
        //    foreach (DictionaryEntry item in columnNameValues) {
        //        var expression = CreateSimpleExpression(ColumnExpressionType.None, item.Key.ToString(), ExpressionType.Equal, item.Value);

        //        expressions.Add(expression);
        //    }
        //    return expressions.ToArray();
        //}

        /// <summary>
        /// Apply a "equal" constraint to the named column
        /// </summary>
        public static Expression Equal(string column, object value, ColumnExpressionType expressionType = ColumnExpressionType.None)
        {
            return CreateSimpleExpression(expressionType, column, ExpressionType.Equal, value);
        }
        /// <summary>
        /// Apply a "not equal" constraint to the named column
        /// </summary>
        public static Expression NotEqual(string column, object value, ColumnExpressionType expressionType = ColumnExpressionType.None)
        {
            return CreateSimpleExpression(expressionType, column, ExpressionType.NotEqual, value);
        }
        /// <summary>
        /// Apply a "greater than" constraint to the named column
        /// </summary>
        public static Expression GreaterThan(string column, object value, ColumnExpressionType expressionType = ColumnExpressionType.None)
        {
            return CreateSimpleExpression(expressionType, column, ExpressionType.GreaterThan, value);
        }
        /// <summary>
        /// Apply a "greater than or equal" constraint to the named column
        /// </summary>
        public static Expression GreaterThanOrEqual(string column, object value, ColumnExpressionType expressionType = ColumnExpressionType.None)
        {
            return CreateSimpleExpression(expressionType, column, ExpressionType.GreaterThanOrEqual, value);
        }
        /// <summary>
        /// Apply a "less than" constraint to the named column
        /// </summary>
        public static Expression LessThan(string column, object value, ColumnExpressionType expressionType = ColumnExpressionType.None)
        {
            return CreateSimpleExpression(expressionType, column, ExpressionType.LessThan, value);
        }
        /// <summary>
        /// Apply a "less than or equal" constraint to the named column
        /// </summary>
        public static Expression LessThanOrEqual(string column, object value, ColumnExpressionType expressionType = ColumnExpressionType.None)
        {
            return CreateSimpleExpression(expressionType, column, ExpressionType.LessThanOrEqual, value);
        }

        // <summary>
        /// Apply a "between" constraint to the named column
        /// </summary>
        public static Expression[] Between(string column, object lo, object hi)
        {
            if (lo != null && hi != null) {
                return new Expression[] { GreaterThanOrEqual(column, lo), LessThanOrEqual(column, hi) };
            }
            else if (lo != null) {
                return new Expression[] { GreaterThanOrEqual(column, lo) };
            }
            else if (hi != null) {
                return new Expression[] { LessThanOrEqual(column, hi) };
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Apply a "like" constraint to the named column
        /// </summary>
        public static Expression Like(string column, string value)
        {
            return new LikeExpression(new ColumnExpression(column), new VariableExpression(value));
        }

        /// <summary>
        /// Apply an "is null" constraint to the named column
        /// </summary>
        public static Expression IsNull(string column)
        {
            return new NullExpression(new ColumnExpression(column));
        }

        ///// <summary>
        ///// Apply an "is null" constraint to the named column
        ///// </summary>
        //public static Expression IsNotNull(string column)
        //{
        //    return new MethodCallExpression(SpecialExpressionType.IsNotNull, 
        //        new ColumnExpression(column, null),
        //        null, null);
        //}

        /// <summary>
        /// Apply an "in" constraint to the named property 
        /// </summary>
        public static Expression In(string column, params object[] values)
        {
            //List<Expression> valueExpressions = new List<Expression>();
            //foreach (object value in values) {
            //    valueExpressions.Add(new VariableExpression(value));
            //}
            var valueExpressions = values.Select(value => new VariableExpression(value)).Cast<Expression>().ToList();

            //List<Expression> arguments = new List<Expression>();
            ////arguments.Add(new MemberExpression(column, type));
            //arguments.Add(new ArrayExpression(type,
            //    new ReadOnlyCollection<Expression>(valueCriterions)));

            //return new BinaryExpression(new ColumnExpression(column, type),
            //    (ExpressionType)SpecialExpressionType.In,
            //    new NewArrayExpression(type, new ReadOnlyCollection<Expression>(valueCriterions)));
            return new InExpression(new ColumnExpression(column),
                new ArrayExpression(new ReadOnlyCollection<Expression>(valueExpressions)));
        }


        private static Expression CreateColumnExpression(string column, ExpressionType expressionType, string otherColumn)
        {
            return new LogicalExpression(new ColumnExpression(column), expressionType, new ColumnExpression(otherColumn));
        }

        /// <summary>
        /// Apply an "equal" constraint to two columns
        /// </summary>
        public static Expression EqualColumn(string column, string otherColumn)
        {
            return CreateColumnExpression(column, ExpressionType.Equal, otherColumn);
        }
        /// <summary>
        /// Apply a "not equal" constraint to the named column
        /// </summary>
        public static Expression NotEqualColumn(string column, string otherColumn)
        {
            return CreateColumnExpression(column, ExpressionType.NotEqual, otherColumn);
        }
        /// <summary>
        /// Apply a "greater than" constraint to the named column
        /// </summary>
        public static Expression GreaterThanColumn(string column, string otherColumn)
        {
            return CreateColumnExpression(column, ExpressionType.GreaterThan, otherColumn);
        }
        /// <summary>
        /// Apply a "greater than or equal" constraint to the named column
        /// </summary>
        public static Expression GreaterThanOrEqualColumn(string column, string otherColumn)
        {
            return CreateColumnExpression(column, ExpressionType.GreaterThanOrEqual, otherColumn);
        }
        /// <summary>
        /// Apply a "less than" constraint to the named column
        /// </summary>
        public static Expression LessThanColumn(string column, string otherColumn)
        {
            return CreateColumnExpression(column, ExpressionType.LessThan, otherColumn);
        }
        /// <summary>
        /// Apply a "less than or equal" constraint to the named column
        /// </summary>
        public static Expression LessThanOrEqualColumn(string column, string otherColumn)
        {
            return CreateColumnExpression(column, ExpressionType.LessThanOrEqual, otherColumn);
        }

        public static Expression IncrementColumn(string column, int step = 1)
        {
            return new LogicalExpression(new ColumnExpression(column), ExpressionType.Equal,
                new LogicalExpression(new ColumnExpression(column), ExpressionType.Add,
                new VariableExpression(step, true)));
        }

        public static Expression DecrementColumn(string column, int step = 1)
        {
            return new LogicalExpression(new ColumnExpression(column), ExpressionType.Equal,
                new LogicalExpression(new ColumnExpression(column), ExpressionType.Decrement,
                new VariableExpression(step, true)));
        }

        public static ColumnExpression Field(string column, ColumnExpressionType columnExpressionType = ColumnExpressionType.None)
        {
            return new ColumnExpression(column, columnExpressionType);
        }

        public static ColumnExpression Field(string column, string alias, ColumnExpressionType columnExpressionType = ColumnExpressionType.None)
        {
            return new ColumnExpression(column, alias, columnExpressionType, null);
        }

        public static OrderExpression OrderBy(string column)
        {
            return new OrderExpression(column, SortOrder.Ascending);
        }

        public static OrderExpression OrderByDescending(string column)
        {
            return new OrderExpression(column, SortOrder.Descending);
        }
    }
}
