using System;
using System.Collections.Generic;
using System.Linq;
using Querying.Data;
using Querying.Query;

public class Join : IOperation
{
    public Func<Entry, bool> Joiner { get; }
    public IOperation CollectionOperation { get; }
    public IOperation ForeignCollectionOperation { get; }
    
    public Join(
        Func<Entry, bool> joiner, 
        IOperation collectionOperation, 
        IOperation foreignCollectionOperation)
    {
        Joiner = joiner;
        CollectionOperation = collectionOperation;
        ForeignCollectionOperation = foreignCollectionOperation;
    }

    public EntryCollection RunOperation()
    {
        var collection = CollectionOperation.RunOperation();
        var fCollection = ForeignCollectionOperation.RunOperation();
        
        var joinedEntries = new List<Entry>();

        foreach (var entry in collection.Entries)
        {
            foreach (var fEntry in fCollection.Entries)
            {
                var joinedEntry = new Entry
                {
                    Fields = JoinFields(
                        entry.Fields, 
                        collection.Keys, 
                        collection.CollectionAlias,
                        fEntry.Fields, 
                        fCollection.Keys,
                        fCollection.CollectionAlias)
                };
                
                if (Joiner(joinedEntry))
                    joinedEntries.Add(joinedEntry);
            }
        }
        
        return new EntryCollection
        {
            CollectionAlias = $"{collection.CollectionAlias}.{fCollection.CollectionAlias}",
            Keys = collection.Keys.Select(k => $"{collection.CollectionAlias}.{k}").Concat(fCollection.Keys.Select(k => $"{fCollection.CollectionAlias}.{k}")).ToArray(),
            Entries = joinedEntries.ToArray()
        };
    }

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
}
