using System;
using System.Collections.Generic;
using System.Diagnostics;
using Querying.Data;
using Querying.Query;

public class Program
{
    public static void Main(string[] args)
    {
        var database = new Database();
        MockData.Seed(database);
        
        Console.WriteLine("Database seeded. Please input query.");
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            var input = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Black;
            
            if (input!.Trim().Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                break;
            
            var operation = Utils.BuildOperation(input+";", database);
            PrintOperation(operation);
        }
    }

    private static void PrintOperation(IOperation operation)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            var results = operation.RunOperation();
            sw.Stop();

            Console.WriteLine(
                $"Table: {results.EntryCollection.CollectionAlias} -- Rows: {results.EntryCollection.Entries.Count}");
            foreach (var result in results.EntryCollection.Entries)
            {
                Console.WriteLine("Entry: --- ");
                foreach (var key in results.EntryCollection.Keys)
                {
                    Console.Write($"{key}: {result.Fields[key]} ");
                }

                Console.WriteLine();
            }


            Console.WriteLine($"Query Duration: {sw.ElapsedMilliseconds}ms");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.Message}");
        }
        
    }
}
