using BGLGroup_MS.Models;
using BGLGroup_MS.Models.Interfaces;
using System;
using System.Web.Mvc;

namespace BGLGroup_MS.Controllers
{
	public class HomeController : Controller
	{
		private readonly IHttpHandler _httpHandler;

		public ActionResult Index()
		{
			return View();
		}

		public HomeController()
		{
			_httpHandler = new HttpClientHandler();
		}

		public HomeController(IHttpHandler httpHandler)
		{
			_httpHandler = httpHandler;
		}

		[HttpPost]
		public ActionResult SubmitForm(GitHubUserInputModel userInputModel)
		{
			if (ModelState.IsValid)
			{
				ViewBag.ErrorMessage = "";
				var gitHubService = new GitHubService(_httpHandler);

				try
				{

					var gitUserModel = gitHubService.GetUserAndRepos(userInputModel.UserName);

					if (string.IsNullOrEmpty(gitUserModel.name))
					{
						ViewBag.ErrorMessage = "Could not find user";

					}

					return PartialView("_GitHubRepoResults", gitUserModel);
				}
				catch(Exception e)
				{
					ViewBag.ErrorMessage = "There was an error retrieving the data";

					return PartialView("_GitHubRepoResults", new GitHubUserViewModel());
				}

			}

			return View();
		}
	}
}