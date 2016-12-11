using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using ThinkDb.Dialect;
using ThinkDb.Expressions;
using ThinkDb.Sql;

namespace ThinkDb
{
    public class DataTable : IDataTable
    {
        private readonly IDatabase _database;
        private readonly ISqlProvider _sqlProvider;
        public DataTable(string table, IDatabase database, ISqlProvider sqlProvider)
        {
            this._database = database;
            this._sqlProvider = sqlProvider;
            this._table = new TableExpression(table);
        }

        private Expression _table;
        private bool _distinct = false;
        public IDataTable Distinct()
        {
            this._distinct = true;
            return this;
        }

        private Expression _fields = null;
        public IDataTable Select(Expression fields)
        {
            this._fields = fields;
            return this;
        }
        private Expression _filters = null;
        public IDataTable Where(Expression wheres)
        {
            this._filters = wheres;
            return this;
        }
        private bool _groupby = false;
        public IDataTable GroupBy()
        {
            _groupby = true;
            return this;
        }
        private Expression _havings = null;
        public IDataTable Having(Expression havings)
        {
            this._havings = havings;
            return this;
        }
        private Expression _sorters = null;
        public IDataTable OrderBy(Expression orders)
        {
            this._sorters = orders;
            return this;
        }

        private int _limit;
        private int _offset;
        public IDataTable Limit(int page, int rows)
        {
            _limit = rows;
            _offset = page * rows;

            return this;
        }

        private void Reset()
        {
        }

        public string GetSelectSql(bool distinct, Expression fields, Expression table, Expression wheres, bool group, Expression havings, Expression orders, int limit, int offset, out DbParameter[] parameters)
        {
            ArrayList values = new ArrayList();
            List<SqlStatement> list = new List<SqlStatement>();

            SqlStatement[] columns = fields == null ? new SqlStatement[] { _sqlProvider.GetColumns() } :
                fields.ToSqlStatement(_sqlProvider);
            SqlStatement sqlSelect = !distinct ? _sqlProvider.GetSelectClause(columns) : _sqlProvider.GetSelectDistinctClause(columns);
            list.Add(sqlSelect);

            SqlStatement sqlTable = _sqlProvider.GetFromClause(table.ToSqlStatement(_sqlProvider));
            list.Add(sqlTable);

            if(wheres != null) {
                SqlStatement sqlWhere = _sqlProvider.GetWhereClause(wheres.ToSqlStatement(_sqlProvider));
                list.Add(sqlWhere);
                values.AddRange(wheres.GetInputs());
            }
            if(group && columns.Length > 0) {
                var groups = columns.Where(item => {
                    var str = item.ToString();
                    if(string.IsNullOrWhiteSpace(str))
                        return false;

                    return str.IndexOf("max", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                        str.IndexOf("min", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                        str.IndexOf("sum", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                        str.IndexOf("count", StringComparison.CurrentCultureIgnoreCase) != -1 ||
                        str.IndexOf("avg", StringComparison.CurrentCultureIgnoreCase) != -1;
                }).ToArray();
                if(groups.Length > 0) {
                    SqlStatement sqlGroup = _sqlProvider.GetGroupByClause(groups);
                    list.Add(sqlGroup);
                }
            }
            if(havings != null) {
                SqlStatement sqlHaving = _sqlProvider.GetHavingClause(havings.ToSqlStatement(_sqlProvider));
                list.Add(sqlHaving);

                values.AddRange(havings.GetInputs());
            }
            if(orders != null) {
                SqlStatement sqlOrder = _sqlProvider.GetOrderByClause(orders.ToSqlStatement(_sqlProvider));
                list.Add(sqlOrder);
            }



            var fullSql = SqlStatement.Join(" ", list);
            if(limit > 0 && offset > 0) {
                fullSql = _sqlProvider.GetLiteralLimit(fullSql, "?", "?", SqlStatement.Empty);
                values.AddRange(new int[] { limit, offset });
            }
            else if(limit > 0) {
                fullSql = _sqlProvider.GetLiteralLimit(fullSql, "?");
                values.Add(limit);
            }

            var matches = Regex.Matches(fullSql.ToString(), @"\?");
            parameters = new DbParameter[matches.Count];
            for(int i = 0; i < matches.Count; i++) {
                parameters[i] = _database.CreateInParameter(_sqlProvider.GetParameterName("p" + i), values[i]);
            }

            return fullSql.ToString();
        }


        public IDictionary Find()
        {
            var result = this.Limit(0, 1).FindAll();
            if (result.Count == 0)
                return new Hashtable();
            //return result[0] as IDictionary;
            return null;
        }

        public ICollection FindAll()
        {
            DbParameter[] parameters;
            string sql = this.GetSelectSql(_distinct, _fields, _table, _filters,
                _groupby, _havings, _sorters, _limit, _offset, out parameters);
            
            try {
                using (var reader = _database.ExecuteReader(CommandType.Text, sql, parameters)) {
                    return reader.ToCollection();
                }
            }
            catch (Exception) {
                throw;
            }
            finally {
                this.Reset();
            }
        }

        public ICollection FindAll(out long total)
        {
            DbParameter[] parameters;
            string sql = this.GetSelectSql(false, Expression.Field("0", ColumnExpressionType.Count),
                _table, _filters, false, null, null, 0, 0, out parameters);

            var result = _database.ExecuteScalar(CommandType.Text, sql, parameters);
            total = Convert.ToInt64(result);

            return FindAll();
        }

        public bool Delete()
        {
            throw new NotImplementedException();
        }

        public IDataTable Data(Expression datas)
        {
            throw new NotImplementedException();
        }

        public bool Insert(out long id)
        {
            throw new NotImplementedException();
        }

        public bool Insert()
        {
            //_sqlProvider.GetUpdate
            throw new NotImplementedException();
        }

        public bool Update()
        {
            throw new NotImplementedException();
        }
    }
}
