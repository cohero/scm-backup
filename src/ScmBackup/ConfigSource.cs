using System;
using System.Collections.Generic;

namespace ScmBackup
{
    /// <summary>
    /// Configuration data to get the repositories of user X from hoster Y
    /// (subclass for Config)
    /// </summary>
    internal class ConfigSource
    {
        /// <summary>
        /// title of this config source (must be unique)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Title another config source to push copy of the repo to. Title must exist in the setting.yml, and the source type
        /// for the title must support pushing and support the same repo type (no repo hg to git etc. conversion.) Used to copy a repo from one hoster to another in addition to making a local backup.
        /// </summary>
        public string PushCopyToTitle { get; set; }

        /// <summary>
        /// name of the hoster
        /// </summary>
        public string Hoster { get; set; }

        /// <summary>
        /// user type (e.g. user/team)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// user name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ApiKey used by some Hosters to access API for repo list. (Currently only applies to Xp-Dev.com hoster.)
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// user name for authentication
        /// (can be a different than the user whose repositories are backed up)
        /// </summary>
        public string AuthName { get; set; }

        /// <summary>
        /// list of repository names which should be ignored
        /// </summary>
        public List<string> IgnoreRepos { get; set; }

        /// <summary>
        /// password for authentication
        /// </summary>
        public string Password { get; set; }

        public bool IsAuthenticated
        {
            get
            {
                return !String.IsNullOrWhiteSpace(this.AuthName) && !String.IsNullOrWhiteSpace(this.Password);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var source = obj as ConfigSource;

            if (source == null)
            {
                return false;
            }

            return (source.Title == this.Title);
        }

        public override int GetHashCode()
        {
            return this.Title.GetHashCode();
        }
    }
}
