using Querying.Data;

namespace Querying.Query
{
    public interface IOperation
    {
        QueryContext RunOperation();
    }
}