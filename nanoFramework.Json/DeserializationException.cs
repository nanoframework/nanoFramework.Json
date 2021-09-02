using System;

namespace nanoFramework.Json
{
    /// <summary>
    /// Exception thrown when there is an problem with a deserialization operation. 
    /// Check the comment from where the exception has been throw to learn the cause.
    /// </summary>
    [Serializable]
    public class DeserializationException : Exception
    {
    }
}
