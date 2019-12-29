namespace ScmBackup.Hosters.XpDev
{
    internal class XpDevHoster : IHoster
    {
        public XpDevHoster(IConfigSourceValidator validator, IHosterApi api, IBackup backup)
        {
            this.Validator = validator;
            this.Api = api;
            this.Backup = backup;
        }

        public IConfigSourceValidator Validator { get; private set; }

        public IHosterApi Api { get; private set; }

        public IBackup Backup { get; private set; }
    }
}
