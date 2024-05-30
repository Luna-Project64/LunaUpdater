using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Octokit;

namespace LunaUpdater
{
    public class Updater
    {
        readonly string repositoryOwner = "Luna-Project64";
        readonly string repositoryName = "Luna-Project64";
        readonly GitHubClient github_ = new GitHubClient(new ProductHeaderValue("LunaUpdater"));
        
        public async Task<Release> HasNewVersion(string currentVersion)
        {
            var releases = await github_.Repository.Release.GetAll(repositoryOwner, repositoryName, new ApiOptions() { PageSize = 5, PageCount = 1 });
            var latestRelease = releases.FirstOrDefault();
            if (latestRelease == null)
            {
                return null;
            }

            if (currentVersion == null)
            {
                return latestRelease;
            }

            bool hasNewVersion = string.Compare(latestRelease.TagName, currentVersion, StringComparison.OrdinalIgnoreCase) > 0;
            if (!hasNewVersion)
            {
                return null;
            }
            else
            {
                return latestRelease;
            }
        }

        public async Task<string> DownloadLatestRelease(Release release)
        {
            var assets = await github_.Repository.Release.GetAllAssets(repositoryOwner, repositoryName, release.Id);
            var asset = assets.FirstOrDefault();
            if (asset == null)
            {
                return null;
            }

            var url = asset.BrowserDownloadUrl;
            using (HttpClient httpClient = new HttpClient())
            {
                // Download the zip file to a temporary location asynchronously
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();

                    string tempFilePath = Path.GetTempFileName();
                    using (FileStream fileStream = File.OpenWrite(tempFilePath))
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }

                    return tempFilePath;
                }
            }
        }
    }
}
