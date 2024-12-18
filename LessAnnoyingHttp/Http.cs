using System.Text;

namespace LessAnnoyingHttp;

public class Http {
	public static Response Get(string endpoint, Header[]? headers = null) {
		using HttpClient client = new ();
		HttpRequestMessage request = new () {
			RequestUri = new Uri(endpoint),
			Method = HttpMethod.Get
		};
		
		if (headers != null)
			foreach (Header header in headers)
				request.Headers.Add(header.Name, header.Value);
		
		HttpResponseMessage response;
		try {
			response = client.SendAsync(request).Result;
		} catch (Exception) {
			return new Response { IsSuccessful = false, Body = "" };
		} 

		return new Response { IsSuccessful = response.IsSuccessStatusCode, Body = response.Content.ReadAsStringAsync().Result, StatusCode = response.StatusCode };
	}

	private static Response BodyRequest(string endpoint, HttpMethod method, string body, Header[]? headers, string contentType) {
		using HttpClient client = new ();
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
		} catch (Exception e) {
			return new Response { IsSuccessful = false, Body = "", Exception = e };
		} 

		return new Response { IsSuccessful = response.IsSuccessStatusCode, Body = response.Content.ReadAsStringAsync().Result, StatusCode = response.StatusCode };
	}

	public static Response Patch(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => BodyRequest(endpoint, HttpMethod.Patch, body, headers, contentType);
	
	public static Response Post(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => BodyRequest(endpoint, HttpMethod.Post, body, headers, contentType);
	
	public static Response Put(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => BodyRequest(endpoint, HttpMethod.Put, body, headers, contentType);

	public static Response Delete(string endpoint, string body, Header[]? headers = null, string contentType = "application/json") => BodyRequest(endpoint, HttpMethod.Delete, body, headers, contentType);
}