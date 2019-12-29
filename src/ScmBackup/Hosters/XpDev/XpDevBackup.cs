using ScmBackup.Scm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScmBackup.Hosters.XpDev
{
    using System.Linq;

    internal class XpDevBackup : BackupBase
    {
        public XpDevBackup(IScmFactory scmfactory)
        {
            this.scmFactory = scmfactory;
        }

        public override void PushRepo(ConfigSource pushToConfigSource, string subdir, HosterRepository sourceRepo, ILogger sourceLogger)
        {
            this.source = pushToConfigSource;
            this.logger = sourceLogger;
            this.repo = sourceRepo;

            if (this.scmFactory == null)
            {
                throw new ArgumentNullException(nameof(this.scmFactory));
            }

            this.InitScm();

            var credentials = new ScmCredentials(this.source.AuthName, this.source.Password);

            // Create repo 
            var api = new XpDevApiExtended(pushToConfigSource, sourceLogger);
            var repoRequest = api.CreateRepoRequest(this.repo.Name, this.repo.Name, this.repo.ShortName, $"{this.repo.Description} [{this.repo.Language.ToUpper()}]", Enum.Parse<RepoType>(this.repo.Scm.ToString()), false, false, true);

            // Push
            this.logger.Log(ErrorLevel.Info, $"    Pushing to [{pushToConfigSource.Hoster}]...");
            var output = this.scm.PushToRemote(repoRequest.HttpsUrl(), subdir, credentials);
            this.logger.LogCmdOutput(output);
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
