using System.Linq;
using Querying.Data;

namespace Querying.Conditions
{
    public class EqualityCondition : ICondition
    {
        public bool ExpectedResult { get; }
     
        public Variable Variable1 { get; }
        public Variable Variable2 { get; }

        public EqualityCondition(Variable var1, Variable var2, bool expectedResult)
        {
            Variable1 = var1;
            Variable2 = var2;
            ExpectedResult = expectedResult;
            
            //check if same types or variable
            if (var1.Type != typeof(FieldIdentifier)
                && var2.Type != typeof(FieldIdentifier)
                && var1.Type != var2.Type)
                throw new System.Exception("Comparison types must be equal or identifier");
            
            //Add tables
            if (var1.Type == typeof(FieldIdentifier))
            {
                var type = (FieldIdentifier)var1.Value;
                if (!string.IsNullOrWhiteSpace(type.FieldSourceName))
                    _tables = _tables.Append(type.FieldSourceName).ToArray();
            }
            if (var2.Type == typeof(FieldIdentifier))
            {
                var type = (FieldIdentifier)var2.Value;
                if (!string.IsNullOrWhiteSpace(type.FieldSourceName))
                    _tables = _tables.Append(type.FieldSourceName).ToArray();
            }
        }

        public bool IsTrue(QueryContext context, Entry entry)
        {
            var v1 = Variable1.Type == typeof(FieldIdentifier)
                ? context.GetField(entry, (FieldIdentifier) Variable1.Value)
                : Variable1.Value;
            
            var v2 = Variable2.Type == typeof(FieldIdentifier)
                ? context.GetField(entry, (FieldIdentifier) Variable2.Value)
                : Variable2.Value;

            return v1.Equals(v2) == ExpectedResult;
        }

        private readonly string[] _tables = new string[0];

        public string[] InvolvedTables => _tables;

        public string ConditionDescription => $"{Variable1} {(ExpectedResult ? "==" : "!=")} {Variable2}";
    }
}