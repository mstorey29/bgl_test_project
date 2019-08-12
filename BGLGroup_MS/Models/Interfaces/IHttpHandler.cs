using System.Net.Http;

namespace BGLGroup_MS.Models.Interfaces
{
	public interface IHttpHandler
	{
		HttpResponseMessage Get(string url);
	}
}