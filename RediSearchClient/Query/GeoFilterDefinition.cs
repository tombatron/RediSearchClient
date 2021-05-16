namespace RediSearchClient.Query
{
    internal class GeoFilterDefinition
    {
        internal string FieldName { get; }

        internal double Latitude { get; }

        internal double Longitude { get; }

        internal double Radius { get; }

        internal Unit DistanceUnit { get; }

        internal GeoFilterDefinition(string fieldName, double latitude, double longitude, double radius, Unit distanceUnit)
        {
            FieldName = fieldName;
            Latitude = latitude;
            Longitude = longitude;
            Radius = radius;
            DistanceUnit = distanceUnit;
        }
    }
}