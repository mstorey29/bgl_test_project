using BGLGroup_MS.Controllers;
using BGLGroup_MS.Models;
using BGLGroup_MS.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace BGLGroup_MS.Tests.Controllers
{
	[TestClass]
	public class HomeControllerTest
	{
		private Mock<IHttpHandler> httpHandler;

		[TestInitialize]
		public void TestSetup()
		{
			httpHandler = new Mock<IHttpHandler>();
		}

		[TestMethod]
		public void Index_Test()
		{
			//Arrange
			var controller = new HomeController(httpHandler.Object);

			//Act
			var resultView = controller.Index() as ViewResult;

			//Assert
			Assert.IsNotNull(resultView);
		}

		[TestMethod]
		public void SubmitFormValidUser_Test()
		{
			//Arrange
			var dummyUsername = "DummyUser";
			var dummyUserContent = new StringContent(string.Format("{{\"name\": \"{0}\"}}", dummyUsername));
			var dummyRepoContent = new StringContent("[{\"name\":\"DummyName\", \"repoUrl\" : \"http://test.com\"}]");

			httpHandler.Setup(h => h.Get(It.Is<String>(s => !s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = dummyUserContent });
			httpHandler.Setup(h => h.Get(It.Is<String>(s => s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = dummyRepoContent });

			var controller = new HomeController(httpHandler.Object);

			var submitModel = new GitHubUserInputModel { UserName = dummyUsername };

			//Act
			var resultView = controller.SubmitForm(submitModel) as PartialViewResult;

			var model = (GitHubUserViewModel)resultView.ViewData.Model;

			//Assert
			Assert.AreEqual(dummyUsername, model.name);
		}

		[TestMethod]
		public void SubmitFormInvalidUser_Test()
		{
			//Arrange
			var dummyUsername = "DummyUser";

			httpHandler.Setup(h => h.Get(It.Is<String>(s => !s.Contains("repo")))).Returns(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

			var controller = new HomeController(httpHandler.Object);

			var submitModel = new GitHubUserInputModel { UserName = dummyUsername };

			//Act
			var resultView = controller.SubmitForm(submitModel) as PartialViewResult;

			//Assert
			Assert.IsNotNull(resultView.ViewBag.ErrorMessage);
		}
	}
}