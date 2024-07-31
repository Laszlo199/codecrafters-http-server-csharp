using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
string directory = args.Length > 1 && args[0] == "--directory" ? args[1] : ".";

TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
Console.WriteLine("Server started, waiting for connections...");

while (true)
{
    TcpClient client = await server.AcceptTcpClientAsync();
    _ = Task.Run(() => HandleRequestAsync(client, directory));
}

async Task HandleRequestAsync(TcpClient client, string directory)
{
    try
    {
        // Reader
        NetworkStream stream = client.GetStream();
        byte[] data = new byte[1024];
        int receivedData = await stream.ReadAsync(data, 0, data.Length);
        string stringData = Encoding.UTF8.GetString(data, 0, receivedData);
        Console.WriteLine("Received data:\n" + stringData);

        string[] requestLines = stringData.Split(new[] { "\r\n" }, StringSplitOptions.None);
        string[][] allTokens = new string[requestLines.Length][];

        for (int i = 0; i < requestLines.Length; i++)
        {
            allTokens[i] = requestLines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        // Select the data
        var uri = allTokens[0][1];

        var send404 = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");

        if (uri == "/")
        {
            var send200 = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
            await stream.WriteAsync(send200, 0, send200.Length);
        }
        else if (uri.Contains("/echo/"))
        {
            var message = uri.Replace("/echo/", string.Empty);
            var messageLength = message.Length;
            var response200 = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {messageLength}\r\n\r\n{message}";
            var send200UserAgent = Encoding.UTF8.GetBytes(response200);
            await stream.WriteAsync(send200UserAgent, 0, send200UserAgent.Length);
        }
        else if (uri.Contains("/user-agent"))
        {
            string userAgent = allTokens[2][1];
            int length = allTokens[2][1].Length;
            var response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {length}\r\n\r\n{userAgent}";
            var send200UserAgent = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(send200UserAgent, 0, send200UserAgent.Length);
        }
        else
        {
            await stream.WriteAsync(send404, 0, send404.Length);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
    finally
    {
        client.Close();
    }
}