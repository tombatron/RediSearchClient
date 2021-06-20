using System.Collections.Generic;

namespace RediSearchClient.Aggregate
{
    /// <summary>
    /// This is the builder class used to construct a `GROUPBY` clause for an aggregation query.
    /// </summary>
    public sealed class GroupByBuilder
    {
        internal GroupByBuilder()
        {
        }

        private string[] _fields;

        /// <summary>
        /// Builder method for specifying which fields from a query result that you want to include in the "GROUPBY" 
        /// section of the aggregation query.
        /// </summary>
        /// <param name="fields">The field(s) to be included in the aggregation query.</param>
        public void Fields(params string[] fields)
        {
            _fields = fields;
        }

        private ReduceSpec _reduceSpec;

        /// <summary>
        /// Builder method for specifying the reducer and any associated properties. 
        /// </summary>
        /// <param name="func">The reducer function that is to be applied to the grouped fields.</param>
        /// <param name="args">Any arguments that are required by the specified reducer.</param>
        /// <returns></returns>
        public ReduceSpec Reduce(Reducer func, params string[] args)
        {
            _reduceSpec = new ReduceSpec(func, args);

            return _reduceSpec;
        }

        internal object[] Build()
        {
            var result = new List<object>();

            if (_fields != null)
            {
                result.Add("GROUPBY");
                result.Add(_fields.Length);
                result.AddRange(_fields);
            }

            if (_reduceSpec != null)
            {
                result.AddRange(_reduceSpec.Build());
            }

            return result.ToArray();
        }

        /// <summary>
        /// This is a continuation of the `GROUPBY` builder that allows us to better define a reducer by giving us the
        /// ability to optionally specify a field alias.
        /// </summary>
        public sealed class ReduceSpec
        {
            private readonly string _func;
            private readonly string[] _args;

            internal ReduceSpec(string func, string[] args)
            {
                _func = func;
                _args = args;
            }

            private string _alias;

            /// <summary>
            /// Builder method for specifying an alias for a reduced field.
            /// </summary>
            /// <param name="alias">The alias for the reduced field.</param>
            public void As(string alias)
            {
                _alias = alias;
            }

            internal object[] Build()
            {
                var result = new List<object>();

                result.Add("REDUCE");
                result.Add(_func);
                result.Add(_args.Length.ToString());
                result.AddRange(_args);

                if (!string.IsNullOrEmpty(_alias))
                {
                    result.Add("AS");
                    result.Add(_alias);
                }

                return result.ToArray();
            }
        }
    }
}