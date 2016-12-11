using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace ThinkDb
{
    public abstract class Database : IDatabase, IDisposable
    {
        #region Private Static Methods
        private static void CloseConnection(IDbConnection connection)
        {
            if (connection == null) {
                return;
            }

            if (connection.State != ConnectionState.Closed) {
                connection.Close();
            }
        }

        private static void DisposeTransaction(IDbTransaction transaction)
        {
            if (transaction == null) {
                return;
            }

            transaction.Dispose();
        }
        private static void DisposeConnection(IDbConnection connection)
        {
            CloseConnection(connection);
            connection.Dispose();
        }

        private static void AttachParameters(IDbCommand command, IEnumerable<DbParameter> commandParameters)
        {
            if (commandParameters != null) {
                foreach (var parameter in commandParameters) {
                    if (parameter != null) {
                        if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null)) {
                            parameter.Value = DBNull.Value;
                        }
                        command.Parameters.Add(parameter);
                    }
                }
            }
        }
        #endregion


        #region Methods
        /// <summary>
        /// 为当前对象创建一个数据连接
        /// </summary>
        protected abstract DbConnection CreateConnection();
        private DbConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) {
                new DatabaseException("没有提供有效的连接。");
            }

            DbConnection connection = null;
            try {
                connection = CreateConnection();
                connection.ConnectionString = connectionString;
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                return connection;
            }
            catch (Exception ex) {
                CloseConnection(connection);
                throw ex;
            }
        }

        /// <summary>
        /// 创建数据库命令
        /// </summary>
        protected abstract DbCommand CreateCommand();
        private DbCommand CreateCommand(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
        {
            var command = CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;
            command.Connection = connection;
            if (transaction != null) {
                command.Transaction = transaction;
            }

            if (commandParameters != null) {
                AttachParameters(command, commandParameters);
            }

            return command;
        }

        /// <summary>
        /// 创建一个DbCommand
        /// </summary>
        protected DbCommand CreateCommandByCommandType(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
        {
            return CreateCommand(_connection, _transaction, commandType, commandText, commandParameters);
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        protected abstract DbParameter CreateParameter();


        private DbConnection _connection;
        /// <summary>
        /// 当前的数据源连接
        /// </summary>
        public DbConnection DbConnection
        {
            get { return this._connection; }
        }
        private DbTransaction _transaction;
        /// <summary>
        /// 当前的数据源事务
        /// </summary>
        public DbTransaction DbTransaction
        {
            get { return this._transaction; }
        }
        #endregion

        #region Ctor
        private readonly string _connectionString;
        private readonly bool _closeConnection = true;
        
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        protected Database(DbConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");

            this._connection = connection;
            this._connectionString = connection.ConnectionString;
            this._closeConnection = false;
        }
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        protected Database(string nameOrConnectionString)
        {
            if(nameOrConnectionString == null)
                throw new ArgumentNullException("nameOrConnectionString");


            if (nameOrConnectionString.StartsWith("name=", StringComparison.CurrentCultureIgnoreCase)) {
                this._connectionString = ConfigurationManager.ConnectionStrings[nameOrConnectionString.Substring(nameOrConnectionString.IndexOf("=") + 1)].ConnectionString;
            }
            else if (nameOrConnectionString.IndexOf(';') != -1 && nameOrConnectionString.IndexOf('=') != -1) {
                this._connectionString = nameOrConnectionString;
            }
            else {
                this._connectionString = ConfigurationManager.ConnectionStrings[nameOrConnectionString].ConnectionString;
            }

            this.Reconnect();
        }
        #endregion


        /// <summary>
        /// 尝试重新连接数据库
        /// </summary>
        public void Reconnect()
        {
            if (!_closeConnection)
                return;


            if (_connection == null) {
                _connection = CreateConnection(_connectionString);
            }
            if (_connection.State != ConnectionState.Open) {
                _connection.Open();

                Trace.WriteLine("open a connection: {0}.", _connectionString);
            }
        }

        /// <summary>
        /// 断开数据库连接
        /// </summary>
        public void Disconnect()
        {
            DisposeTransaction(_transaction);
            _transaction = null;

            if (_closeConnection) {
                CloseConnection(_connection);

                Trace.WriteLine("close the connection.");
            }
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public DbTransaction BeginTransaction()
        {
            Reconnect();

            if (_transaction == null) {
                _transaction = _connection.BeginTransaction();

                Trace.WriteLine("begin transaction.");
            }

            return _transaction;
        }
        
        /// <summary>
        /// 执行当前数据库连接对象的命令,指定参数.
        /// </summary>
        /// <remarks>
        /// 示例: int result = ExecuteNonQuery(CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParamter参数数组(无参数请写(DbParameter[])null)</param>
        /// <returns>返回命令影响的行数</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return this.ExecuteNonQuery(commandType, commandText, (IEnumerable<DbParameter>)commandParameters);
        }
        /// <summary>
        /// 执行当前数据库连接对象的命令,指定参数.
        /// </summary>
        public int ExecuteNonQuery(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
        {
            if(string.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");


            using (IDbCommand command = CreateCommandByCommandType(commandType, commandText, commandParameters)) {
                try {
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex) {
                    throw ex;
                }
                finally {
                    command.Parameters.Clear();
                }
            }
        }
        /// <summary>
        /// 执行当前数据库连接对象的数据阅读器,指定参数.
        /// </summary>
        /// <remarks>
        /// 示例: IDataReader dr = ExecuteReader(CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParamter参数数组(无参数请写(DbParameter[])null)</param>
        /// <returns>返回包含结果集的IDataReader</returns>
        public IDataReader ExecuteReader(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return this.ExecuteReader(commandType, commandText, (IEnumerable<DbParameter>)commandParameters);
        }
        /// <summary>
        /// 执行当前数据库连接对象的数据阅读器,指定参数.
        /// </summary>
        public IDataReader ExecuteReader(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
        {
            if(string.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");


            using (IDbCommand command = CreateCommandByCommandType(commandType, commandText, commandParameters)) {
                try {
                    return command.ExecuteReader();
                }
                catch (Exception ex) {
                    throw ex;
                }
                finally {
                    bool canClear = true;
                    foreach (IDataParameter parameter in command.Parameters) {
                        if (parameter.Direction != ParameterDirection.Input)
                            canClear = false;
                    }

                    if (canClear) {
                        command.Parameters.Clear();
                    }
                }
            }
        }
        /// <summary>
        /// 执行指定数据库连接对象的命令,指定参数,返回结果集中的第一行第一列.
        /// </summary>
        /// <remarks>
        /// 示例: int orderCount = (int)ExecuteScalar(CommandType.StoredProcedure, "GetOrderCount", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParamter参数数组(无参数请写(DbParameter[])null)</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return this.ExecuteScalar(commandType, commandText, (IEnumerable<DbParameter>)commandParameters);
        }
        /// <summary>
        /// 执行指定数据库连接对象的命令,指定参数,返回结果集中的第一行第一列.
        /// </summary>
        public object ExecuteScalar(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
        {
            if(string.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");


            using (IDbCommand command = CreateCommandByCommandType(commandType, commandText, commandParameters)) {
                try {
                    return command.ExecuteScalar();
                }
                catch (Exception ex) {
                    throw ex;
                }
                finally {
                    command.Parameters.Clear();
                }
            }
        }
        /// <summary>
        /// 执行指定数据库连接字符串的命令,指定参数值.返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例: DataSet ds = ExecuteDataset(CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParamter参数数组(无参数请写(DbParameter[])null)</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public virtual DataSet ExecuteDataset(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return this.ExecuteDataset(commandType, commandText, (IEnumerable<DbParameter>)commandParameters);
        }
        /// <summary>
        /// 执行指定数据库连接字符串的命令,指定参数值.返回DataSet.
        /// </summary>
        public abstract DataSet ExecuteDataset(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters);

        /// <summary>
        /// Command对象的传入参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="value">值</param>
        public DbParameter CreateInParameter(string paramName, object value)
        {
            DbParameter param = CreateParameter();
            param.ParameterName = BuildParameterName(paramName);
            param.Direction = ParameterDirection.Input;
            if (!(value == null || value == DBNull.Value))
                param.Value = value;

            return param;
        }

        /// <summary>
        /// Command对象的传入参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">数据长度</param>
        /// <param name="value">值</param>
        public DbParameter CreateInParameter(string paramName, DbType dbType, int size, object value)
        {
            return CreateParameter(paramName, dbType, ParameterDirection.Input, size, value);
        }
        /// <summary>
        /// Command对象的传入参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="value">值</param>
        public DbParameter CreateInParameter(string paramName, DbType dbType, object value)
        {
            return CreateInParameter(paramName, dbType, -1, value);
        }
        /// <summary>
        /// Command对象的传出参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">数据长度</param>
        public DbParameter CreateOutParameter(string paramName, DbType dbType, int size)
        {
            return CreateParameter(paramName, dbType, ParameterDirection.Output, size, null);
        }
        /// <summary>
        /// Command对象的传出参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        public DbParameter CreateOutParameter(string paramName, DbType dbType)
        {
            return CreateOutParameter(paramName, dbType, -1);
        }
        /// <summary>
        /// Command对象的传出参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        public DbParameter CreateOutParameter(string paramName)
        {
            DbParameter param = CreateParameter();
            param.ParameterName = BuildParameterName(paramName);
            param.Direction = ParameterDirection.Output;

            return param;
        }

        private DbParameter CreateParameter(string paramName, DbType dbType, ParameterDirection direction, int size, object value)
        {
            DbParameter param = CreateParameter();
            param.ParameterName = BuildParameterName(paramName);
            param.DbType = dbType;
            param.Direction = direction;
            if (size > 0)
                param.Size = size;
            if (!(direction == ParameterDirection.Output && value == null))
                param.Value = value;

            return param;
        }

        /// <summary>
        /// Builds a value parameter name for the current database.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A correctly formated parameter name.</returns>
        protected virtual string BuildParameterName(string name)
        {
            return name;
        }


        /// <summary>
        /// 释放数据库连接
        /// </summary>
        public void Dispose()
        {
            DisposeTransaction(_transaction);
            _transaction = null;
            if(_closeConnection) {
                DisposeConnection(_connection);
                _connection = null;
            }
        }


        
        

        public IDataTable CreateDataTable(string table)
        {
            throw new NotImplementedException();
        }
    }
}
