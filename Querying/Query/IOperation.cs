using Querying.Data;

namespace Querying.Query
{
    public interface IOperation
    {
        EntryCollection RunOperation();
    }
}