using System.Net.Http;
using IHttpHandler = BGLGroup_MS.Models.Interfaces.IHttpHandler;

namespace BGLGroup_MS.Models
{
	public class HttpClientHandler : IHttpHandler
	{
		private HttpClient _client = new HttpClient();

		public HttpResponseMessage Get(string url)
		{
			var header = "BGL-TEST-APP";

			_client.DefaultRequestHeaders.Add("User-Agent", header);

			return _client.GetAsync(url).Result;
		}

		private void GetAsync(string url)
		{
		}
	}
}