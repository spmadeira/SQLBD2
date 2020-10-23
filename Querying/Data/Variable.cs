using System;

namespace Querying.Data
{
    public class Variable
    {
        public Type Type { get; }
        public object Value { get; }

        public Variable(Type type, object value)
        {
            Type = type;
            Value = value;
            if (value.GetType() != type) throw new System.Exception($"{value} is not {type}");
        }
    }
}