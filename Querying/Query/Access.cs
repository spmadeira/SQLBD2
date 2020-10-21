using System;
using System.Linq;
using Querying.Data;

namespace Querying.Query
{
    public class Access : IOperation
    {
        public string TableName { get; }
        public Database Database { get; }

        public Access(string tableName, Database database)
        {
            TableName = tableName;
            Database = database;
        }

        public EntryCollection RunOperation()
        {
            var col = Database.Entries
                .First(ec =>
                    ec.CollectionAlias.Equals(TableName, StringComparison.InvariantCultureIgnoreCase));

            return new EntryCollection
            {
                CollectionAlias = col.CollectionAlias,
                Entries = col.Entries.ToArray(),
                Keys = col.Keys.ToArray()
            };
        }
    }
}