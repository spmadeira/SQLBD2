using System.Collections.Generic;

namespace Querying.Data
{
    public class EntryCollection
    {
        public string CollectionAlias { get; set; }
        public string[] Keys { get; set;  }
        public Entry[] Entries { get; set;  }
    }
}