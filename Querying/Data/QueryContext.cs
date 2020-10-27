using System;
using System.Linq;
using Querying.Query;

namespace Querying.Data
{
    public class QueryContext
    {
        public string[] IncludedTables { get; set; }
        public EntryCollection EntryCollection { get; set; }

        public object GetFieldByName(Entry entry, string name)
        {
            var identifier = GetFieldIdentifierFromName(name);
            return entry.Fields[identifier];
        }

        public object GetField(Entry entry, FieldIdentifier identifier)
        {
            if (IncludedTables.Length < 2)
            {
                var key = EntryCollection.Keys.FirstOrDefault(k =>
                    k.FieldName.Equals(identifier.FieldName, StringComparison.InvariantCultureIgnoreCase));
                
                if (key.Equals(default))
                    throw new System.Exception($"{identifier} not found in context.");

                return entry.Fields[key];
            }
            if (!EntryCollection.Keys.Contains(identifier))
                throw new System.Exception($"{identifier} not found in context.");
            return entry.Fields[identifier];
        }

        public object GetFieldByName(Entry entry, string name, Func<object> @default)
        {
            try
            {
                return GetFieldByName(entry, name);
            }
            catch
            {
                return @default();
            }
        }

        public FieldIdentifier GetFieldIdentifierFromName(string name)
        {
            if (name.Contains(".")) //is TABLE.FIELD
            {
                var parts = name.Split(".", 2);
                var identifier = new FieldIdentifier(parts[0],parts[1]);
                if (EntryCollection.Keys.Contains(identifier))
                    return identifier;
                else
                    throw new System.Exception($"Identifier {name} not found.");
            }
            else //is FIELD
            {
                var identifiers = EntryCollection.Keys.Where(i =>
                    i.FieldName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();
                
                if (!identifiers.Any())
                    throw new System.Exception($"Identifier {name} not found.");
                else if (identifiers.Length > 1)
                    throw new System.Exception($"Ambiguous identifier {name}");
                else
                    return identifiers.First();
            }
        }

        public bool CanGetIdentifierFromName(string name)
        {
            try
            {
                GetFieldIdentifierFromName(name);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}