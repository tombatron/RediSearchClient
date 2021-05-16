namespace RediSearchClient.Query
{
    public sealed class Unit
    {
        public static Unit Miles = new Unit("mi");
        public static Unit Meters = new Unit("m");
        public static Unit Kilometers = new Unit("km");
        public static Unit Feet = new Unit("ft");

        private readonly string _unitDefinition;

        private Unit(string unitDefinition) =>
            _unitDefinition = unitDefinition;

        public override string ToString() =>
            _unitDefinition;
    }
}