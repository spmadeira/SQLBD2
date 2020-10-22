using System;
using System.Linq;
using Querying.Data;
using Querying.Query;

public class Where : IOperation
{
    public Func<QueryContext, Entry,bool> Obeys { get; }
    public IOperation CollectionOperation { get; }
    
    public Where(Func<QueryContext, Entry, bool> obeys, IOperation collectionOperation)
    {
        Obeys = obeys;
        CollectionOperation = collectionOperation;
    }

    public QueryContext RunOperation()
    {
        var queryContext = CollectionOperation.RunOperation();

        return new QueryContext
        {
            EntryCollection = new EntryCollection
            {
                CollectionAlias = queryContext.EntryCollection.CollectionAlias,
                Entries = queryContext.EntryCollection
                    .Entries
                    .Where(e => Obeys(queryContext, e))
                    .ToList(),
                Keys = queryContext.EntryCollection.Keys.ToArray()
            },
            IncludedTables = queryContext.IncludedTables.ToArray()
        };
    }
}
