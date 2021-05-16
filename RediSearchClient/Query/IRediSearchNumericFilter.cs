namespace RediSearchClient.Query
{
    public interface IRediSearchNumericFilter
    {
        string FieldName { get; }

        string Min { get; }

        string Max { get; }
    }
}