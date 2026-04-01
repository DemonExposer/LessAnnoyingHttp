using System.Text;

namespace LessAnnoyingHttp;

public static class Http {
	private static readonly HttpClient Client = new (new SocketsHttpHandler {
		PooledConnectionLifetime = TimeSpan.FromMinutes(2)
	});

	/// <summary>
	/// The timeout in seconds which requests should wait for before failing (default: 10)<br/>
	/// Changing this value changes the timeouts globally
	/// </summary>
	public static int Timeout {
		get => (int) Client.Timeout.TotalSeconds;
		set => Client.Timeout = TimeSpan.FromSeconds(value);
	}

	static Http() {
		Timeout = 10;
		AppDomain.CurrentDomain.ProcessExit += (_, _) => Client.Dispose();
	}
	
	/// <summary>
	/// Sends a GET request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="headers">Optional headers</param>
	/// <returns><see cref="Response"/></returns>
	public static async Task<Response> GetAsync(string endpoint, Header[]? headers = null) {
		HttpRequestMessage request = new () {
			RequestUri = new Uri(endpoint),
			Method = HttpMethod.Get
		};
		
		if (headers != null)
			foreach (Header header in headers)
				request.Headers.Add(header.Name, header.Value);
		
		HttpResponseMessage response;
		try {
			response = await Client.SendAsync(request);
		} catch (TaskCanceledException) {
			throw new TimeoutException($"Timeout waiting for response for request to {endpoint}");
		} catch (Exception e) {
			return new Response { IsSuccessful = false, Body = "", Exception = e };
		}
		
		return new Response { IsSuccessful = response.IsSuccessStatusCode, Body = response.Content.ReadAsStringAsync().Result, Headers = new Dictionary<string, IEnumerable<string>>(response.Headers.ToDictionary(), StringComparer.OrdinalIgnoreCase), StatusCode = response.StatusCode };
	}
	
	/// <summary>
	/// Synchronous version of <see cref="GetAsync"/>
	/// </summary>
	public static Response Get(string endpoint, Header[]? headers = null) => GetAsync(endpoint, headers).GetAwaiter().GetResult();

	private static async Task<Response> BodyRequest(string endpoint, HttpMethod method, string body, Header[]? headers, string contentType) {
		HttpRequestMessage request = new () {
			RequestUri = new Uri(endpoint),
			Method = method,
			Content = new StringContent(body, Encoding.UTF8, contentType)
		};
		
		if (headers != null)
			foreach (Header header in headers)
				request.Headers.Add(header.Name, header.Value);
		
		HttpResponseMessage response;
		try {
			response = await Client.SendAsync(request);
		} catch (TaskCanceledException) {
			throw new TimeoutException($"Timeout waiting for response for request to {endpoint}");
		} catch (Exception e) {
			return new Response { IsSuccessful = false, Body = "", Exception = e };
		} 

		return new Response { IsSuccessful = response.IsSuccessStatusCode, Body = response.Content.ReadAsStringAsync().Result, Headers = new Dictionary<string, IEnumerable<string>>(response.Headers.ToDictionary(), StringComparer.OrdinalIgnoreCase), StatusCode = response.StatusCode };
	}
	
	/// <summary>
	/// Sends a DELETE request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="body">The request body</param>
	/// <param name="headers">Optional headers</param>
	/// <param name="contentType">The content type of the body - default = application/json</param>
	/// <returns><see cref="Response"/></returns>
	public static async Task<Response> DeleteAsync(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => await BodyRequest(endpoint, HttpMethod.Delete, body, headers, contentType);
	
	/// <summary>
	/// Synchronous version of <see cref="DeleteAsync"/>
	/// </summary>
	public static Response Delete(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => DeleteAsync(endpoint, body, headers, contentType).GetAwaiter().GetResult();

	/// <summary>
	/// Sends a PATCH request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="body">The request body</param>
	/// <param name="headers">Optional headers</param>
	/// <param name="contentType">The content type of the body - default = application/json</param>
	/// <returns><see cref="Response"/></returns>
	public static async Task<Response> PatchAsync(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => await BodyRequest(endpoint, HttpMethod.Patch, body, headers, contentType);
	
	/// <summary>
	/// Synchronous version of <see cref="PatchAsync"/>
	/// </summary>
	public static Response Patch(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => PatchAsync(endpoint, body, headers, contentType).GetAwaiter().GetResult();
	
	/// <summary>
	/// Sends a POST request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="body">The request body</param>
	/// <param name="headers">Optional headers</param>
	/// <param name="contentType">The content type of the body - default = application/json</param>
	/// <returns><see cref="Response"/></returns>
	public static async Task<Response> PostAsync(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => await BodyRequest(endpoint, HttpMethod.Post, body, headers, contentType);
	
	/// <summary>
	/// Synchronous version of <see cref="PostAsync"/>
	/// </summary>
	public static Response Post(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => PostAsync(endpoint, body, headers, contentType).GetAwaiter().GetResult();
	
	/// <summary>
	/// Sends a PUT request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="body">The request body</param>
	/// <param name="headers">Optional headers</param>
	/// <param name="contentType">The content type of the body - default = application/json</param>
	/// <returns><see cref="Response"/></returns>
	public static async Task<Response> PutAsync(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => await BodyRequest(endpoint, HttpMethod.Put, body, headers, contentType);
	
	/// <summary>
	/// Synchronous version of <see cref="PutAsync"/>
	/// </summary>
	public static Response Put(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => PutAsync(endpoint, body, headers, contentType).GetAwaiter().GetResult();
}