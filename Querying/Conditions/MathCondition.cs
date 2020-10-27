using System;
using System.Linq;
using Querying.Data;

namespace Querying.Conditions
{
    public class MathCondition : ICondition
    {
        public enum MathOperation
        {
            LessThanOrEqualTo,
            LessThan,
            GreaterThan,
            GreaterThanOrEqualTo
        }

        public MathOperation Operation { get; }

        public Variable Variable1 { get; }
        public Variable Variable2 { get; }

        public MathCondition(Variable var1, Variable var2, MathOperation operation)
        {
            Variable1 = var1;
            Variable2 = var2;
            Operation = operation;

            //check if valid
            if (var1.Type == typeof(string) || var2.Type == typeof(string))
                throw new System.Exception("Can't apply math to strings");

            //check if same types or variable
            if (var1.Type != typeof(FieldIdentifier)
                && var2.Type != typeof(FieldIdentifier)
                && var1.Type != var2.Type)
                throw new System.Exception("Comparison types must be equal or identifier");

            //Add tables
            if (var1.Type == typeof(FieldIdentifier))
            {
                var type = (FieldIdentifier) var1.Value;
                if (!string.IsNullOrWhiteSpace(type.FieldSourceName))
                    _tables = _tables.Append(type.FieldSourceName).ToArray();
            }

            if (var2.Type == typeof(FieldIdentifier))
            {
                var type = (FieldIdentifier) var2.Value;
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

            var c1 = v1 as IComparable;
            var c2 = v2 as IComparable;

            if (c1 == null || c2 == null)
                throw new System.Exception();

            return Operation switch
            {
                MathOperation.LessThanOrEqualTo => c1.CompareTo(c2) <= 0,
                MathOperation.LessThan => c1.CompareTo(c2) < 0,
                MathOperation.GreaterThan => c1.CompareTo(c2) > 0,
                MathOperation.GreaterThanOrEqualTo => c1.CompareTo(c2) >= 0,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private readonly string[] _tables = new string[0];

        public string[] InvolvedTables => _tables;

        public string ConditionDescription =>
            $"{Variable1} {(Operation switch {MathOperation.LessThanOrEqualTo => "<=", MathOperation.LessThan => "<", MathOperation.GreaterThan => ">", MathOperation.GreaterThanOrEqualTo => ">=", _ => throw new ArgumentException()})} {Variable2}";
    }
}