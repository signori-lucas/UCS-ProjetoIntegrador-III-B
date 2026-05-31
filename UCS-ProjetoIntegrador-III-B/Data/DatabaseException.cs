using System;

namespace UCS_ProjetoIntegrador_III_B.Data
{
    /// <summary>
    /// Exception thrown when there is an error interacting with the database.
    /// Contains a friendly message suitable for UI display and the original exception as inner.
    /// </summary>
    public class DatabaseException : Exception
    {
        public DatabaseException(string message, Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
