#region Copyright and License

//  --------------------------------------------------------------------------------------------------------------------
//  <copyright company="Cohero" file="XpDevApi.cs" >
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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security;
    using System.Security.Authentication;
    using System.Text;

    using global::ScmBackup.Http;

    internal class XpDevApi : IHosterApi
    {
        #region Fields and Constants

        private readonly IHttpRequest request;

        #endregion

        #region Constructors and Destructors

        public XpDevApi(IHttpRequest request)
        {
            // Used by xpdev backup hoster & scm backup process
            this.request = request;
        }

        #endregion

        #region Public Methods

        public List<HosterRepository> GetRepositoryList(ConfigSource source)
        {
            // Called by CSM backup processes (rest in this class are called from bbtoxpdev hoster which instantiates this using a configsource and uses httpclient instead of httprequest because we have to post in addition to get (httprequest can only get)
            var list = new List<HosterRepository>();
            try
            {
                this.request.SetBaseUrl($"https://{source.Name}.xp-dev.com/api/v1/");

                if (source.IsAuthenticated)
                {
                    this.request.AddHeader("X-XPDevToken", source.ApiKey);
                }

                var url = "repositories";

                // Get list of all repos
                var result = this.request.Execute(url).Result;

                if (result.IsSuccessStatusCode)
                {
                    // If successful, add each to list of hoster repositories to be backed up
                    // Wikis not supported at XpDev
                    // Subversion (and any other type of scm other than Mercurial or Git) is not support for backup and will throw exception
                    var repoList = XpDevRepositoryResponse.ListFromJson(result.Content);

                    foreach (var repoResponse in repoList)
                    {
                        ScmType type;
                        switch (repoResponse.RepoType)
                        {
                            case RepoType.Mercurial:
                                type = ScmType.Mercurial;
                                break;
                            case RepoType.Git:
                                type = ScmType.Git;
                                break;
                            case RepoType.Subversion:
                                // Skip this repo type - ideally we'd log here, but don't know how in this framework - only option seems to be to throw and stop everything.
                                continue;
                            default:
                                throw new InvalidOperationException(string.Format(Resource.ApiInvalidScmType, repoResponse.RepoName));
                        }

                        // Add to list of hoster repositories that will be backed up
                        var hosterRepository = new HosterRepository(repoResponse.RepoName, repoResponse.RepoName, repoResponse.HttpsUrl(), type);
                        hosterRepository.SetPrivate(!repoResponse.DirectFileHosting); // Direct File Hosting is ALWAYS PUBLIC, else assume private - there doesn't appear to be any other indicator from XpDev API
                        list.Add(hosterRepository);
                    }
                }
                else
                {
                    // Throw Exception
                    var msg = string.Empty;
                    XpDevErrorMessageResponse errorResponse = null;
                    try
                    {
                        errorResponse = XpDevErrorMessageResponse.FromJson(result.Content);
                    }
                    catch
                    {
                        // Ignore
                    }

                    if (errorResponse != null)
                    {
                        msg = errorResponse.ErrorMessage;
                    }

                    switch (result.Status)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new AuthenticationException(string.Format(Resource.ApiAuthenticationFailed, "ApiKey") + $" ({msg})");
                        case HttpStatusCode.Forbidden:
                            throw new SecurityException(Resource.ApiMissingPermissions + $" ({msg})");
                        case HttpStatusCode.NotFound:
                            throw new InvalidOperationException(string.Format(Resource.ApiInvalidUsername, "ApiKey") + $" ({msg})");
                        default:
                            throw new Exception($"{msg} ({(int)result.Status} - {result.Status.ToString()})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return list;
        }
        #endregion

    }
}