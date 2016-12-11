
namespace ThinkDb.Sql
{
    /// <summary>
    /// Represents a literal SQL part
    /// </summary>
    public class SqlLiteralPart : SqlPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLiteralPart"/> class.
        /// </summary>
        /// <param name="literal">The literal.</param>
        public SqlLiteralPart(string literal)
        {
            Literal = literal;
        }

        /// <summary>
        /// The resulting SQL string
        /// </summary>
        /// <value></value>
        public override string Sql { get { return Literal; } }

        /// <summary>
        /// Literal SQL used as is
        /// </summary>
        public string Literal { get; private set; }

        /// <summary>
        /// Creates a SqlLiteralPart from a given string (implicit)
        /// </summary>
        public static implicit operator SqlLiteralPart(string literal)
        {
            return new SqlLiteralPart(literal);
        }
    }
}
