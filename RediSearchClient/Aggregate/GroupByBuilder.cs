using System.Collections.Generic;

namespace RediSearchClient.Aggregate
{
    public sealed class GroupByBuilder
    {
        internal GroupByBuilder()
        {
        }

        private string[] _fields;

        public void Fields(params string[] fields)
        {
            _fields = fields;
        }

        private ReduceSpec _reduceSpec;

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