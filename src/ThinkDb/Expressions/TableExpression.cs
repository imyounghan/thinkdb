using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ThinkDb.Dialect;
using ThinkDb.Sql;

namespace ThinkDb.Expressions
{
    public class TableExpression : Expression
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; private set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        internal TableExpression(string name)
            : this(name, null)
        { }

        internal TableExpression(string name, string alias)
            : base((ExpressionType)1011)
        {
            this.Name = name;
            this.Alias = alias;
        }

        public override SqlStatement[] ToSqlStatement(ISqlProvider sqlProvider)
        {
            if (string.IsNullOrWhiteSpace(Alias))
                return new SqlStatement[] { sqlProvider.GetTable(Name) };

            return new SqlStatement[] { sqlProvider.GetTableAsAlias(Name, Alias) };
        }

        public override string ToString()
        {
            return string.Format("{0} AS {1}", Name, Alias);
        }
    }
}
