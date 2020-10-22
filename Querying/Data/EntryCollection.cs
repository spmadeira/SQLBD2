using System.Collections.Generic;

namespace Querying.Data
{
    public class EntryCollection
    {
        public string CollectionAlias { get; set; }
        public FieldIdentifier[] Keys { get; set;  }
        public List<Entry> Entries { get; set; }
    }
}