using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ThinkDb
{
    /// <summary>
    /// 表示一个数据连接操作
    /// </summary>
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// 重新连接数据库
        /// </summary>
        void Reconnect();
        /// <summary>
        /// 断开数据库连接
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 开始事务
        /// </summary>
        DbTransaction BeginTransaction();
        /// <summary>
        /// 获取当前的数据库连接
        /// </summary>
        DbConnection DbConnection { get; }
        /// <summary>
        /// 获取当前执行的事务
        /// </summary>
        DbTransaction DbTransaction { get; }

        ///// <summary>
        ///// 获取自动增量的T-SQL
        ///// </summary>
        //string GetIdentitySql();
        ///// <summary>
        ///// 获取指定表名的结构
        ///// </summary>
        //TableSchemas[] GetTableSchemas(string tableName);


        /// <summary>
        /// 执行当前数据库连接对象的命令,指定参数.
        /// </summary>
        /// <remarks>
        /// 示例: int result = ExecuteNonQuery(CommandType.StoredProcedure, "PublishOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本,其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">分配给命令的DbParamter参数数组(无参数请写(DbParameter[])null)</param>
        /// <returns>返回命令影响的行数</returns>
        int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行当前数据库连接对象的命令,指定参数.
        /// </summary>
        int ExecuteNonQuery(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters);

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
        IDataReader ExecuteReader(CommandType commandType, string commandText, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行当前数据库连接对象的数据阅读器,指定参数.
        /// </summary>
        IDataReader ExecuteReader(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters);

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
        object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行指定数据库连接对象的命令,指定参数,返回结果集中的第一行第一列.
        /// </summary>
        object ExecuteScalar(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters);

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
        DataSet ExecuteDataset(CommandType commandType, string commandText, params DbParameter[] commandParameters);
        /// <summary>
        /// 执行指定数据库连接字符串的命令,指定参数值.返回DataSet.
        /// </summary>
        DataSet ExecuteDataset(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters);

        /// <summary>
        /// Command对象的传入参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="value">值</param>
        DbParameter CreateInParameter(string paramName, object value);
        /// <summary>
        /// Command对象的传入参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="value">值</param>
        DbParameter CreateInParameter(string paramName, DbType dbType, object value);
        /// <summary>
        /// Command对象的传入参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">数据长度</param>
        /// <param name="value">值</param>
        DbParameter CreateInParameter(string paramName, DbType dbType, int size, object value);
        
        /// <summary>
        /// Command对象的传出参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">数据长度</param>
        DbParameter CreateOutParameter(string paramName, DbType dbType, int size);
        /// <summary>
        /// Command对象的传出参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="dbType">数据类型</param>
        DbParameter CreateOutParameter(string paramName, DbType dbType);
        /// <summary>
        /// Command对象的传出参数
        /// </summary>
        /// <param name="paramName">参数名称</param>
        DbParameter CreateOutParameter(string paramName);

        IDataTable CreateDataTable(string table);
    }
}
