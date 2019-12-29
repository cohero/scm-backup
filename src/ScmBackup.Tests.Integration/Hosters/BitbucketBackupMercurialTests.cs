﻿using ScmBackup.Hosters.Bitbucket;
using ScmBackup.Http;
using ScmBackup.Scm;
using System.IO;
using Xunit;

namespace ScmBackup.Tests.Integration.Hosters
{
    public class BitbucketBackupMercurialTests : IBackupTests
    {
        private string prefix = "Bitbucket";

        internal override string PublicUserName { get { return "scm-backup-testuser"; } }
        internal override string PublicRepoName { get { return "scm-backup-test"; } }

        internal override string PrivateUserName { get { return TestHelper.EnvVar(prefix, "Name"); } }
        internal override string PrivateRepoName { get { return TestHelper.EnvVar(prefix, "RepoPrivate"); } }

        protected override void Setup(bool usePrivateRepo)
        {
            // re-use test repo for Api tests
            this.source = new ConfigSource();
            this.source.Hoster = "bitbucket";
            this.source.Type = "user";
            this.source.Name = this.GetUserName(usePrivateRepo);
            this.source.AuthName = TestHelper.EnvVar(prefix, "Name");
            this.source.Password = TestHelper.EnvVar(prefix, "PW");

            var config = new Config();
            config.Sources.Add(this.source);
            
            var context = new FakeContext();
            context.Config = config;
            this.context = context;
            this.logger = new FakeLogger();

            var api = new BitbucketApi(new HttpRequest());
            var repoList = api.GetRepositoryList(this.source);
            this.repo = repoList.Find(r => r.ShortName == this.GetRepoName(usePrivateRepo));
            
            this.scm = new MercurialScm(new FileSystemHelper(), context, new UrlHelper());
            Assert.True(this.scm.IsOnThisComputer());

            var scmFactory = new FakeScmFactory();
            scmFactory.Register(ScmType.Mercurial, this.scm);
            this.sut = new BitbucketBackup(scmFactory);
        }

        protected override void AssertRepo(string dir)
        {
            Assert.True(Directory.Exists(dir));
            Assert.True(this.scm.DirectoryIsRepository(dir));
            Assert.True(scm.RepositoryContainsCommit(dir, "617f9e55262be7b6d1c9db081ec351ff25c9a0e5"));
        }

        protected override void AssertWiki(string dir)
        {
            Assert.True(Directory.Exists(dir));
            Assert.True(this.scm.DirectoryIsRepository(dir));
            Assert.True(scm.RepositoryContainsCommit(dir, "befce8ddfb6976918c3c3e1a44fb6a68a438b785"));
        }

        protected override void AssertPrivateRepo(string dir)
        {
            Assert.True(Directory.Exists(dir));
            Assert.True(this.scm.DirectoryIsRepository(dir));
        }
    }
}
