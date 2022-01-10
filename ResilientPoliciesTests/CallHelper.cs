using System.Net.Http;
using System.Threading.Tasks;
using Users.Api.Infrastructure.Http;

namespace ResilientPoliciesTests
{
    public static class CallHelper
    {
		public static async Task InvokeMultipleHttpRequests(IRestHttpClient resilientClient, int taskCount, HttpMethod method, string uri = "http://localhost")
		{
			var tasks = new Task[taskCount];
			for (var i = 0; i < taskCount; i++)
			{
				var requestMessage = new HttpCustomRequest("resource", method);
				requestMessage.AddHeader("TaskId", i.ToString());
				tasks[i] = resilientClient.ExecuteRequest<object>(requestMessage);
			}
			await Task.WhenAll(tasks);
		}
	}
}
