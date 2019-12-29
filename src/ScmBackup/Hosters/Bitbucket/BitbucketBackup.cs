using ScmBackup.Scm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScmBackup.Hosters.Bitbucket
{
    internal class BitbucketBackup : BackupBase
    {
        public BitbucketBackup(IScmFactory scmfactory)
        {
            this.scmFactory = scmfactory;
        }

        public override void BackupRepo(string subdir, ScmCredentials credentials)
        {
            this.logger.Log(ErrorLevel.Info, $"    Backing up repo...");

            this.InitScm();

            var output = this.scm.PullFromRemote(this.repo.CloneUrl, subdir, credentials);
            this.logger.LogCmdOutput(output);

            if (!this.scm.DirectoryIsRepository(subdir))
            {
                throw new InvalidOperationException(Resource.DirectoryNoRepo);
            }
        }

        public override void BackupWiki(string subdir, ScmCredentials credentials)
        {
            this.logger.Log(ErrorLevel.Info, $"    Backing up wiki...");

            this.InitScm();

            var output = this.scm.PullFromRemote(this.repo.WikiUrl, subdir, credentials);
            this.logger.LogCmdOutput(output);

            if (!this.scm.DirectoryIsRepository(subdir))
            {
                throw new InvalidOperationException(Resource.DirectoryNoRepo);
            }
        }
    }
}
