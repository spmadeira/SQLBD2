using Querying.Data;

namespace Querying.Query
{
    public interface IOperation
    {
        QueryContext RunOperation();
        IOperation CollectionOperation { get; } //mudar pra array por causa do JOIN
    }
}