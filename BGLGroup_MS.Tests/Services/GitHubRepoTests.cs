using BGLGroup_MS.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace BGLGroup_MS.Tests
{
	[TestClass()]
	public class GitHubRepoTests
	{
		private Mock<IHttpHandler> httpHandler;
		private GitHubService gitHubService;

		[TestInitialize]
		public void TestSetup()
		{
			httpHandler = new Mock<IHttpHandler>();
			gitHubService = new GitHubService(httpHandler.Object);
		}

		[TestMethod()]
		public void GetUserWithReposValidUser_Test()
		{
			//Arrange
			var dummyUsername = "DummyUser";
			var dummyUserContent = new StringContent(string.Format("{{\"name\": \"{0}\"}}", dummyUsername));
			var dummyRepoContent = new StringContent("[{\"name\":\"DummyName\", \"repoUrl\" : \"http://test.com\"}]");

			httpHandler.Setup(h => h.Get(It.Is<String>(s => !s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = dummyUserContent });
			httpHandler.Setup(h => h.Get(It.Is<String>(s => s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = dummyRepoContent });

			//Act
			var returnedModel = gitHubService.GetUserAndRepos(dummyUsername);

			//Assert
			Assert.AreEqual(dummyUsername, returnedModel.name);
			Assert.AreEqual(1, returnedModel.userRepos.Count());
		}

		[TestMethod()]
		public void GetUserWithReposInvalidUser_Test()
		{
			//Arrange
			var dummyUsername = "DummyUserDoesNotExist";
			httpHandler.Setup(h => h.Get(It.Is<String>(s => !s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

			//Act
			var returnedModel = gitHubService.GetUserAndRepos(dummyUsername);

			//Assert
			Assert.IsNull(returnedModel.name);
			Assert.AreEqual(0, returnedModel.userRepos.Count());
		}

		[TestMethod()]
		public void GetUserWithReposMaxFiveResults_Test()
		{
			//Arrange
			var dummyUsername = "DummyUser";
			var dummyUserContent = new StringContent(string.Format("{{\"name\": \"{0}\"}}", dummyUsername));
			var dummyRepoContent = new StringContent("[{\"name\":\"DummyName1\", \"repoUrl\" : \"http://test.com\"}," +
				"{\"name\":\"DummyName2\", \"repoUrl\" : \"http://test.com\"},{\"name\":\"DummyName3\", \"repoUrl\" : \"http://test.com\"}" +
				",{\"name\":\"DummyName4\", \"repoUrl\" : \"http://test.com\"},{\"name\":\"DummyName5\", \"repoUrl\" : \"http://test.com\"}," +
				"{\"name\":\"DummyName6\", \"repoUrl\" : \"http://test.com\"}]");

			httpHandler.Setup(h => h.Get(It.Is<String>(s => !s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = dummyUserContent });
			httpHandler.Setup(h => h.Get(It.Is<String>(s => s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = dummyRepoContent });

			//Act
			var returnedModel = gitHubService.GetUserAndRepos(dummyUsername);

			//Assert
			Assert.IsTrue(returnedModel.userRepos.Count() <= 5);
		}

		[TestMethod()]
		public void GetUserWithReposVerifyOrderBy_Test()
		{
			//Arrange
			var dummyUsername = "DummyUser";
			var dummyUserContent = new StringContent(string.Format("{{\"name\": \"{0}\"}}", dummyUsername));
			var dummyRepoContent = new StringContent("[{\"name\":\"DummyName1\", \"repoUrl\" : \"http://test.com\", \"stargazers_count\":10}," +
				"{\"name\":\"DummyName2\", \"repoUrl\" : \"http://test.com\", \"stargazers_count\":5},{\"name\":\"DummyName3\", \"repoUrl\" : \"http://test.com\", \"stargazers_count\":0}]");

			httpHandler.Setup(h => h.Get(It.Is<String>(s => !s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = dummyUserContent });
			httpHandler.Setup(h => h.Get(It.Is<String>(s => s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = dummyRepoContent });

			var expectedOrder = new List<int> {10,5,0};

			//Act
			var returnedModel = gitHubService.GetUserAndRepos(dummyUsername);

			//Get stargazer values only
			var starGazerValues = returnedModel.userRepos.Select(r => r.starGazerCount).ToList();

			//Assert
			CollectionAssert.AreEqual(expectedOrder, starGazerValues);
		}
	}
}