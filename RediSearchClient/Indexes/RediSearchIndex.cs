using System;

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
		[Obsolete("Use the `OnHash` method instead.")]
		public static RediSearchHashIndexBuilder On(RediSearchStructure structure) => OnHash();

		/// <summary>
		/// Specify that an index be created for Redis Hash's.
		/// </summary>
		/// <returns></returns>
		public static RediSearchHashIndexBuilder OnHash() =>
			new RediSearchHashIndexBuilder();

		/// <summary>
		/// Specify that an index be created for Redis JSON objects.
		/// </summary>
		/// <returns></returns>
		public static RediSearchJsonIndexBuilder OnJson() =>
			new RediSearchJsonIndexBuilder();
	}
}