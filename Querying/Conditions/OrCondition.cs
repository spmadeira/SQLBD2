using System.Linq;
using Querying.Data;

namespace Querying.Conditions
{
    public class OrCondition : ICondition
    {
        public ICondition First { get; }
        public ICondition Second { get; }
        
        public bool IsTrue(QueryContext context, Entry entry)
        {
            return First.IsTrue(context, entry) || Second.IsTrue(context, entry);
        }

        public OrCondition(ICondition first, ICondition second)
        {
            First = first;
            Second = second;
        }

        public string[] InvolvedTables => First.InvolvedTables.Concat(Second.InvolvedTables).ToArray();

        public string ConditionDescription => $"{First.ConditionDescription} || {Second.ConditionDescription}";

        public int Complexity => (int) ((First.Complexity + Second.Complexity) * 1.5f);
    }
}