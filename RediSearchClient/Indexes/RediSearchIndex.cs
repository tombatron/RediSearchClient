namespace RediSearchClient.Indexes
{
	/// <summary>
	/// This is a factory of sorts that kicks off the index builder.
	/// </summary>
    public static class RediSearchIndex
    {
		/// <summary>
		/// Builder method for specifying the structure that we're going to apply the index to.
		/// </summary>
		/// <param name="structure">Hash or (in the future) JSON</param>
		/// <returns></returns>
        public static RediSearchIndexBuilder On(RediSearchStructure structure) =>
            new RediSearchIndexBuilder(structure);
    }
}