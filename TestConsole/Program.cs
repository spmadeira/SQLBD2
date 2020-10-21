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
            var input = Console.ReadLine() + ";";
            Console.ForegroundColor = ConsoleColor.Black;
            var operation = Utils.BuildOperation(input, database);
            printOp(operation);
        }
    }

    private static void printOp(IOperation operation)
    {
        var results = operation.RunOperation();
        
        Console.WriteLine($"Table: {results.CollectionAlias}");
        foreach (var result in results.Entries)
        {
            Console.WriteLine("Entry: --- ");
            foreach (var key in results.Keys)
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
        database.Insert("User", new Dictionary<string, object> {{"id", 0}, {"name", "User1"}, {"points", 48}});
        database.Insert("User", new Dictionary<string, object> {{"id", 1}, {"name", "User2"}, {"points", 800}});
        database.Insert("User", new Dictionary<string, object> {{"id", 2}, {"name", "User3"}, {"points", 55}});
        database.Insert("User", new Dictionary<string, object> {{"id", 3}, {"name", "User4"}, {"points", 12}});
        database.Insert("User", new Dictionary<string, object> {{"id", 4}, {"name", "User5"}, {"points", 0}});
        database.Insert("User", new Dictionary<string, object> {{"id", 5}, {"name", "User6"}, {"points", 90}});
        
        database.AddTable("Transaction", new []{"id", "userId", "date"});
        database.Insert("Transaction", new Dictionary<string, object>{{"id", 0}, {"userId", 0}, {"date", DateTime.Today}});
        database.Insert("Transaction", new Dictionary<string, object>{{"id", 1}, {"userId", 0}, {"date", DateTime.Today}});
        database.Insert("Transaction", new Dictionary<string, object>{{"id", 2}, {"userId", 1}, {"date", DateTime.Today}});
        database.Insert("Transaction", new Dictionary<string, object>{{"id", 3}, {"userId", 1}, {"date", DateTime.Today}});
        database.Insert("Transaction", new Dictionary<string, object>{{"id", 4}, {"userId", 2}, {"date", DateTime.Today}});
    }
    
    private static void testqueries()
    {
        var database = new Database();
        seed(database);
        
        //SELECT name, points
        //FROM User
        //WHERE points >= 30
        //ORDERBY points

        {
            var accessOperation = new Access("User", database);
            var whereOperation = new Where(e => (int)e.Fields["points"] >= 30, accessOperation);
            var orderOperation = new Order("points", whereOperation);
            var selectOperation = new Select(new []{"name", "points"}, orderOperation);

            var results = selectOperation.RunOperation();

            Console.WriteLine($"Table: {results.CollectionAlias}");
            foreach (var result in results.Entries)
            {
                Console.WriteLine("Entry: --- ");
                foreach (var key in results.Keys)
                {
                    Console.Write($"{key}: {result.Fields[key]} ");
                }
                Console.WriteLine();
            }
        }
        
        
        //SELECT *
        //FROM User
        //JOIN Transaction
        //ON Transaction.userId = User.id

        {
            var userAccessOperation = new Access("User",database);
            var transactionAccessOperation = new Access("Transaction", database);
            var joinOperation = new Join(
                (e) => e.Fields["Transaction.userId"].Equals(e.Fields["User.id"]), 
                userAccessOperation, 
                transactionAccessOperation);
            var selectOperation = new Select(joinOperation);
            
            var results = selectOperation.RunOperation();

            Console.WriteLine($"Table: {results.CollectionAlias}");
            foreach (var result in results.Entries)
            {
                Console.WriteLine("Entry: --- ");
                foreach (var key in results.Keys)
                {
                    Console.Write($"{key}: {result.Fields[key]} ");
                }
                Console.WriteLine();
            }
        }
    }
}
