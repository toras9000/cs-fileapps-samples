#!/usr/bin/env -S dotnet run --file
#:package Kokuban@0.2.0
using System.Net.Sockets;
using System.Text;
using Kokuban;

static class ConsoleUtils
{
    public static void WritePrompt()
    {
        lock (ConsoleSync)
        {
            Console.Write('>');
        }
    }

    public static void WriteAttention(string message)
    {
        lock (ConsoleSync)
        {
            Console.WriteLine(Chalk.Yellow[message]);
        }
    }

    public static void WriteInsertion(string text)
    {
        lock (ConsoleSync)
        {
            var orgCurPos = Console.CursorLeft;
            Console.Write(Environment.NewLine);
            Console.WriteLine(Chalk.Gray[text]);
            Console.Write('>');
            if (2 < orgCurPos)
            {
                Console.Write(new string('.', orgCurPos - 1));
            }
        }
    }

    public static async Task TcpReceiveDumper(TcpClient tcp, CancellationToken cancelToken)
    {
        var buffer = new byte[8192];
        while (true)
        {
            try
            {
                var length = await tcp.Client.ReceiveAsync(buffer, cancelToken);
                if (length == 0) break;

                var text = Encoding.ASCII.GetString(buffer.AsSpan(0, length));
                ConsoleUtils.WriteInsertion(text);
            }
            catch (SocketException)
            {
            }
        }
    }

    private static readonly object ConsoleSync = new();
}
