using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microwave.EventStores;
using Microwave.Queries;
using MongoDB.Driver;

namespace Microwave.Eventstores.UnitTests
{
    public class IntegrationTests
    {
        protected EventDatabase EventDatabase;
        protected ReadModelDatabase ReadModelDatabase;

        [TestInitialize]
        public void SetupMongoDb()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            EventDatabase = new EventDatabase(client.GetDatabase("IntegrationTest"));
            ReadModelDatabase = new ReadModelDatabase(client.GetDatabase("IntegrationTest"));
            client.DropDatabase("IntegrationTest");
        }
    }
}