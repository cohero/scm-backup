using ScmBackup.Hosters;
using Xunit;

namespace ScmBackup.Tests.Hosters
{
    using System.Linq;

    public class BackupBaseTests
    {
        [Fact]
        public void BackupBaseExecutesAllSubMethods()
        {
            var repo = new HosterRepository("foo", "foo", "http://clone", ScmType.Git);
            repo.SetWiki(true, "http://wiki");
            repo.SetIssues(true, "http://issues");

            var reader = new FakeConfigReader();
            reader.SetDefaultFakeConfig();

            var context = new Context(reader);
            var logger = new FakeLogger();

            var sut = new FakeHosterBackup();
            sut.MakeBackup(new ConfigSource(), repo, @"c:\foo", context, logger);

            Assert.True(sut.WasExecuted);
        }

    }
}

