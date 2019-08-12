using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BGLGroup_MS.Models
{
	public class GitHubUserViewModel
	{
		[Display(Name = "UserName")]
		public string name { get; set; }

		[Display(Name = "Location")]
		public string location { get; set; }

		[Display(Name = "Avatar URL")]
		public string avatarUrl { get; set; }

		[Display(Name = "User Repositories")]
		public List<GitHubUserRepoViewModel> userRepos { get; set; } = new List<GitHubUserRepoViewModel>();
	}
}