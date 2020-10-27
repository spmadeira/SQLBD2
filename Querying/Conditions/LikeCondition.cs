using System.Linq;
using System.Text.RegularExpressions;
using Querying.Data;

namespace Querying.Conditions
{
    public class LikeCondition : ICondition
    {
        public Variable Variable1 { get; }
        public Variable Variable2 { get; }

        public LikeCondition(Variable variable1, Variable variable2)
        {
            Variable1 = variable1;
            Variable2 = variable2;

            if (variable2.Type != typeof(string))
                throw new System.Exception("Right-hand operator of Like must be string");
            
            //Add tables
            if (Variable1.Type == typeof(FieldIdentifier))
            {
                var type = (FieldIdentifier) Variable1.Value;
                if (!string.IsNullOrWhiteSpace(type.FieldSourceName))
                    _tables = _tables.Append(type.FieldSourceName).ToArray();
            }
        }

        public bool IsTrue(QueryContext context, Entry entry)
        {
            var stringValue = Variable2.Value.ToString();
            var asRegexString = stringValue
                .Replace("%", ".*")
                .Replace("_", ".");
            
            var regex = new Regex(asRegexString, RegexOptions.IgnoreCase);

            var v = Variable1.Type == typeof(FieldIdentifier)
                ? context.GetField(entry, (FieldIdentifier) Variable1.Value)
                : Variable1.Value;

            var str = v.ToString() == null ? string.Empty : v.ToString();
            
            var rgxMatch = regex.Match(str);
            return rgxMatch.Success && rgxMatch.Groups[0].Value.Equals(str);
        }

        private readonly string[] _tables = new string[0];

        public string[] InvolvedTables => _tables;

        public string ConditionDescription
            => $"{Variable1} like {Variable2}";
    }
}