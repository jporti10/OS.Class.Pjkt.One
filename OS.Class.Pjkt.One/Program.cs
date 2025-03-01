using System;
using System.Threading;
using System.IO.Pipes;
using System.IO;

namespace OS.Class.Pjkt.One
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Multi-Threading Demo ===");
            RunMultiThreadingDemo();

            Console.WriteLine("\n=== IPC Demo ===");
            RunIPCDemo();
        }

        // Runs all phases of the multi-threading demo (Project A).
        static void RunMultiThreadingDemo()
        {
            // Phase 1: Basic Thread Operations (creating threads without synchronization)
            Console.WriteLine("\nPhase 1: Basic Thread Operations (no resource protection)");
            BasicThreadOperations();

            // Phase 2: Resource Protection (using locks to synchronize access)
            Console.WriteLine("\nPhase 2: Resource Protection using lock");
            ThreadSafeOperations();

            // Phase 3: Deadlock Creation (simulate a deadlock scenario with unsorted locks)
            Console.WriteLine("\nPhase 3: Deadlock Creation demonstration");
            DeadlockCreation();

            // Phase 4: Deadlock Resolution (resolve deadlock by enforcing a lock ordering)
            Console.WriteLine("\nPhase 4: Deadlock Resolution demonstration");
            DeadlockResolution();
        }

        // Phase 1: Demonstrates basic thread creation and concurrent operations
        // This method uses a BankAccountUnsafe instance that does not protect its balance.
        static void BasicThreadOperations()
        {
            BankAccountUnsafe account = new BankAccountUnsafe(1000);
            Thread[] threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    // Each thread simulates a series of transactions:
                    // withdrawing 100 and then depositing 50 repeatedly.
                    for (int j = 0; j < 5; j++)
                    {
                        account.Withdraw(100);
                        account.Deposit(50);
                        Thread.Sleep(10); // Simulate processing delay
                    }
                });
                threads[i].Start();
            }
            foreach (var t in threads)
                t.Join();
            Console.WriteLine("Final unsafe balance: " + account.Balance);
        }

        // Phase 2: Demonstrates synchronized thread operations using lock for resource protection.
        // It uses BankAccountSafe that wraps balance updates in lock statements.
        static void ThreadSafeOperations()
        {
            BankAccountSafe account = new BankAccountSafe(1000);
            Thread[] threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < 5; j++)
                    {
                        account.Withdraw(100);
                        account.Deposit(50);
                        Thread.Sleep(10);
                    }
                });
                threads[i].Start();
            }
            foreach (var t in threads)
                t.Join();
            Console.WriteLine("Final safe balance: " + account.Balance);
        }

        // Phase 3: Demonstrates a deadlock scenario.
        // Two accounts are used; threads transfer funds in opposite directions without consistent lock ordering.
        static void DeadlockCreation()
        {
            BankAccountSafe accountA = new BankAccountSafe(1000, "AccountA");
            BankAccountSafe accountB = new BankAccountSafe(1000, "AccountB");

            // Two threads try to transfer funds concurrently in opposite directions.
            Thread t1 = new Thread(() => TransferFunds(accountA, accountB, 100, ordered: false));
            Thread t2 = new Thread(() => TransferFunds(accountB, accountA, 200, ordered: false));
            t1.Start();
            t2.Start();

            // Wait briefly; if threads do not finish, assume deadlock.
            bool t1Finished = t1.Join(1000);
            bool t2Finished = t2.Join(1000);
            if (!t1Finished || !t2Finished)
            {
                Console.WriteLine("Deadlock detected: Transfers did not complete in time.");
            }
            else
            {
                Console.WriteLine("Transfers completed (unexpected, as deadlock was expected).");
            }
        }

        // Phase 4: Resolves the deadlock by enforcing a consistent lock order.
        static void DeadlockResolution()
        {
            BankAccountSafe accountA = new BankAccountSafe(1000, "AccountA");
            BankAccountSafe accountB = new BankAccountSafe(1000, "AccountB");

            Thread t1 = new Thread(() => TransferFunds(accountA, accountB, 100, ordered: true));
            Thread t2 = new Thread(() => TransferFunds(accountB, accountA, 200, ordered: true));
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            Console.WriteLine("Transfers completed successfully with deadlock resolution.");
            Console.WriteLine($"{accountA.Name} balance: {accountA.Balance}");
            Console.WriteLine($"{accountB.Name} balance: {accountB.Balance}");
        }

        // Helper method to transfer funds between accounts.
        // The 'ordered' parameter determines whether locks are acquired in a consistent order.
        static void TransferFunds(BankAccountSafe from, BankAccountSafe to, int amount, bool ordered)
        {
            if (ordered)
            {
                // Enforce a lock order based on account names to prevent deadlock.
                BankAccountSafe first = (string.Compare(from.Name, to.Name) < 0) ? from : to;
                BankAccountSafe second = (first == from) ? to : from;
                lock (first.LockObj)
                {
                    Thread.Sleep(50); // Simulate processing delay
                    lock (second.LockObj)
                    {
                        from.Withdraw(amount);
                        to.Deposit(amount);
                        Console.WriteLine($"Transferred {amount} from {from.Name} to {to.Name}.");
                    }
                }
            }
            else
            {
                // Without enforcing order, potential deadlock may occur.
                lock (from.LockObj)
                {
                    Thread.Sleep(50);
                    lock (to.LockObj)
                    {
                        from.Withdraw(amount);
                        to.Deposit(amount);
                        Console.WriteLine($"Transferred {amount} from {from.Name} to {to.Name}.");
                    }
                }
            }
        }

        // Runs the IPC demo (Project B) using named pipes.
        // This simulates interprocess communication by having a server and a client running in separate threads.
        static void RunIPCDemo()
        {
            Thread serverThread = new Thread(NamedPipeServer);
            Thread clientThread = new Thread(NamedPipeClient);
            serverThread.Start();

            // Ensure the server is ready before starting the client.
            Thread.Sleep(500);
            clientThread.Start();

            serverThread.Join();
            clientThread.Join();
        }

        // Named pipe server method.
        // Waits for a client connection and reads a message from the pipe.
        static void NamedPipeServer()
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("TestPipe", PipeDirection.In))
            {
                Console.WriteLine("Named Pipe Server: Waiting for connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("Named Pipe Server: Client connected.");
                using (StreamReader reader = new StreamReader(pipeServer))
                {
                    string message = reader.ReadLine();
                    Console.WriteLine("Named Pipe Server received: " + message);
                }
            }
        }

        // Named pipe client method.
        // Connects to the server and sends a message through the pipe.
        static void NamedPipeClient()
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "TestPipe", PipeDirection.Out))
            {
                Console.WriteLine("Named Pipe Client: Connecting to server...");
                pipeClient.Connect();
                using (StreamWriter writer = new StreamWriter(pipeClient))
                {
                    writer.AutoFlush = true;
                    string message = "Hello from Named Pipe Client!";
                    writer.WriteLine(message);
                    Console.WriteLine("Named Pipe Client sent: " + message);
                }
            }
        }
    }

    // -------------------------------
    // Bank Account Classes
    // -------------------------------

    // BankAccountUnsafe is used in Phase 1 to show basic thread operations without synchronization.
    class BankAccountUnsafe
    {
        public int Balance { get; private set; }
        public BankAccountUnsafe(int initialBalance)
        {
            Balance = initialBalance;
        }
        public void Deposit(int amount)
        {
            Balance += amount;
        }
        public void Withdraw(int amount)
        {
            Balance -= amount;
        }
    }

    // BankAccountSafe uses lock to synchronize access to the account balance.
    // It is used in Phases 2, 3, and 4.
    class BankAccountSafe
    {
        public int Balance { get; private set; }
        public string Name { get; private set; }
        public object LockObj { get; } = new object();

        public BankAccountSafe(int initialBalance, string name = "Account")
        {
            Balance = initialBalance;
            Name = name;
        }

        public void Deposit(int amount)
        {
            lock (LockObj)
            {
                Balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            lock (LockObj)
            {
                Balance -= amount;
            }
        }
    }
}
