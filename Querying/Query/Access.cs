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

        public QueryContext RunOperation()
        {
            var col = Database.Entries
                .First(ec =>
                    ec.CollectionAlias.Equals(TableName, StringComparison.InvariantCultureIgnoreCase));

            return new QueryContext
            {
                IncludedTables = new []{col.CollectionAlias},
                EntryCollection = new EntryCollection
                {
                    CollectionAlias = col.CollectionAlias,
                    Entries = col.Entries.ToList(),
                    Keys = col.Keys.ToArray()
                }
            };
        }
    }
}