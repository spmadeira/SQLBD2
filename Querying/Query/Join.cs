using System;
using System.Collections.Generic;
using System.Linq;
using Querying.Data;
using Querying.Query;

public class Join : IOperation
{
    public Func<QueryContext, Entry, bool> Joiner { get; }
    public IOperation CollectionOperation { get; }
    public IOperation ForeignCollectionOperation { get; }
    
    public Join(
        Func<QueryContext, Entry, bool> joiner, 
        IOperation collectionOperation, 
        IOperation foreignCollectionOperation)
    {
        Joiner = joiner;
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
                
                if (Joiner(mergedContext, joinedEntry))
                    joinedEntries.Add(joinedEntry);
            }
        }

        mergedContext.EntryCollection.Entries = joinedEntries.ToArray();

        return mergedContext;
    }

    [Obsolete]
    private Dictionary<string, object> JoinFields(
        Dictionary<string, object> fields, 
        string[] keys, 
        string name, 
        Dictionary<string, object> foreignFields, 
        string[] foreignKeys,
        string foreignName)
    {
        var newFields = new Dictionary<string,object>();
    
        foreach (var key in keys)
        {
            newFields[$"{name}.{key}"] = fields[key];
        }
    
        foreach (var key in foreignKeys)
        {
            newFields[$"{foreignName}.{key}"] = foreignFields[key];
        }
    
        return newFields;
    }

    private Entry MergeEntries(Entry entry, Entry fEntry)
    {
        var kvps = entry.Fields.Concat(fEntry.Fields).ToArray();
        return new Entry
        {
            Fields = new Dictionary<FieldIdentifier, object>(kvps)
        };
    }
}
