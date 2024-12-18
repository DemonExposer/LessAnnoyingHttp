using System.Net;

namespace LessAnnoyingHttp;

public class Response {
	public string Body { get; internal init; } = "";
	public bool IsSuccessful { get; internal init; }
	public HttpStatusCode StatusCode { get; internal init; }
	public Exception? Exception { get; internal init; }

	internal Response() { }
}