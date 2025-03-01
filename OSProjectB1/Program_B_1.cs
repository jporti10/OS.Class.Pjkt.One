using System.IO.Pipes;

namespace OSProject_B_1
{
    internal abstract class Program
    {
        private static void Main()
        {
            Console.WriteLine("OS Class Project 1 Part B: [IPC - Client side]");
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "ProjectPipe", PipeDirection.Out))
            {
                Console.WriteLine("Client: Connecting to server...\n");
                
                pipeClient.Connect(); //** Connecting to the Server to enable sending messages **//
                Console.WriteLine("Client: Connected!\n");

                using (StreamWriter writer = new StreamWriter(pipeClient))
                {
                    writer.AutoFlush = true;
                    
                    for (int i = 1; i <= 5; i++)
                    {
                        string message = $"Message {i} from IPC Client at {DateTime.Now:T}";
                        writer.WriteLine(message);
                        
                        Console.WriteLine("Client sent: " + message);
                    }
                }
            }
            Console.WriteLine("\nClient: All messages sent, connection closed. Press any key to exit.");
            Console.ReadKey();
        }
    }
}