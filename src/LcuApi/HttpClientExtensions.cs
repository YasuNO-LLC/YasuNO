using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace LcuApi
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(
            this HttpClient client,
            Uri requestUri,
            HttpContent content
        )
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
                          {
                              Content = content
                          };

            var response = new HttpResponseMessage();

            // In case you want to set a timeout
            //CancellationToken cancellationToken = new CancellationTokenSource(60).Token;

            try
            {
                response = await client.SendAsync(request);

                // If you want to use the timeout you set
                //response = await client.SendRequestAsync(request).AsTask(cancellationToken);
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine("ERROR: " + e);
            }

            return response;
        }

        public static Task<HttpResponseMessage> PatchAsync(
            this HttpClient client,
            string requestUri,
            HttpContent content
        )
        {
            var uri = new Uri(client.BaseAddress + requestUri);
            return client.PatchAsync(uri, content);
        }
    }
}
