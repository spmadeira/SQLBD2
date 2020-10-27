using Querying.Data;

namespace Querying.Query
{
    public interface IOperation
    {
        QueryContext RunOperation();
        IOperation[] ChildOperations { get; } //mudar pra array por causa do JOIN
        string OperationDescription { get; }
    }
}