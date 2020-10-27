using System;
using System.Linq;
using Querying.Data;

namespace Querying.Conditions
{
    public class InCondition : ICondition
    {
        public bool ExpectedResult { get; }
     
        public Variable Variable { get; }
        public Variable[] Variables { get; }

        public InCondition(Variable var1, Variable[] vars, bool expectedResult)
        {
            Variable = var1;
            Variables = vars;
            ExpectedResult = expectedResult;

            //Add tables
            if (var1.Type == typeof(FieldIdentifier))
            {
                var type = (FieldIdentifier)var1.Value;
                if (!string.IsNullOrWhiteSpace(type.FieldSourceName))
                    _tables = _tables.Append(type.FieldSourceName).ToArray();
            }
        }

        public bool IsTrue(QueryContext context, Entry entry)
        {
            if (Variables.Contains(Variable))
            {
                return ExpectedResult;
            }
            return !ExpectedResult;
        }

        private readonly string[] _tables = new string[0];

        public string[] InvolvedTables => _tables;

        public string ConditionDescription =>
            $"{Variable} in [{string.Join(",", Variables.Select(v => v.ToString()))}]";
    }
}