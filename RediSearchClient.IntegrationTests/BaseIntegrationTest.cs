using System;
using System.Linq;
using StackExchange.Redis;
using static RediSearchClient.IntegrationTests.SampleData;

namespace RediSearchClient.IntegrationTests
{
    public abstract class BaseIntegrationTest : IDisposable
    {
        private static bool HasIndexCleanupRun = false;

        protected const string MovieDataPrefix = "movie::";

        private ConnectionMultiplexer _muxr;
        protected IDatabase _db;
        protected string _indexName;
        protected string _recordPrefix;
        protected string _dictionaryName;

        public virtual void Setup()
        {
            _muxr = ConnectionMultiplexer.Connect("localhost");

            _db = _muxr.GetDatabase(0);

            _indexName = Guid.NewGuid().ToString("n");
            _recordPrefix = Guid.NewGuid().ToString("n");
            _dictionaryName = Guid.NewGuid().ToString("n");

            SetupDemoMovieData();

            // SetupZipCodeData();

            CleanupIndexes();
        }

        public virtual void TearDown()
        {
            _muxr.Dispose();
        }

        public BaseIntegrationTest()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        private void SetupDemoMovieData()
        {
            if (_db.KeyExists($"{MovieDataPrefix}1"))
            {
                // Movies are already in the database, bail. 
                return;
            }

            for (var i = 0; i < Movies.Length; i++)
            {
                _db.HashSet($"{MovieDataPrefix}{i + 1}", Movies[i]);
            }
        }

        private void SetupZipCodeData()
        {
            if (_db.KeyExists("zip::32506"))
            {
                // Zipcodes are already loaded... probably. Bail anyway. 
                return;
            }

            foreach (var zipHash in SampleData.ZipCodes)
            {
                var key = $"zip::{zipHash[0].Value}";

                _db.HashSet(key, zipHash);
            }
        }

        private static object locker = new object();

        private void CleanupIndexes()
        {
            if (!HasIndexCleanupRun)
            {
                lock (locker)
                {
                    if (!HasIndexCleanupRun)
                    {
                        foreach (var index in _db.ListIndexes())
                        {
                            if(Guid.TryParse(index, out var _))
                            {
                                _db.DropIndex(index);
                            }
                        }

                        HasIndexCleanupRun = true;
                    }
                }
            }
        }
    }
}