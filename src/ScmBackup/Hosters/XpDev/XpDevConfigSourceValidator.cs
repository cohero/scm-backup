namespace ScmBackup.Hosters.XpDev
{
    /// <summary>
    /// validator for Bitbucket repositories
    /// </summary>
    internal class XpDevConfigSourceValidator : ConfigSourceValidatorBase
    {
        public override string HosterName
        {
            get { return "xpdev"; }
        }

        public override bool AuthNameAndNameMustBeEqual
        {
            get { return true; }
        }
    }
}
