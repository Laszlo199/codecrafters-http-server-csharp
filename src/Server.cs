using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualBasic;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
var socket = server.AcceptSocket();




byte[] data = new byte[1000];
int receivedData = socket.Receive(data);
string stringData = Encoding.UTF8.GetString(data, 0, receivedData);
string[] requestLines = stringData.Split(new[] { "\r\n" }, StringSplitOptions.None);
string requestLine = requestLines[0];
string[] requestLineTokens = requestLine.Split(' ');
string uri = requestLineTokens[1];

var message = requestLineTokens[1].Replace("/echo/",string.Empty);
int lenght = message.Length;
var respons200 = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {lenght}\r\n\r\n{message}"; 
var send200 = Encoding.UTF8.GetBytes(respons200);
var send404 = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");

if( uri.Contains("/echo/" ) || uri.Length == 1)
{
    socket.Send(send200);
}
else
{
    socket.Send(send404);
}