namespace OSProject1A
{
    internal static class ProgramA
    {
        private static void Main()
        {
            //** Program begins and goes through the 4 phases as specified **//
            Console.WriteLine("OS Class Project 1 Part A: [Implementation of Multi-Threading]");

            //** Phase 1: Use of standard thread op without sync **//
            Console.WriteLine("\nPhase 1: Basic Thread Operations (Unsafe)");
            RunThreadOp();
            //Thread.Sleep(2000);  //** Wait for 2 seconds before starting the next phase (testing porpoises) **//

            
            //** Phase 2: Utilizing a thread-safe op using locks **//
            Console.WriteLine("\nPhase 2: Resource Protection (Safe)");
            RunSafeThreadOp();
            //Thread.Sleep(2000); //** Sleep for another 2 seconds **//

            
            //** Phase 3: Creating a deadlock encounter by transferring funds without lock ordering **//
            Console.WriteLine("\nPhase 3: Deadlock Creation");
            RunDeadlockCreation();
            //Thread.Sleep(2000); //** Sleep for another 2 seconds **//

            // Phase 4: Solve deadlock by enforcing consistent lock ordering for account fund transfers **//
            Console.WriteLine("\nPhase 4: Deadlock Resolution");
            RunDeadlockResolution();
            //Thread.Sleep(2000); //** Sleep for another 2 seconds **//

            //** Outro portion to exit the program**//
            Console.WriteLine("\nProject 1 Part A is done. Press any key to end the program.");
            Console.ReadKey(); //** Will read the input of the key to end the program **//
        }

        
        ///*** METHODS ***///
        
        //** Phase 1 Method: Create thread operations using 10 threads on an unsafe bank account **//
        static void RunThreadOp()
        {
            BankAccountUnsafe account = new BankAccountUnsafe(1000); //** Initial amount is 1000$ **//
            Thread[] threads = new Thread[10]; //** Making threads **//
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {   //** Lambda expression prof. mentioned during class **//
                    //** Each thread performs 10 iterations of withdrawals and deposits
                    for (int j = 0; j < 10; j++)
                    {
                        account.Withdraw(30);
                        //Console.WriteLine("Take xx$");
                        account.Deposit(35);
                        //Console.WriteLine("Add xx$");
                        //Thread.Sleep(100); //** Use sleep to slow down thread operations and see what it does **//
                        //Console.WriteLine("");
                        //** Runs 100 times: Bal should be 1500 with these withdraws and deposits **//
                        //** Bottom line: having these print statements and sleep commands does affect the amount of times the thread operates as it runs inconsistently for some reason when testing **//
                    }
                });
                
                threads[i].IsBackground = true;  //** Mark as background thread **//
                threads[i].Start(); //** I wasnt able to shut off the program but as marking the thread as a background one it enabled me to close the program without issue **//
            }
            
