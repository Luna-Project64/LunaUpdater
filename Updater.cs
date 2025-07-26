using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HttpClientProgress;
using Octokit;

namespace LunaUpdater
{
    public class Updater
    {
        readonly string repositoryOwner = "Luna-Project64";
        readonly string repositoryName = "Luna-Project64";
        readonly GitHubClient github_ = new GitHubClient(new ProductHeaderValue("LunaUpdater"));
        
        public async Task<Release> HasNewVersion(string currentVersionString)
        {
            var releases = await github_.Repository.Release.GetAll(repositoryOwner, repositoryName, new ApiOptions() { PageSize = 5, PageCount = 1 });
            var latestRelease = releases.FirstOrDefault();
            if (latestRelease == null)
            {
                return null;
            }

            if (currentVersionString == null)
            {
                return latestRelease;
            }

            var latestReleaseVersion = new Version(latestRelease.TagName.TrimStart('v'));
            var currentVersion = new Version(currentVersionString.TrimStart('v'));

            bool hasNewVersion = latestReleaseVersion > currentVersion;
            if (!hasNewVersion)
            {
                return null;
            }
            else
            {
                return latestRelease;
            }
        }

        public async Task<string> DownloadLatestRelease(Release release, EventHandler<float> onProgress)
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
                httpClient.Timeout = TimeSpan.FromMinutes(30);

                var progress = new Progress<float>();
                progress.ProgressChanged += onProgress;

                var tempFile = Path.GetTempFileName();
                using (var file = new FileStream(tempFile, System.IO.FileMode.Create, FileAccess.Write, FileShare.None))
                    await httpClient.DownloadDataAsync(url, file, progress);

                return tempFile;
            }
        }
    }
}
