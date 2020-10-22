using System;
using System.Collections.Generic;
using Querying.Data;
using Querying.Query;

public class Program
{
    public static void Main(string[] args)
    {
        var database = new Database();
        seed(database);

        
        Console.WriteLine("Database seeded. Please input query.");
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            var input = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Black;
            
            if (input!.Trim().Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                Environment.Exit(0);
            
            var operation = Utils.BuildOperation(input+";", database);
            printOp(operation);
        }
    }

    private static void printOp(IOperation operation)
    {
        var results = operation.RunOperation();
        
        Console.WriteLine($"Table: {results.EntryCollection.CollectionAlias}");
        foreach (var result in results.EntryCollection.Entries)
        {
            Console.WriteLine("Entry: --- ");
            foreach (var key in results.EntryCollection.Keys)
            {
                Console.Write($"{key}: {result.Fields[key]} ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    private static void seed(Database database)
    {
        database.AddTable("User", new []{"id", "name", "points"});
        database.Insert("User", new object[]{0, "User1", 48});
        database.Insert("User", new object[]{1, "User2", 800});
        database.Insert("User", new object[]{2, "User3", 55});
        database.Insert("User", new object[]{3, "User4", 12});
        database.Insert("User", new object[]{4, "User5", 0});
        database.Insert("User", new object[]{5, "User6", 90});

        database.AddTable("Transaction", new []{"id", "userId", "date", "pointsTransferred"});
        database.Insert("Transaction", new object[]{0,0,DateTime.Today - TimeSpan.FromDays(1), 20});
        database.Insert("Transaction", new object[]{1,0,DateTime.Today, 30});
        database.Insert("Transaction", new object[]{2,1,DateTime.Today - TimeSpan.FromDays(3), 7});
        database.Insert("Transaction", new object[]{3,1,DateTime.Today - TimeSpan.FromDays(1), 25});
        database.Insert("Transaction", new object[]{4,2,DateTime.Today, 60});
    }
}
