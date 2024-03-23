namespace RediSearchClient.Indexes
{
    /// <summary>
    /// To judge how similar two objects are, we can compare their vector values, 
    /// by using various distance metrics. In the context of vector search, similarity 
    /// measures are a function that takes two vectors as input and calculates a 
    /// distance value between them. The distance can take many shapes, it can be 
    /// the geometric distance between two points, it could be an angle between the 
    /// vectors, it could be a count of vector component differences, etc. Ultimately, 
    /// we use the calculated distance to judge how close or far apart two vector 
    /// embeddings are. There are three distance metrics to measure the degree of 
    /// similarity between two vectors u, v ∈ R n where n is the length of the vectors:
    /// L2, IP, and COSINE...
    /// </summary>
    public enum DistanceMetric
    {
        /// <summary>
        /// Euclidean distance between two vectors d ( u, v) = ∑ i = 1 n ( u i − v i) 2
        /// </summary>
        L2 = 0,

        /// <summary>
        /// Inner product of two vectors d ( u, v) = 1 − u ⋅ v
        /// </summary>
        IP,

        /// <summary>
        /// Cosine distance of two vectors d ( u, v) = 1 − u ⋅ v ‖ u ‖ ‖ v ‖
        /// </summary>
        COSINE
    }
}
