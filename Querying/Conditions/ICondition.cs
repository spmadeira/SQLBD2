using Querying.Data;

namespace Querying.Conditions
{
    public interface ICondition
    {
        bool IsTrue(QueryContext context, Entry entry);
        string[] InvolvedTables { get; }
    }
}