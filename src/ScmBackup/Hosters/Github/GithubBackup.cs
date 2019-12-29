using ScmBackup.Scm;
using System;

namespace ScmBackup.Hosters.Github
{
    internal class GithubBackup : BackupBase
    {
        public GithubBackup(IScmFactory scmfactory)
        {
            this.scmFactory = scmfactory;
        }

        public override void BackupRepo(string subdir, ScmCredentials credentials)
        {
            this.logger.Log(ErrorLevel.Info, $"    Backing up repo...");

            InitScm();

            var output = scm.PullFromRemote(this.repo.CloneUrl, subdir, credentials);
            this.logger.LogCmdOutput(output);

            if (!scm.DirectoryIsRepository(subdir))
            {
                throw new InvalidOperationException(Resource.DirectoryNoRepo);
            }
        }

        public override void BackupWiki(string subdir, ScmCredentials credentials)
        {
            this.logger.Log(ErrorLevel.Info, $"    Backing up wiki...");

            InitScm();

            var output = scm.PullFromRemote(this.repo.WikiUrl, subdir, credentials);
            this.logger.LogCmdOutput(output);

            if (!scm.DirectoryIsRepository(subdir))
            {
                throw new InvalidOperationException(Resource.DirectoryNoRepo);
            }
        }
    }
}
