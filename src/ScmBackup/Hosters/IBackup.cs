namespace ScmBackup.Hosters
{
    using global::ScmBackup.Scm;

    internal interface IBackup
    {
        /// <summary>
        /// backup everything from this repo which needs to be backed up
        /// </summary>
        void MakeBackup(ConfigSource source, HosterRepository repo, string repoFolder, IContext context, ILogger logger);

        /// <summary>
        /// Pushes the repo to an SCM. Required in the IBackup interface to allow calling on a factory generated hoster backup object.
        /// </summary>
        /// <param name="pushToConfigSource">The push to configuration source.</param>
        /// <param name="subdir">The subdir.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="sourceRepo">The source repo.</param>
        /// <param name="sourceLogger">The source logger.</param>
        void PushRepo(ConfigSource pushToConfigSource, string subdir, HosterRepository sourceRepo, ILogger sourceLogger);
    }
}
