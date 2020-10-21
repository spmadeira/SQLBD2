using System.Linq;
using Querying.Data;

namespace Querying.Query
{
    public class Select : IOperation
    {
        public string[] Parameters { get; }
        public IOperation CollectionOperation { get; }

        public Select(IOperation collectionOperation)
        {
            CollectionOperation = collectionOperation;
        }

        public Select(string[] parameters, IOperation collectionOperation) : this(collectionOperation)
        {
            Parameters = parameters.ToArray();
        }

        public EntryCollection RunOperation()
        {
            var collection = CollectionOperation.RunOperation();
            
            if (Parameters != null && Parameters.Any())
            {
                return new EntryCollection
                {
                    CollectionAlias = collection.CollectionAlias,
                    Entries = collection.Entries.ToArray(),
                    Keys = collection.Keys.Intersect(Parameters).ToArray(),
                };
            }
            else
            {
                return new EntryCollection
                {
                    CollectionAlias = collection.CollectionAlias,
                    Entries = collection.Entries.ToArray(),
                    Keys = collection.Keys.ToArray()
                };
            }
        }
    }
}