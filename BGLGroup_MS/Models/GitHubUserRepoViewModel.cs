using System.ComponentModel.DataAnnotations;

namespace BGLGroup_MS.Models
{
	public class GitHubUserRepoViewModel
	{
		[Display(Name = "Repository Name")]
		public string name { get; set; }

		[Display(Name = "Repository URL")]
		public string repoUrl { get; set; }

		[Display(Name = "Stargazer Count")]
		public int starGazerCount { get; set; }
	}
}