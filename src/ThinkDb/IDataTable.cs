using System.Collections;
using System.Collections.Generic;
using ThinkDb.Expressions;

namespace ThinkDb
{
    public interface IDataTable
    {
        /// <summary>
        /// 消除重复项
        /// </summary>
        IDataTable Distinct();
        /// <summary>
        /// 设置查询的字段
        /// </summary>
        IDataTable Select(Expression fields);
        //IDataTable Join(string table);
        /// <summary>
        /// 设定查询条件
        /// </summary>
        IDataTable Where(Expression wheres);
        /// <summary>
        /// 分组查询(前提是必须指定Fileds)
        /// </summary>
        IDataTable GroupBy();
        /// <summary>
        /// 过滤
        /// </summary>
        IDataTable Having(Expression havings);
        /// <summary>
        /// 排序
        /// </summary>
        IDataTable OrderBy(Expression orders);
        /// <summary>
        /// 显示分页数据
        /// </summary>
        IDataTable Limit(int page, int rows);

        /// <summary>
        /// 找到一条记录键与值的集合
        /// </summary>
        IDictionary Find();
        /// <summary>
        /// 找到所有记录集的集合
        /// </summary>
        ICollection FindAll();
        /// <summary>
        /// 找到所有记录集的集合并输出记录总数
        /// </summary>
        ICollection FindAll(out long total);
        /// <summary>
        /// 删除符合条件的数据
        /// </summary>
        bool Delete();
        /// <summary>
        /// 插入数据后并返回数据库提供的自动增量
        /// </summary>
        bool Insert(out long id);
        /// <summary>
        /// 插入数据
        /// </summary>
        bool Insert();
        /// <summary>
        /// 更新符合条件的数据
        /// </summary>
        bool Update();
    }
}
