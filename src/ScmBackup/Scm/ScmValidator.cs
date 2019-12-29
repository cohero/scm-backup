using ScmBackup.Scm;
using System;
using System.Collections.Generic;

namespace ScmBackup.Scm
{
    /// <summary>
    /// Verifies that all passed SCMs are present on this machine
    /// </summary>
    internal class ScmValidator : IScmValidator
    {
        private readonly IScmFactory factory;
        private readonly ILogger logger;

        public ScmValidator(IScmFactory factory, ILogger logger)
        {
            this.factory = factory;
            this.logger = logger;
        }

        public bool ValidateScms(HashSet<ScmType> scms)
        {
            bool ok = true;
            this.logger.Log(ErrorLevel.Info, Resource.ScmValidatorStarting);

            foreach (var scmType in scms)
            {
                var scm = this.factory.Create(scmType);

                bool onComputer = false;
                try
                {
                    onComputer = scm.IsOnThisComputer();
                }
                catch (Exception ex)
                {
                    this.logger.Log(ErrorLevel.Error, ex, scm.DisplayName + ": ");
                }

                if (onComputer)
                {
                    this.logger.Log(ErrorLevel.Info, $"  FOUND: {scm.DisplayName} v{scm.GetVersionNumber()}");
                }
                else
                {
                    this.logger.Log(ErrorLevel.Error, $"  {scm.DisplayName} NOT FOUND");
                    ok = false;
                }
            }

            this.logger.Log(ErrorLevel.Info, string.Empty);

            return ok;
        }
    }
}
