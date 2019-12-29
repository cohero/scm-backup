using ScmBackup.Scm;
using System;
using System.IO;

namespace ScmBackup.Hosters
{
    using System.Linq;

    using global::ScmBackup.CompositionRoot;

    internal abstract class BackupBase : IBackup
    {
        public readonly string SubDirRepo = "repo";
        public readonly string SubDirWiki = "wiki";
        public readonly string SubDirIssues = "issues";

        protected IContext context;
        protected HosterRepository repo;
        protected IScm scm;
        protected ConfigSource source;
        protected ILogger logger;

        // this MUST be filled in the child classes' constructor
        public IScmFactory scmFactory;

        public void MakeBackup(ConfigSource source, HosterRepository repo, string repoFolder, IContext context, ILogger logger)
        {
            this.context = context;
            this.source = source;
            this.logger = logger;

            if (this.scmFactory == null)
            {
                throw new ArgumentNullException("!!");
            }

            ScmCredentials credentials = null;
            if (repo.IsPrivate)
            {
                credentials = new ScmCredentials(source.AuthName, source.Password);
            }

            this.repo = repo;

            string subdir = Path.Combine(repoFolder, this.SubDirRepo);

            // Backup Repo
            this.BackupRepo(subdir, credentials);

            // Push backup to another remote hoster
            if (!string.IsNullOrWhiteSpace(source.PushCopyToTitle))
            {
                var pushToConfigSource = this.context.Config.Sources.FirstOrDefault(x => x.Title.ToLower() == this.source.PushCopyToTitle.ToLower());
                if (pushToConfigSource == null)
                {
                    this.logger.Log(ErrorLevel.Warn, $"    **** Unable to locate [{this.source.PushCopyToTitle}] config source in settings.yml. Unable to push backup copy of repo ****");
                    return;
                }
                else
                {
                    try
                    {
                        // Get hoster instance defined by pushToConfigSource.Hoster and call PushRepo
                        var container = Bootstrapper.BuildContainer();
                        var factory = container.GetInstance<IHosterFactory>();
                        var pushHoster = factory.Create(pushToConfigSource.Hoster);
                        pushHoster.Backup.PushRepo(pushToConfigSource, subdir, this.repo, this.logger);
                    }
                    catch (Exception ex)
                    {
                        this.logger.Log(ErrorLevel.Error, ex, $"    **** PUSH FAILED: {ex.Message} ****");
                    }
                }
            }

            // Backup Wiki
            if (this.repo.HasWiki)
            {
                subdir = Path.Combine(repoFolder, this.SubDirWiki);
                this.BackupWiki(subdir, credentials);
            }

            // Backup Issues
            if (this.repo.HasIssues)
            {
                subdir = Path.Combine(repoFolder, this.SubDirIssues);
                this.BackupIssues(subdir, credentials);
            }

            this.logger.Log(ErrorLevel.Info, $"    DONE\n");
        }

        public void InitScm()
        {
            if (this.scm == null)
            {
                this.scm = this.scmFactory.Create(this.repo.Scm);
                if (!this.scm.IsOnThisComputer())
                {
                    throw new InvalidOperationException(string.Format(Resource.ScmNotOnThisComputer, this.repo.Scm.ToString()));
                }
            }
        }

        // this MUST be implemented in the child classes
        public abstract void BackupRepo(string subdir, ScmCredentials credentials);

        // optional if hoster supports pushing to repo
        public virtual void PushRepo(ConfigSource pushToConfigSource, string subdir, HosterRepository sourceRepo, ILogger sourceLogger)
        {
            this.logger.Log(ErrorLevel.Info, $"    (pushing requested but not supported by hoster type [{pushToConfigSource.Hoster}])");
        }

        // these can be implemented in the child classes IF the given hoster supports issues, a wiki...
        public virtual void BackupWiki(string subdir, ScmCredentials credentials)
        {
            this.logger.Log(ErrorLevel.Info, $"    (wiki backup requested but not supported in code)");

        }

        public virtual void BackupIssues(string subdir, ScmCredentials credentials)
        {
            this.logger.Log(ErrorLevel.Info, $"    (issue backup requested but not supported in code)");
        }
    }
}
