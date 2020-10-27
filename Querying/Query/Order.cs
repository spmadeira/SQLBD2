using System.Linq;
using Querying.Data;

namespace Querying.Query
{
    public class Order : IOperation
    {
        public string Key { get; }
        public IOperation CollectionOperation { get; set; }

        public Order(string key, IOperation collectionOperation)
        {
            Key = key;
            CollectionOperation = collectionOperation;
        }
        
        public QueryContext RunOperation()
        {
            var context = CollectionOperation.RunOperation();
            var orderedEntries = context.EntryCollection
                .Entries
                .OrderBy(e => context.GetFieldByName(e, Key));

            return new QueryContext
            {
                EntryCollection = new EntryCollection
                {
                    CollectionAlias = context.EntryCollection.CollectionAlias,
                    Entries = orderedEntries.ToList(),
                    Keys = context.EntryCollection.Keys.ToArray()
                },
                IncludedTables = context.IncludedTables.ToArray()
            };
        }

        public IOperation[] ChildOperations => new []{ CollectionOperation };

        public string OperationDescription => $"Order By {Key}";
    }
}