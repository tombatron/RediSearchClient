using System.Linq;

namespace RediSearchClient.Aggregate
{
    public sealed class RediSearchAggregateDefinition
    {
        internal object[] Fields { get; }

        internal RediSearchAggregateDefinition(object[] fields) =>
            Fields = fields;

        public override string ToString() => 
            string.Join(" ", Fields.Select(x => x.ToString()));
    }
}