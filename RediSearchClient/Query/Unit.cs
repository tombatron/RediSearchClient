namespace RediSearchClient.Query
{
    /// <summary>
    /// Unit of distance for geo queries.
    /// </summary>
    public sealed class Unit
    {
        /// <summary>
        /// Miles.
        /// </summary>
        /// <returns></returns>
        public static Unit Miles = new Unit("mi");

        /// <summary>
        /// Meters.
        /// </summary>
        /// <returns></returns>
        public static Unit Meters = new Unit("m");

        /// <summary>
        /// Kilometers.
        /// </summary>
        /// <returns></returns>
        public static Unit Kilometers = new Unit("km");

        /// <summary>
        /// Feet.
        /// </summary>
        /// <returns></returns>
        public static Unit Feet = new Unit("ft");

        private readonly string _unitDefinition;

        private Unit(string unitDefinition) =>
            _unitDefinition = unitDefinition;

        /// <summary>
        /// String representation of the defined unit.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            _unitDefinition;
    }
}