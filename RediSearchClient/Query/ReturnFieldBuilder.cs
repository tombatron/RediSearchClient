using System.Collections.Generic;

namespace RediSearchClient.Query
{
    /// <summary>
    /// Builder used for defining return fields and their optional aliases. 
    /// </summary>
    public sealed class ReturnFieldBuilder
    {
        private List<(string fieldName, string alias)> _returnFields;

        internal ReturnFieldBuilder()
        {
            _returnFields = new List<(string fieldName, string alias)>();
        }

        internal int FieldCount { get; private set; }

        /// <summary>
        /// Specify a field and an optional field alias to return with your query.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="alias"></param>
        public void Field(string fieldName, string alias = null)
        {
            FieldCount += 3;

            if (alias is null)
            {
                alias = fieldName;
            }

            _returnFields.Add((fieldName, alias));
        }

        internal object[] ReturnParameters()
        {
            var parameters = new List<object>
            {
                "RETURN",
                (_returnFields.Count * 3) // {fieldName} AS {alias}
            };

            foreach(var (fieldName, alias) in _returnFields)
            {
                parameters.Add(fieldName);
                parameters.Add("AS");
                parameters.Add(alias);
            }

            return parameters.ToArray();
        }
    }
}
