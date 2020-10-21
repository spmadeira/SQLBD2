using System;
using System.Collections.Generic;
using System.Linq;

namespace Querying.Data
{
    public class Database
    {
        public List<EntryCollection> Entries { get; } = new List<EntryCollection>();

        public void AddTable(string name, string[] schema)
        {
            Entries.Add(new EntryCollection
            {
                CollectionAlias = name,
                Entries = new Entry[0],
                Keys = schema.ToArray()
            });
        }

        public void Insert(string tableName, Dictionary<string, object> keys)
        {
            var table = Entries
                .FirstOrDefault(ec => ec.CollectionAlias.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));

            if (table == default)
                throw new System.Exception("no table");

            if (keys.Keys.Except(table.Keys).Any())
                throw new System.Exception("invalid keys");

            table.Entries = table.Entries.Append(new Entry
            {
                Fields = new Dictionary<string, object>(keys.ToArray())
            }).ToArray();
        }
    }
}