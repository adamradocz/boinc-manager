Usage
-----

Starting and Stopping the Manager:
```csharp
BoincManager.Manager manager = new BoincManager.Manager();

// Initialize the Manager
using (var context = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
{
    BoincManager.Utils.InitializeApplication(context, manager, true);
}

// Start the Manager
System.Threading.Tasks.Task.Run(() => manager.Start());

// Stop the Manager
manager.Stop();
```

Stopping the Manager in 3 seconds:
```csharp
manager.Stop(3000).ContinueWith(task => Console.WriteLine("Error: " + task.Exception), TaskContinuationOptions.OnlyOnFaulted);
```