            foreach (var t in threads)
                t.Join();
            //** Display total Bal **//
            Console.WriteLine($"Final unsafe balance: {account.Balance}");
        }

        //** Phase 2: Thread-safe ops using sync and lock in the bank account **//
        static void RunSafeThreadOp()
        {
            BankAccountSafeMutex account_Temp = new BankAccountSafeMutex(1000);
            Thread[] threads = new Thread[10]; //** Making 10 threads like last time **//
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    //** Each thread should use ops with proper locking (mutex) **//
                    for (int j = 0; j < 10; j++)
                    {
                        account_Temp.Withdraw(45);
                        account_Temp.Deposit(25);
                        //Thread.Sleep(100);
                    }
                });
                
                threads[i].IsBackground = true;  //** Same as last, I do feel like it is a sort of hot glue fix :/ **//
                threads[i].Start();
            }
            
            foreach (var t in threads)
                t.Join();
            //** Display total Bal **//
            Console.WriteLine($"Final safe balance: {account_Temp.Balance}");
            //** We don't really have to worry about negative numbers since after all, a bank will rob you legally in the form of interest **//
        }

        //** Phase 3: Deadlock created by transferring funds between two acc. w/o consistent lock **//
        static void RunDeadlockCreation()
        {
            //** We are going to stress test the deadlock to see if it simply won't go or if doing this increases the unexpected result **//
            int iterations = 10;
            int successCount = 0;
            int deadlockCount = 0;

            Console.WriteLine("Attempting to transfer funds between accounts...\nOn the off-chance we get a successful transfer, investigate.\n");
            
            //** Create these accounts - on the off-chance it does transfer, do investigate, the bank would essentially be duplicating money (O_o) **//
            BankAccountSafe account_A = new BankAccountSafe(1000, "AccountA");
            BankAccountSafe account_B = new BankAccountSafe(1000, "AccountB");
            
            //** This right here is the stress test that repeats the process to see that deadlocking does indeed happen **//
            for (int i = 0; i < iterations; i++)
            {
                Thread t1 = new Thread(() => TransferFunds(account_A, account_B, 100, ordered: false));
                Thread t2 = new Thread(() => TransferFunds(account_B, account_A, 150, ordered: false));
                
                t1.IsBackground = true;
                t2.IsBackground = true;
                t1.Start();
                t2.Start();

                bool t1Finished = t1.Join(3000);
                bool t2Finished = t2.Join(3000);

                if (t1Finished && t2Finished)
                {
                    successCount++;
                    Console.WriteLine($"Attempt {i + 1}: [NOTICE] Transfer completed successfully. (Unusual Behavior)");
                }
                else
                {
                    deadlockCount++;
                    Console.WriteLine($"Attempt {i + 1}: [ERROR] Deadlock detected.");
                }
                Thread.Sleep(500); // Pause before next iteration.
            }
            Console.WriteLine($"\nStress Test Results: {successCount} successful transfers, {deadlockCount} deadlocks out of {iterations} iterations.\n");
        }
        
        //** Phase 4: Deadlock resolution by forcing a consistent lock order **//
        static void RunDeadlockResolution()
        {
            BankAccountSafe account_A = new BankAccountSafe(1000, "AccountA");
            BankAccountSafe account_B = new BankAccountSafe(1000, "AccountB");

            Thread t1 = new Thread(() => TransferFunds(account_A, account_B, 100, ordered: true));
            Thread t2 = new Thread(() => TransferFunds(account_B, account_A, 150, ordered: true));
            //** I guess lambda expressions are pretty neat **//
            
            t1.IsBackground = true;
            t2.IsBackground = true;

            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            Console.WriteLine("\nTransfers completed with deadlock resolution.");
            Console.WriteLine($"{account_A.Name} balance: {account_A.Balance}");
            Console.WriteLine($"{account_B.Name} balance: {account_B.Balance}");
        }

        //** Phase 4 method may be right there, but we are going use a helper method to transfer the funds between the two accounts **//
        //** The 'ordered' flag as shown in phase 3 and 4 determines whether locks are acquired in a consistent order **//
        //** This was another reason behind the overcomplicated method of creating the stress test **//
        static void TransferFunds(BankAccountSafe from, BankAccountSafe to, int amount, bool ordered)
        {
            if (ordered)
            {
                //** Lock ordering based on the account names to avoid deadlock **//
                BankAccountSafe first = String.CompareOrdinal(from.Name, to.Name) < 0 ? from : to;
                BankAccountSafe second = (first == from) ? to : from;
                
                lock (first.LockObj)
                {
                    Thread.Sleep(150);
                    
                    lock (second.LockObj)
                    {
                        from.Withdraw(amount);
                        to.Deposit(amount);
                        
                        Console.WriteLine($"Transferred {amount} from {from.Name} to {to.Name} (ordered).");
                    }
                }
            }
            
            else
            {
                //** Without consistent lock order a deadlock could occur **//
                lock (from.LockObj)
                {
                    Thread.Sleep(150);
                    
                    lock (to.LockObj)
                    {
                        from.Withdraw(amount);
                        to.Deposit(amount);
                        
                        Console.WriteLine($"Transferred {amount} from {from.Name} to {to.Name} (unordered).");
                    }
                }
            }
        }
    }

    
    ///*** CLASSES ***///
    class BankAccountUnsafe
    {
        public int Balance { get; private set; }
        public BankAccountUnsafe(int initialBalance)
        {
            Balance = initialBalance;
        }
        public void Deposit(int amount) { Balance += amount; }
        public void Withdraw(int amount) { Balance -= amount; }
    }
    
    
    class BankAccountSafeMutex
    {
        public int Balance { get; private set; }
        private Mutex _mutex = new Mutex();

        public BankAccountSafeMutex(int initialBalance)
        {
            Balance = initialBalance;
        }

        public void Deposit(int amount)
        {
            _mutex.WaitOne();
            try { Balance += amount; }
            finally { _mutex.ReleaseMutex(); }
        }

        public void Withdraw(int amount)
        {
            _mutex.WaitOne();
            try { Balance -= amount; }
            finally { _mutex.ReleaseMutex(); }
        }
    }
    
    class BankAccountSafe
    {
        public int Balance { get; private set; }
        public string Name { get; }
        public object LockObj { get; } = new();

        public BankAccountSafe(int initialBalance, string name)
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