using System;

namespace RediSearchClient.IntegrationTests
{
    public abstract class BaseIntegrationTest : IDisposable
    {
        public abstract void Setup();

        public abstract void TearDown();

        public BaseIntegrationTest()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}