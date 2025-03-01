using System.IO.Pipes;

namespace OSProject1B
{
    internal abstract class Program
    {
        static void Main()
        {
            Console.WriteLine("OS Class Project 1 Part B: [IPC - Server side]");
            
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("ProjectPipe", PipeDirection.In))
            {
                Console.WriteLine("Server: Waiting for client connection...\n");
                
                pipeServer.WaitForConnection(); //** Waiting to receive connection from Client to obtain messages **//
                Console.WriteLine("Server: Incoming client connection...\nServer: Connected!\n");
                
                using (StreamReader reader = new StreamReader(pipeServer))
                {
                    //** Keep read incoming messages until client disconnects **//
                    while (!reader.EndOfStream)
                    {
                        string? message = reader.ReadLine();
                        
                        if (message == null)
                            break;
                        
                        Thread.Sleep(3000);
                        Console.WriteLine("Server received: " + message + $" [at {DateTime.Now:T}]");
                        //** Simulate processing and input delay for each message **//
                    }
                }
            }
            Console.WriteLine("\nServer: Connection closed. Press any key to exit.");
            Console.ReadKey();
        }
    }
}