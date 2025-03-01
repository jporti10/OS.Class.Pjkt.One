# OS.Class.Pjkt.One

Hello and welcome to my first official git project from OS Class. The prompy the teacher has given the class is to either use C#, Rust or C++ to write a program that follows the conditions in which we:

**Phase 1: Basic Thread Operations**
- Create threads that perform concurrent operations
- Demonstrate proper thread creation and management
- Show basic thread safety principles
- Banking Example: Implement threads for individual transactions

**Phase 2: Resource Protection**
- Implement mutex locks for shared resource protection
- Demonstrate proper lock acquisition and release
- Show synchronized access to shared resources
- Banking Example: Protect account access with mutexes (the banking scenario was the thing I ended up choosing)

**Phase 3: Deadlock Creation**
- Create scenarios requiring multiple resource locks
- Demonstrate how deadlocks can occur
- Implement deadlock detection
- Banking Example: Show deadlock in multi-account transfers

**Phase 4: Deadlock Resolution**
- Implement timeout mechanisms
- Add proper resource ordering
- Show deadlock prevention and recovery
- Banking Example: Resolve deadlocks through ordered account access

**Setup:** You will need to download the code from the repo or clone it into your own IDE and run it in a **C# compiler** or something equivalent/compatible. You dont neccesarily need to do it in Linux but for what I had to do this has been tested on Linux, Windows, but **not Mac yet**. It **probably** works but I haven't went that far.

**Usage:** This was just to show an example of how multhreaded ops (operations) are used and presented in the form of a C# program.

**Issues:** Only current issue I know is that for part B inside the Linux environment is that the Client side likes to send all the messages at once while in Windows it like to lag behind and not send until the message is recieved on the server side message. Other than that I don't believe there are other issues to note of.
