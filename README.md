# Server-Sent Events (SSE) Sample

> This is a very simple project that completely implements Server-Sent Events (SSE)
> using ASP.NET Core 5 WebAPI.

**Server-Sent Events** is a one-way messaging, from server to clients that don't need any new request from
clients to receive updated data from server after initialization. visit [HTML SSE API](https://www.w3schools.com/html/html5_serversentevents.asp)
on _w3schools.com_ for more information.

---

#### How it works?

SSE is an event-based method of receiving data from the server; so, at first you need to define that from which
URL you want to receive data. In a `<script>` tag, add this line of _javascript_ :

```javascript
const eSource = new EventSource("/home/events");
```

Immediately after calling `EventSource` constructor, user agent will start a new request to the server with header
`content-type: text/event-stream`. To receive messages from the server you need to define a function for
**onmessage** event of the `EventSource` object, like this :

```javascript
eSource.onmessage = function (event) {
    console.log(event.data);
}
```

That's all!

Now you need to create an endpoint at _/home/events_ that produces `content-type: text/event-stream` and keeps
the connection opens while the client keeps its connection open!

> If your Action on the server-side completes with a [Successful responses](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status#successful_responses)
> before the client closes the connection, the client call the **onerror** event of the `EventSource` and
> then try to send a new request to the server.

Here is a sample of server-side Action that keeps the connection open until the client closes its own :

```c#
[HttpGet("/home/events")]
public async Task EventsAsync()
{
    string remoteUser = HttpContext.Connection.RemoteIpAddress.MapToIPv4() +
                        ":" +
                        HttpContext.Connection.RemotePort;

    HttpContext.Response.Headers["cache-control"] = "no-cache";
    HttpContext.Response.Headers["content-type"] = "text/event-stream";

    while (!HttpContext.RequestAborted.IsCancellationRequested)
    {
        string data = "data: " + DateTime.Now.ToString("HH:mm:ss.fff");
        await HttpContext.Response.WriteAsync(data, HttpContext.RequestAborted);

        _logger.LogInformation("Wait for 3 seconds and again send data to " + remoteUser);

        System.Threading.Thread.Sleep(3000);
    }

    _logger.LogInformation("Connection to " + remoteUser + " has closed");
}
```
