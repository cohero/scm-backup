using ScmBackup.Hosters;
using ScmBackup.Http;
using System.Collections.Generic;

namespace ScmBackup
{
    using System;

    /// <summary>
    /// Backs up all repositories from a single source
    /// </summary>
    internal class BackupMaker : IBackupMaker
    {
        private readonly ILogger logger;
        private readonly IFileSystemHelper fileHelper;
        private readonly IHosterBackupMaker backupMaker;
        private readonly IContext context;

        public BackupMaker(ILogger logger, IFileSystemHelper fileHelper, IHosterBackupMaker backupMaker, IContext context)
        {
            this.logger = logger;
            this.fileHelper = fileHelper;
            this.backupMaker = backupMaker;
            this.context = context;
        }

        public void Backup(ConfigSource source, IEnumerable<HosterRepository> repos)
        {
            this.logger.Log(ErrorLevel.Info, Resource.BackupMaker_Source, source.Title);

            string sourceFolder = this.fileHelper.CreateSubDirectory(context.Config.LocalFolder, source.Title);

            var url = new UrlHelper();

            var errorCounter = 0;

            foreach (var repo in repos)
            {
                //counter++;
                try
                {
                    string repoFolder = this.fileHelper.CreateSubDirectory(sourceFolder, repo.FullName);

                    this.logger.Log(ErrorLevel.Info, Resource.BackupMaker_Repo, repo.Scm.ToString(), url.RemoveCredentialsFromUrl(repo.CloneUrl));

                    this.backupMaker.MakeBackup(source, repo, repoFolder, this.context, this.logger);
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    this.logger.Log(ErrorLevel.Error, ex, $"    **** Failed backing up repo {repo.FullName}. Continuing with other repos. ****");
                }
            }

            if (errorCounter > 0)
            {
                throw new Exception($"Backup completed with ({errorCounter}) errors. See log above for details. Completed as many repo backups as possible.");
            }
        }
    }
}
