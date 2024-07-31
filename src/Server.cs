using System.Net;
using System.Net.Sockets;
using System.Text;


// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

while(true){
var socket = server.AcceptSocket();

//Reader
byte[] data = new byte[1024];
int receivedData = socket.Receive(data);
string stringData = Encoding.UTF8.GetString(data, 0, receivedData);
Console.WriteLine("Received data:\n" + stringData);

string[] requestLines = stringData.Split(new[] { "\r\n" }, StringSplitOptions.None);
string [][] allTokens = new string [requestLines.Length][];

for(int i = 0; i < requestLines.Length; i++)
{
    allTokens[i] = requestLines[i].Split(new[]{ ' ' },StringSplitOptions.RemoveEmptyEntries);
}

//select the data
var uri = allTokens[0][1];

var send404 = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");

if(uri == "/")
{
    var send200 = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
    socket.Send(send200);
}
else if(uri.Contains("/echo/"))
{
    var message = uri.Replace("/echo/", string.Empty);
    var messageLenght = message.Length;
    var respons200 = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {messageLenght}\r\n\r\n{message}";
    var send200UserAgent = Encoding.UTF8.GetBytes(respons200);
    socket.Send(send200UserAgent);

}else if(uri.Contains("/user-agent"))
{
    string userAgent = allTokens[2][1];
    int lenght = allTokens[2][1].Length;
    var respons = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {lenght}\r\n\r\n{userAgent}";
    var send200UserAgent = Encoding.UTF8.GetBytes(respons);
    socket.Send(send200UserAgent);
}else
{
    socket.Send(send404);
}

}