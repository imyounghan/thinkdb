using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ThinkDb.Dialect;
using ThinkDb.Expressions;
using ThinkDb.Sql;

namespace ThinkDb.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Expression<Func<UserDTO, bool>> lamda = (p) => new int[] { 1,2,3 }.Contains(p.ID);
            //Expression<Func<IGrouping<string, UserDTO>, object>> lamda = p => new {
            //    ID = p.Max(item => item.ID)
            //};

            //ExpressionType ExpressionType = (ExpressionType)ThinkNet.Data.Expressions.CustomExpressionType.Column;


            //Expression<Func<UserDTO, bool>> lamda = (p) => p.UserName.Contains("yang");

            //var dt = new DataTable("User", null, new SqlServerSqlProvider())
            //    .Select(Expression.Field("0", "result", ColumnExpressionType.Count))
            //    .Where(Expression.Equal("user", "younghan") && Expression.Equal("password", "000000"))
            //    .OrderBy(Expression.OrderBy("ID"));
            //IList values;
            //var sql = (dt as DataTable).GetSelectSql(out values);

            //int index = 0;
            //var result = new Regex(@"\?").Replace(sql, (match) => {
            //    return string.Format("@p{0}", index++);
            //});

            //var matches = new Regex(@"\?").Matches(sql);
            ISqlProvider sqlProvider = new SqlServerSqlProvider();
            var criterion = Expression.IncrementColumn("score");
            var sql = criterion.ToSqlStatement(sqlProvider);
            var fullSql = SqlStatement.Join(" ", sql);
            Console.WriteLine(fullSql.ToString());

            criterion = Expression.Like("userName", "yang");
            sql = criterion.ToSqlStatement(sqlProvider);
            fullSql = SqlStatement.Join(" ", sql);
            Console.WriteLine(fullSql.ToString());

            criterion = Expression.EqualColumn("user", "younghan");
            sql = criterion.ToSqlStatement(sqlProvider);
            fullSql = SqlStatement.Join(" ", sql);
            Console.WriteLine(fullSql.ToString());

            criterion = Expression.Equal("id", 10, ColumnExpressionType.Max);
            sql = criterion.ToSqlStatement(sqlProvider);
            fullSql = SqlStatement.Join(" ", sql);
            Console.WriteLine(fullSql.ToString());

            criterion = Expression.Equal("user", "younghan") &&
                (Expression.Equal("password", "000000") ||
                 Expression.Equal("password", "123456"));
            sql = criterion.ToSqlStatement(sqlProvider);
            fullSql = SqlStatement.Join(" ", sql);
            Console.WriteLine(fullSql.ToString());

            criterion = Expression.In("id", 1, 2, 3);
            sql = criterion.ToSqlStatement(sqlProvider);
            fullSql = SqlStatement.Join(" ", sql);
            Console.WriteLine(fullSql.ToString());

            Console.ReadKey();

        }
    }
}
