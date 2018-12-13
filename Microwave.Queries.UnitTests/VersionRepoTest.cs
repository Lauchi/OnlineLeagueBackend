using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microwave.Queries.UnitTests
{
    [TestClass]
    public class VersionRepoTest
    {
        [TestMethod]
        public async Task VersionRepo_SaveAndLoad()
        {
            var options = new DbContextOptionsBuilder<QueryStorageContext>()
                .UseInMemoryDatabase("VersionRepo_SaveAndLoad")
                .Options;

            var versionRepository = new VersionRepository(new QueryStorageContext(options));

            await versionRepository.SaveVersion(new LastProcessedVersion("Type", 1));
            await versionRepository.SaveVersion(new LastProcessedVersion("Type", 2));

            var count = await versionRepository.GetVersionAsync("Type");
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task VersionRepo_DuplicateUpdate()
        {
            var options = new DbContextOptionsBuilder<QueryStorageContext>()
                .UseInMemoryDatabase("VersionRepo_DuplicateUpdate")
                .Options;

            var versionRepository = new VersionRepository(new QueryStorageContext(options));

            await versionRepository.SaveVersion(new LastProcessedVersion("Type", 1));
            await versionRepository.SaveVersion(new LastProcessedVersion("Type", 1));

            var count = await versionRepository.GetVersionAsync("Type");
            Assert.AreEqual(1, count);
        }
    }
}