using System;
using System.Collections.Generic;
using System.Linq;
using Querying.Conditions;
using Querying.Data;
using Querying.Query;

public class Join : IOperation
{
    public ICondition Condition { get; }
    public IOperation CollectionOperation { get; }
    public IOperation ForeignCollectionOperation { get; }

    public Join(
        ICondition condition,
        IOperation collectionOperation,
        IOperation foreignCollectionOperation)
    {
        Condition = condition;
        CollectionOperation = collectionOperation;
        ForeignCollectionOperation = foreignCollectionOperation;
    }

    public QueryContext RunOperation()
    {
        var context = CollectionOperation.RunOperation();
        var fContext = ForeignCollectionOperation.RunOperation();

        var mergedContext = new QueryContext
        {
            IncludedTables = context.IncludedTables.Concat(fContext.IncludedTables).ToArray(),
            EntryCollection = new EntryCollection
            {
                CollectionAlias =
                    $"{context.EntryCollection.CollectionAlias}.{fContext.EntryCollection.CollectionAlias}",
                Keys = context.EntryCollection.Keys.Concat(fContext.EntryCollection.Keys).ToArray(),
                Entries = null
            }
        };

        var joinedEntries = new List<Entry>();

        foreach (var entry in context.EntryCollection.Entries)
        {
            foreach (var fEntry in fContext.EntryCollection.Entries)
            {
                var joinedEntry = MergeEntries(entry, fEntry);

                if (Condition.IsTrue(mergedContext, joinedEntry))
                    joinedEntries.Add(joinedEntry);
            }
        }

        mergedContext.EntryCollection.Entries = joinedEntries.ToList();

        return mergedContext;
    }

    private Entry MergeEntries(Entry entry, Entry fEntry)
    {
        var kvps = entry.Fields.Concat(fEntry.Fields).ToArray();
        return new Entry
        {
            Fields = new Dictionary<FieldIdentifier, object>(kvps)
        };
    }

    public IOperation[] ChildOperations => new[] { CollectionOperation, ForeignCollectionOperation };

    public string OperationDescription => $"Join on {Condition.ConditionDescription}";
}