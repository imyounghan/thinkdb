using System;

namespace ThinkDb
{
    /// <summary>
    /// 数据访问异常 
    /// </summary>
    public class DatabaseException : Exception
    {
        /// <summary>Default constructor.
        /// </summary>
        public DatabaseException() { }
        /// <summary>Parameterized constructor.
        /// </summary>
        public DatabaseException(string message) :
            base(message)
        { }
        /// <summary>Parameterized constructor.
        /// </summary>
        public DatabaseException(string message, Exception innerException) :
            base(message, innerException)
        { }
        /// <summary>Parameterized constructor.
        /// </summary>
        public DatabaseException(string message, params object[] args) :
            base(string.Format(message, args))
        { }
    }
}
