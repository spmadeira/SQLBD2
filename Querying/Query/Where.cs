using System;
using System.Linq;
using Querying.Data;
using Querying.Query;

public class Where : IOperation
{
    public Func<Entry,bool> Obeys { get; }
    public IOperation CollectionOperation { get; }
    
    public Where(Func<Entry, bool> obeys, IOperation collectionOperation)
    {
        Obeys = obeys;
        CollectionOperation = collectionOperation;
    }

    public EntryCollection RunOperation()
    {
        var collection = CollectionOperation.RunOperation();
        
        return new EntryCollection
        {
            CollectionAlias = collection.CollectionAlias,
            Entries = collection.Entries
                .Where(e => Obeys(e))
                .ToArray(),
            Keys = collection.Keys.ToArray()
        };
    }
}
