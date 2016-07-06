﻿namespace ScmBackup.Tests.Hosters
{
    internal class FakeConfigSourceValidator : IConfigSourceValidator
    {
        public bool WasValidated { get; private set; }

        public ValidationResult Result { get; set; }

        public ValidationResult Validate(ConfigSource config)
        {
            this.WasValidated = true;
            return this.Result;
        }
    }
}