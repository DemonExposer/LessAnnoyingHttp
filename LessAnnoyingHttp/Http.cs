using System.Text;

namespace LessAnnoyingHttp;

public static class Http {
	/// <summary>
	/// The timeout in seconds which requests should wait for before failing (default: 10)<br/>
	/// Changing this value changes the timeouts globally
	/// </summary>
	public static int Timeout { get; set; } = 10;
	
	/// <summary>
	/// Sends a GET request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="headers">Optional headers</param>
	/// <returns><see cref="Response"/></returns>
	public static Response Get(string endpoint, Header[]? headers = null) {
		using HttpClient client = new ();
		client.Timeout = TimeSpan.FromSeconds(Timeout);
		HttpRequestMessage request = new () {
			RequestUri = new Uri(endpoint),
			Method = HttpMethod.Get
		};
		
		if (headers != null)
			foreach (Header header in headers)
				request.Headers.Add(header.Name, header.Value);
		
		HttpResponseMessage response;
		try {
			response = client.Send(request);
		} catch (TaskCanceledException) {
			throw new TimeoutException($"Timeout waiting for response for request to {endpoint}");
		} catch (Exception e) {
			return new Response { IsSuccessful = false, Body = "", Exception = e };
		} 

		return new Response { IsSuccessful = response.IsSuccessStatusCode, Body = response.Content.ReadAsStringAsync().Result, StatusCode = response.StatusCode };
	}

	private static Response BodyRequest(string endpoint, HttpMethod method, string body, Header[]? headers, string contentType) {
		using HttpClient client = new ();
		client.Timeout = TimeSpan.FromSeconds(Timeout);
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
			response = client.Send(request);
		} catch (TaskCanceledException) {
			throw new TimeoutException($"Timeout waiting for response for request to {endpoint}");
		} catch (Exception e) {
			return new Response { IsSuccessful = false, Body = "", Exception = e };
		} 

		return new Response { IsSuccessful = response.IsSuccessStatusCode, Body = response.Content.ReadAsStringAsync().Result, StatusCode = response.StatusCode };
	}
	
	/// <summary>
	/// Sends a DELETE request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="body">The request body</param>
	/// <param name="headers">Optional headers</param>
	/// <param name="contentType">The content type of the body - default = application/json</param>
	/// <returns><see cref="Response"/></returns>
	public static Response Delete(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => BodyRequest(endpoint, HttpMethod.Delete, body, headers, contentType);

	/// <summary>
	/// Sends a PATCH request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="body">The request body</param>
	/// <param name="headers">Optional headers</param>
	/// <param name="contentType">The content type of the body - default = application/json</param>
	/// <returns><see cref="Response"/></returns>
	public static Response Patch(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => BodyRequest(endpoint, HttpMethod.Patch, body, headers, contentType);
	
	/// <summary>
	/// Sends a POST request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="body">The request body</param>
	/// <param name="headers">Optional headers</param>
	/// <param name="contentType">The content type of the body - default = application/json</param>
	/// <returns><see cref="Response"/></returns>
	public static Response Post(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => BodyRequest(endpoint, HttpMethod.Post, body, headers, contentType);
	
	/// <summary>
	/// Sends a PUT request
	/// </summary>
	/// <param name="endpoint">The URL to send the request to</param>
	/// <param name="body">The request body</param>
	/// <param name="headers">Optional headers</param>
	/// <param name="contentType">The content type of the body - default = application/json</param>
	/// <returns><see cref="Response"/></returns>
	public static Response Put(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => BodyRequest(endpoint, HttpMethod.Put, body, headers, contentType);
}