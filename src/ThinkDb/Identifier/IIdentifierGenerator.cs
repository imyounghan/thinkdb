
namespace ThinkDb.Identifier
{
    /// <summary>
    /// 表示生成标识id的接口
    /// </summary>
    public interface IIdentifierGenerator
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 生成新的id
        /// </summary>
        object GenerateNewId(IDatabase database, object entity);
    }
}
