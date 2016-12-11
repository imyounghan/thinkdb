using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThinkDb.Expressions;

namespace ThinkDb
{
    public static class DataTableExtentions
    {
        public static long Count(this IDataTable datatable)
        {
            var dict = datatable.Select(Expression.Field("0", "Result", ColumnExpressionType.Count)).Find();

            if (dict.Count == 0)
                return 0;

            return Convert.ToInt64(dict["Result"]);
        }
    }
}
