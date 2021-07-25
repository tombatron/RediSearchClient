using System;

namespace RediSearchClient.Exceptions
{
    /// <summary>
    /// Describes an exception that can be throw during result map configuration.
    /// </summary>
    public class ResultMapperConfigurationException : Exception
    {
        /// <summary>
        /// Exception with no initial context. 
        /// </summary>
        public ResultMapperConfigurationException() { }

        /// <summary>
        /// Initialize an exception with a custom message. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultMapperConfigurationException(string message) : base(message) { }

        /// <summary>
        /// Initial an exception with a custom message and an inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        public ResultMapperConfigurationException(string message, Exception inner) : base(message, inner) { }
    }
}