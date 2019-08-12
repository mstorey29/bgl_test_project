using BGLGroup_MS.Models;
using BGLGroup_MS.Models.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BGLGroup_MS
{
	public class GitHubService : IGitHubService
	{
		private readonly IHttpHandler _httpHandler;

		public GitHubService(IHttpHandler httpHandler)
		{
			_httpHandler = httpHandler;
		}

		/// <summary>
		/// Returns a GitHub user and their top 5
		/// repositories based on stargazer count from
		/// a username
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public GitHubUserViewModel GetUserAndRepos(string userName)
		{
			var gitHubUser = GetUser(userName);

			if (!string.IsNullOrEmpty(gitHubUser.name))
			{
				gitHubUser.userRepos = GetUserReposTopFiveStarGazer(userName);
			}

			return gitHubUser;
		}

		private GitHubUserViewModel GetUser(string userName)
		{
			//Build URL string for API call
			var apiUrl = string.Format("{0}users/{1}", GetGitHubApiURL(), userName);

			var response = _httpHandler.Get(apiUrl);

			if (response.IsSuccessStatusCode)
			{
				var result = response.Content.ReadAsStringAsync().Result;
				return MapGitUserViewModel(JsonConvert.DeserializeObject<GitHubUser>(result));
			}
			else
			{
				return new GitHubUserViewModel();
			}
		}

		private List<GitHubUserRepoViewModel> GetUserReposTopFiveStarGazer(string userName)
		{
			var apiUrl = string.Format("{0}users/{1}/repos", GetGitHubApiURL(), userName);

			var response = _httpHandler.Get(apiUrl);

			if (response.IsSuccessStatusCode)
			{
				var result = response.Content.ReadAsStringAsync().Result;
				var mappedResult = MapGitUserRepoViewModel(JsonConvert.DeserializeObject<List<GitHubRepo>>(result));

				return GetTopFiveReposByStargazerCountDesc(mappedResult);
			}
			else
			{
				return new List<GitHubUserRepoViewModel>();
			}
		}

		private GitHubUserViewModel MapGitUserViewModel(GitHubUser gitHubUser)
		{
			return new GitHubUserViewModel
			{
				name = gitHubUser.name,
				location = gitHubUser.location,
				avatarUrl = gitHubUser.avatar_url,
				userRepos = new List<GitHubUserRepoViewModel>()
			};
		}

		private List<GitHubUserRepoViewModel> MapGitUserRepoViewModel(List<GitHubRepo> gitHubRepos)
		{
			var gitHubReposViewModels = new List<GitHubUserRepoViewModel>();

			foreach (var gitRepo in gitHubRepos)
			{
				gitHubReposViewModels.Add(new GitHubUserRepoViewModel
				{
					name = gitRepo.name,
					repoUrl = gitRepo.url,
					starGazerCount = gitRepo.stargazers_count
				});
			}

			return gitHubReposViewModels;
		}

		private List<GitHubUserRepoViewModel> GetTopFiveReposByStargazerCountDesc(List<GitHubUserRepoViewModel> gitHubUserRepos)
		{
			return gitHubUserRepos.OrderByDescending(g => g.starGazerCount).Take(5).ToList();
		}

		private string GetGitHubApiURL()
		{
			var gitHubApiUrl = ConfigurationManager.AppSettings["GitHubApi"];

			if (string.IsNullOrEmpty(gitHubApiUrl))
			{
				throw new NullReferenceException("GitHubApi has not been configured");
			}

			return gitHubApiUrl;
		}
	}
}