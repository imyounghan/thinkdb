using System.Data;

namespace ThinkDb
{
    /// <summary>
    /// 表示数据表中列的架构
    /// </summary>
    public class TableSchemas
    {
        /// <summary>
        /// 获取或设置字段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置字段的数据类型。
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示字段是否允许空值。
        /// </summary>
        public bool AllowDBNull { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示字段是否为自动增量
        /// </summary>
        public bool AutoIncrement { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示字段是否为主键
        /// </summary>
        public bool Primary { get; set; }

        /// <summary>
        /// 获取该字段对应的数据类型的 <see cref="DbType"/>
        /// </summary>
        public DbType GetDbTypeForParameter()
        {
            switch (this.DataType.ToLower()) {
                case "bit":
                    return DbType.Boolean;
                case "tinyint":
                    return DbType.SByte;
                case "smallint":
                    return DbType.Int16;
                case "int":
                    return DbType.Int32;
                case "long":
                    return DbType.Int32;
                case "float":
                    return DbType.Double;
                case "real":
                    return DbType.Single;
                case "datetime":
                case "smalldatetime":
                    return DbType.DateTime;
                case "money":
                case "smallmoney":
                    return DbType.Currency;
                case "binary":
                case "varbinary":
                case "image":
                    return DbType.Binary;
                case "uniqueidentifier":
                    return DbType.Guid;
                case "numeric":
                case "decimal":
                    return DbType.Decimal;
                case "char":
                case "nchar":
                    return DbType.AnsiStringFixedLength;
                case "varchar":
                case "nvarchar":
                    return DbType.AnsiString;
                case "xml":
                    return DbType.Xml;
                default:
                    return DbType.Object;
            }
        }
    }
}
