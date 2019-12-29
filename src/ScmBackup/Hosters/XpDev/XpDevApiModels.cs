#region Copyright and License

//  --------------------------------------------------------------------------------------------------------------------
//  <copyright company="Cohero" file="XpDevApiModels.cs" >
//    Copyright © Cohero.  All rights reserved.
//  </copyright>
//  <summary>
//    Licensing and use of this code is subject to Cohero software license agreement terms.
//    For additional information, contact Cohero at (858) 777-1983 or info@cohero.com
//  </summary>
//  --------------------------------------------------------------------------------------------------------------------

#endregion

namespace ScmBackup.Hosters.XpDev
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;


    public enum ProjectType
    {
        Barebones,
        XPDev,
        Trac
    }

    public enum RepoType
    {
        Mercurial,
        Git,
        Subversion
    }

    public enum TransportType
    {
        HTTPS,
        SSH,
        WebDAV
    }

    public class XpDevProjectResponse
    {
        #region Public Methods

        public static XpDevProjectResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<XpDevProjectResponse>(json, XpDevJsonHelpers.Settings);
        }

        public static List<XpDevProjectResponse> ListFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<XpDevProjectResponse>>(json, XpDevJsonHelpers.Settings);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, XpDevJsonHelpers.Settings);
        }

        #endregion

        #region Serializable Properties and Fields (in serialization order, not sorted)

        [JsonProperty("id")]
        public long ProjectId { get; set; }

        [JsonProperty("name")]
        public string ProjectName { get; set; }

        [JsonProperty("fileAttachmentsSize")]
        public long FileAttachmentsSize { get; set; }

        [JsonProperty("bugStates")]
        public string BugStates { get; set; }

        [JsonProperty("abbreviatedName")]
        public string AbbreviatedName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public ProjectType ProjectType { get; set; }

        [JsonProperty("trac")]
        public XpDevProjTrac Trac { get; set; }

        #endregion
    }

    public class XpDevProjTrac
    {
        #region Serializable Properties and Fields (in serialization order, not sorted)

        [JsonProperty("created")]
        public bool Created { get; set; }
        
        [JsonProperty("notificationEnabled")]
        public bool NotificationEnabled { get; set; }

        [JsonProperty("incomingEmailEnabled")]
        public bool IncomingEmailEnabled { get; set; }

        [JsonProperty("timelineMaxDays")]
        public long TimelineMaxDays { get; set; }

        [JsonProperty("allowTicketDeletion")]
        public bool AllowTicketDeletion { get; set; }

        [JsonProperty("alwaysNotifyReporter")]
        public bool AlwaysNotifyReporter { get; set; }

        [JsonProperty("alwaysNotifyOwner")]
        public bool AlwaysNotifyOwner { get; set; }

        [JsonProperty("alwaysNotifyUpdater")]
        public bool AlwaysNotifyUpdater { get; set; }

        [JsonProperty("restrictDropDown")]
        public bool RestrictDropDown { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("alwaysCc")]
        public string AlwaysCc { get; set; }

        [JsonProperty("alwaysBcc")]
        public string AlwaysBcc { get; set; }

        [JsonProperty("subjectPrefix")]
        public string SubjectPrefix { get; set; }

        #endregion
    }

    public class XpDevRepositoryResponse
    {
        #region Serializable Properties and Fields (in serialization order, not sorted)

        [JsonProperty("id")]
        public long RepoId { get; set; }

        [JsonProperty("name")]
        public string RepoName { get; set; }

        [JsonProperty("type")]
        public RepoType RepoType { get; set; }

        [JsonProperty("project")]
        public XpDevRepoProject Project { get; set; }

        [JsonProperty("minimumCommentLength")]
        public long MinimumCommentLength { get; set; }

        [JsonProperty("allowRevisionPropertyChanges")]
        public bool AllowRevisionPropertyChanges { get; set; }

        [JsonProperty("commitEmails")]
        public List<string> CommitEmails { get; set; }

        [JsonProperty("webhookUrl")]
        public string WebhookUrl { get; set; }

        [JsonProperty("commitEmailsSubject")]
        public string CommitEmailsSubject { get; set; }

        [JsonProperty("commitEmailsPlainText")]
        public bool CommitEmailsPlainText { get; set; }

        [JsonProperty("allowedIpAddresses")]
        public string AllowedIpAddresses { get; set; }

        [JsonProperty("transports")]
        public List<XpDevRepoTransport> Transports { get; set; }

        [JsonProperty("directFileHosting")]
        public bool DirectFileHosting { get; set; }

        [JsonProperty("noDiffsInCommitEmails")]
        public bool NoDiffsInCommitEmails { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>Returns the first HTTPS Url for the repository (if there is one, else blank string).</summary>
        /// <returns>Repository URL or blank string.</returns>
        public string HttpsUrl()
        {
            return this.Transports?.FirstOrDefault(x => x.TransportType == TransportType.HTTPS)?.Url ?? string.Empty;
        }

        /// <summary>Returns the first SSH Url for the repository (if there is one, else blank string).</summary>
        /// <returns>Repository URL or blank string.</returns>
        public string SSHUrl()
        {
            return this.Transports?.FirstOrDefault(x => x.TransportType == TransportType.SSH)?.Url ?? string.Empty;
        }

        /// <summary>Returns the first WebDAV Url for the repository (if there is one, else blank string).</summary>
        /// <returns>Repository URL or blank string.</returns>
        public string WebDAVUrl()
        {
            return this.Transports?.FirstOrDefault(x => x.TransportType == TransportType.WebDAV)?.Url ?? string.Empty;
        }

        public static List<XpDevRepositoryResponse> ListFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<XpDevRepositoryResponse>>(json, XpDevJsonHelpers.Settings);
        }

        public static XpDevRepositoryResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<XpDevRepositoryResponse>(json, XpDevJsonHelpers.Settings);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, XpDevJsonHelpers.Settings);
        }

        #endregion
    }

    public class XpDevRepoProject
    {
        #region Serializable Properties and Fields (in serialization order, not sorted)

        #region Serializable Properties and Fields (in serialization order, not sorted)

        [JsonProperty("id")]
        public long? ProjectId { get; set; }

        #endregion

        #endregion
    }

    public class XpDevRepoTransport
    {
        #region Serializable Properties and Fields (in serialization order, not sorted)

        [JsonProperty("type")]
        public TransportType TransportType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        #endregion
    }

    public class NewRepoCreateRequest
    {
        #region Serializable Properties and Fields (in serialization order, not sorted)

        [JsonProperty("project")]
        public ExistingProject Project { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public RepoType Type { get; set; }

        [JsonProperty("createInitialDirectories")]
        public bool CreateInitialDirectories { get; set; }

        #endregion

        #region Public Methods

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, XpDevJsonHelpers.Settings);
        }

        #endregion
    }

    public class ExistingProject
    {
        #region Serializable Properties and Fields (in serialization order, not sorted)

        [JsonProperty("id")]
        public long Id { get; set; }

        #endregion
    }

    public class NewProjectCreateRequest
    {
        #region Serializable Properties and Fields (in serialization order, not sorted)

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public ProjectType Type { get; set; }

        [JsonProperty("abbreviatedName")]
        public string AbbreviatedName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        #endregion

        #region Public Methods

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, XpDevJsonHelpers.Settings);
        }

        #endregion
    }
}