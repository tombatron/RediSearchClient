using System;

namespace RediSearchClient.Exceptions
{
    /// <summary>
    /// Describes an exception thrown while setting RediSearch runtime configuration options. 
    /// </summary>
    public class RediSearchConfigurationException : Exception
    {
        /// <summary>
        /// Let's just throw an exception with no context. 
        /// </summary>
        public RediSearchConfigurationException()
        {

        }

        /// <summary>
        /// Throws an exception with a specific error inside for the end user to decipher. 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public RediSearchConfigurationException(string message) : base(message)
        {

        }

        /// <summary>
        /// Throws an exception wrapped around another exception, with the outer exception containing
        /// a specific error. 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <returns></returns>

        public RediSearchConfigurationException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}