using System.Net;
using System.Net.Sockets;
using System.Text;


// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
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
string userAgent = allTokens[3][1];
var uri = allTokens[0][1];

//create the messages and check the uri
//var message = string.Empty;
int lenght = userAgent.Length; 

var respons200 = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {lenght}\r\n\r\n{userAgent}";
var send200 = Encoding.UTF8.GetBytes(respons200);
var send404 = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");


if(uri.Contains("/user-agent") || uri.Length == 1)
{
    socket.Send(send200);
}else
{
    socket.Send(send404);
}