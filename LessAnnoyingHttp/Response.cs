using System.Net;

namespace LessAnnoyingHttp;

/// <summary>
/// The object returned by an HTTP request
/// <remarks>Has the following properties:
/// <list type="bullet">
/// <item><see cref="Body"/></item>
/// <item><see cref="IsSuccessful"/></item>
/// <item><see cref="StatusCode"/></item>
/// <item><see cref="Exception"/></item>
/// </list>
/// </remarks>
/// </summary>
public class Response {
	/// <summary>
	/// The response of the HTTP request (is empty when the request failed)
	/// </summary>
	public string Body { get; internal init; } = "";

	/// <summary>
	/// The headers sent back by the server
	/// </summary>
	public Dictionary<string, IEnumerable<string>> Headers { get; internal init; } = [];
	
	/// <summary>
	/// Specifies whether the request was successful
	/// </summary>
	public bool IsSuccessful { get; internal init; }
	
	/// <summary>
	/// The HTTP status code returned by the server
	/// </summary>
	public HttpStatusCode StatusCode { get; internal init; }
	
	/// <summary>
	/// Any exception that was thrown during the sending of the request (is null when no exception was thrown)
	/// </summary>
	public Exception? Exception { get; internal init; }

	internal Response() { }
}