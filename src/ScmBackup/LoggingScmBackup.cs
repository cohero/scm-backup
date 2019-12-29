namespace ScmBackup
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;

    internal class LoggingScmBackup : IScmBackup
    {
        private readonly IScmBackup backup;
        private readonly IContext context;
        private readonly ILogger logger;

        public LoggingScmBackup(IScmBackup backup, IContext context, ILogger logger)
        {
            this.backup = backup;
            this.context = context;
            this.logger = logger;
        }

        public void Run()
        {
            logger.Log(ErrorLevel.Info, this.context.AppTitle);
            logger.Log(ErrorLevel.Info, $"Cohero custom version of SCM Backup from {Resource.AppWebsite}");
            logger.Log(ErrorLevel.Info, string.Empty);

            // TODO: log more stuff (operating system, configuration...)
            try
            {
                var userName = Environment.UserName;
                var dnsName = Dns.GetHostEntry("localhost").HostName;
                var machineName = Environment.MachineName;
                logger.Log(ErrorLevel.Info, $"Executing Server DNS:     {dnsName}");
                logger.Log(ErrorLevel.Info, $"Executing Server Machine: {machineName}");
                var path = Assembly.GetExecutingAssembly().CodeBase;
                var directory = Path.GetDirectoryName(path);
                logger.Log(ErrorLevel.Info, $"Executing Location:       {directory}");
                logger.Log(ErrorLevel.Info, $"Executing User:           {userName}");
            }
            catch (Exception ex)
            {
                logger.Log(ErrorLevel.Error, $"Error logging environment information, continuing anyway ({ex.Message})");
            }
            logger.Log(ErrorLevel.Info, string.Empty);

            this.backup.Run();

            logger.Log(ErrorLevel.Info, string.Empty);
            logger.Log(ErrorLevel.Info, Resource.BackupFinished);
            logger.Log(ErrorLevel.Info, string.Format(Resource.BackupFinishedDirectory, this.context.Config.LocalFolder));
            logger.Log(ErrorLevel.Info, string.Empty);
        }
    }
}
