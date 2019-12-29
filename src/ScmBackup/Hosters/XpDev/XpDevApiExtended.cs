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
    using System.Net.Http.Headers;
    using System.Security;
    using System.Security.Authentication;
    using System.Text;
    using System.Text.RegularExpressions;

    using global::ScmBackup.Http;

    internal class XpDevApiExtended
    {
        #region Fields and Constants

        private readonly HttpClient client;
        private readonly ConfigSource configSource;
        private List<XpDevProjectResponse> projectList = null;
        private List<XpDevRepositoryResponse> repoList = null;
        private readonly ILogger logger;

        #endregion

        #region Constructors and Destructors

        public XpDevApiExtended(ConfigSource source, ILogger logger)
        {
            // used by bbtoxpdev backup hoster (to call create repo request, etc) to mirror bitbucket to xpdev - a temp process
            this.client = new HttpClient();
            this.configSource = source;
            this.logger = logger;

            var baseUri = new Uri($"https://{this.configSource.Name}.xp-dev.com/api/v1/");

            this.client.BaseAddress = baseUri;
            this.client.DefaultRequestHeaders.Add("X-XPDevToken", this.configSource.ApiKey);
            //this.client.DefaultRequestHeaders.Add("Content-Type", "application/json"); // Set when content is create with new StringContent
            this.client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        #endregion

        #region Public Methods

        public XpDevProjectResponse CreateProjectRequest(string name, ProjectType projectType, string abbreviatedName, string description, bool throwIfRepoPreExists, bool forceListRefresh)
        {
            // Does project already exist - if so return existing project record, else create new and return new project record
            XpDevProjectResponse project;
            var json = string.Empty;
            try
            {
                project = this.ListProjects(forceListRefresh).FirstOrDefault(x => x.ProjectName.ToLower() == name.ToLower());
                if (project != null)
                {
                    if (throwIfRepoPreExists)
                    {
                        throw new DuplicateNameException($"Project [{name}] already exists. Failed to create new project.");
                    }
                    else
                    {
                        return project;
                    }
                }

                // Clean up abbreviated name - only alphanumeric, dash, underscore and period
                Regex rgx = new Regex(@"[^A-Za-z0-9-\\_\\.]");
                abbreviatedName = rgx.Replace(abbreviatedName, "-");

                this.logger.Log(ErrorLevel.Info, $"    Creating new project [{name} at xp-dev]...");
                var newProject = new NewProjectCreateRequest() { Name = name, Type = projectType, AbbreviatedName = abbreviatedName, Description = description };
                var url = "projects";
                HttpResponseMessage result;
                json = newProject.ToJson();
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    result = this.client.PostAsync(url, content).Result;
                }

                if (result.IsSuccessStatusCode)
                {
                    project = XpDevProjectResponse.FromJson(result.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    ThrowOnApiFailure(result);
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("repo-request-json", json);
                Console.WriteLine(ex.Message);
                this.logger.Log(ErrorLevel.Error, ex, $"    **** EXCEPTION CREATING PROJECT: {ex.Message} ****");
                this.logger.Log(ErrorLevel.Error, $"    **** JSON: {json} ****");
                this.projectList = null;
                throw;
            }

            return project;
        }

        public XpDevRepositoryResponse CreateRepoRequest(string projectName, string repoName, string abbreviatedName, string description, RepoType repoType, bool createInitialDirectories, bool throwIfRepoPreExists, bool forceListRefresh)
        {
            // Does repo already exist? if so, return existing repo record, else create and return new repo record
            XpDevRepositoryResponse repo = null;
            var json = string.Empty;
            try
            {
                repo = this.ListRepos(forceListRefresh).FirstOrDefault(x => x.RepoName.ToLower() == repoName.ToLower());
                if (repo != null)
                {
                    if (throwIfRepoPreExists)
                    {
                        throw new DuplicateNameException($"Repo [{repoName}] already exists. Create repo failed to create new repo.");
                    }
                    else
                    {
                        return repo;
                    }
                }

                // Does project exist? create if not, else get project ID from project record
                var project = this.CreateProjectRequest(projectName, ProjectType.Barebones, abbreviatedName, description, throwIfRepoPreExists, forceListRefresh);

                this.logger.Log(ErrorLevel.Info, $"    Creating new repo [{repoName} at xp-dev]...");
                var newRepo = new NewRepoCreateRequest() { Project = new ExistingProject() { Id = project.ProjectId }, Name = repoName, Type = repoType, CreateInitialDirectories = createInitialDirectories };
                var url = "repositories";
                HttpResponseMessage result;
                json = newRepo.ToJson();
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    result = this.client.PostAsync(url, content).Result;
                }

                if (result.IsSuccessStatusCode)
                {
                    repo = XpDevRepositoryResponse.FromJson(result.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    ThrowOnApiFailure(result);
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("repo-request-json", json);
                Console.WriteLine(ex.Message);
                this.logger.Log(ErrorLevel.Error, ex, $"    **** EXCEPTION CREATING REPO: {ex.Message} ****");
                this.logger.Log(ErrorLevel.Error, $"    **** JSON: {json} ****");
                throw;
            }

            return repo;
        }

        public List<XpDevProjectResponse> ListProjects(bool forceListUpdate = true)
        {
            // Check for cached list and make sure a forced list update was not requested
            if (this.projectList != null && !forceListUpdate)
            {
                return this.projectList;
            }

            try
            {
                var url = "projects";
                var result = this.client.GetAsync(url).Result;

                if (result.IsSuccessStatusCode)
                {
                    this.projectList = XpDevProjectResponse.ListFromJson(result.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    this.projectList = null;
                    ThrowOnApiFailure(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.projectList = null;
                throw;
            }

            return this.projectList;
        }

        public List<XpDevRepositoryResponse> ListRepos(bool forceListUpdate = true)
        {
            // Check for cached list and make sure a forced list update was not requested
            if (this.repoList != null && !forceListUpdate)
            {
                return this.repoList;
            }

            try
            {
                var url = "repositories";
                var result = this.client.GetAsync(url).Result;

                if (result.IsSuccessStatusCode)
                {
                    this.repoList = XpDevRepositoryResponse.ListFromJson(result.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    this.repoList = null;
                    ThrowOnApiFailure(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.repoList = null;
            }

            return this.repoList;
        }

        #endregion

        #region Non-Public Methods

        private static void ThrowOnApiFailure(HttpResponseMessage result)
        {
            // Throw Exception
            var msg = string.Empty;
            XpDevErrorMessageResponse errorResponse = null;
            try
            {
                errorResponse = XpDevErrorMessageResponse.FromJson(result.Content.ReadAsStringAsync().Result);
            }
            catch
            {
                // Ignore
            }

            if (errorResponse != null)
            {
                msg = errorResponse.ErrorMessage;
            }

            switch (result.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException(string.Format(Resource.ApiAuthenticationFailed, "ApiKey") + $" ({msg})");
                case HttpStatusCode.Forbidden:
                    throw new SecurityException(Resource.ApiMissingPermissions + $" ({msg})");
                case HttpStatusCode.NotFound:
                    throw new InvalidOperationException(string.Format(Resource.ApiInvalidUsername, "ApiKey") + $" ({msg})");
                default:
                    throw new Exception($"{msg} {result.ReasonPhrase} ({(int)result.StatusCode} - {result.StatusCode.ToString()})");
            }
        }

        #endregion
    }
}