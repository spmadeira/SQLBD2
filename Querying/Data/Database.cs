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
                Entries = new List<Entry>(),
                Keys = schema.Select(s => new FieldIdentifier(name,s)).ToArray()
            });
        }

        public void Insert(string tableName, object[] values)
        {
            var table = Entries
                .FirstOrDefault(ec => ec.CollectionAlias.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));

            if (table == default)
                throw new System.Exception($"No table named {tableName}");
            
            if (values.Length != table.Keys.Length)
                throw new System.Exception($"Invalid keys for {tableName}");

            var fields = new Dictionary<FieldIdentifier, object>();

            for (int i = 0; i < table.Keys.Length; i++)
            {
                fields[table.Keys[i]] = values[i];
            }
            
            // table.Entries = table.Entries.Append(new Entry
            // {
            //     Fields = fields
            // }).ToArray();
            table.Entries.Add(new Entry{Fields = fields});
        }
    }
}