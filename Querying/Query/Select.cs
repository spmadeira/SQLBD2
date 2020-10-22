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

        public QueryContext RunOperation()
        {
            var context = CollectionOperation.RunOperation();
            
            if (Parameters != null && Parameters.Any())
            {
                var identifiers = Parameters
                    .Select(p => context.GetFieldIdentifierFromName(p));
                
                return new QueryContext
                {
                    EntryCollection = new EntryCollection
                    {
                        CollectionAlias = context.EntryCollection.CollectionAlias,
                        Entries = context.EntryCollection.Entries.ToArray(),
                        Keys = context.EntryCollection.Keys.Intersect(identifiers).ToArray()
                    },
                    IncludedTables = context.IncludedTables.ToArray()
                };
            }
            else
            {
                return new QueryContext
                {
                    EntryCollection = new EntryCollection
                    {
                        CollectionAlias = context.EntryCollection.CollectionAlias,
                        Entries = context.EntryCollection.Entries.ToArray(),
                        Keys = context.EntryCollection.Keys.ToArray()
                    },
                    IncludedTables = context.IncludedTables.ToArray()
                };
            }
        }
    }
}