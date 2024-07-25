using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
var socket = server.AcceptSocket();

var send200 = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
var send404 = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");

byte[] data = new byte[1000];
int receivedData = socket.Receive(data);
string stringData = Encoding.UTF8.GetString(data, 0, receivedData);
string[] requestLines = stringData.Split(new[] { "\r\n" }, StringSplitOptions.None);
string requestLine = requestLines[0];
string[] requestLineTokens = requestLine.Split(' ');
string uri = requestLineTokens[1];

if(uri == "/")
{
    socket.Send(send200);
}
else
{
    socket.Send(send404);
}

