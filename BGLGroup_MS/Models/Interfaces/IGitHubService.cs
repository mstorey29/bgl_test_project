namespace BGLGroup_MS.Models.Interfaces
{
	public interface IGitHubService
	{
		GitHubUserViewModel GetUserAndRepos(string userName);
	}
}