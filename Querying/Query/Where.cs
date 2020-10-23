using System;
using System.Linq;
using Querying.Conditions;
using Querying.Data;
using Querying.Query;

public class Where : IOperation
{
    public ICondition Condition { get; }
    public IOperation CollectionOperation { get; set; }
    
    public Where(ICondition condition, IOperation collectionOperation)
    {
        Condition = condition;
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
                    .Where(e => Condition.IsTrue(queryContext, e))
                    .ToList(),
                Keys = queryContext.EntryCollection.Keys.ToArray()
            },
            IncludedTables = queryContext.IncludedTables.ToArray()
        };
    }
}
