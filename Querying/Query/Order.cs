using System.Linq;
using Querying.Data;

namespace Querying.Query
{
    public class Order : IOperation
    {
        public string Key { get; }
        public IOperation CollectionOperation { get; }

        public Order(string key, IOperation collectionOperation)
        {
            Key = key;
            CollectionOperation = collectionOperation;
        }
        
        public EntryCollection RunOperation()
        {
            var collection = CollectionOperation.RunOperation();
            var orderedEntries = collection.Entries
                .OrderBy(e => e.Fields[Key]);

            return new EntryCollection
            {
                CollectionAlias = collection.CollectionAlias,
                Entries = orderedEntries.ToArray(),
                Keys = collection.Keys
            };
        }
    }
}