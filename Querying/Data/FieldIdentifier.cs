using System;

namespace Querying.Data
{
    public readonly struct FieldIdentifier
    {
        public string FieldSourceName { get; }
        public string FieldName { get; }

        public FieldIdentifier(string fieldSourceName, string fieldName)
        {
            FieldSourceName = fieldSourceName;
            FieldName = fieldName;
        }

        public override bool Equals(object? obj)
        {
            if (obj is FieldIdentifier identifier)
            {
                return FieldSourceName.Equals(identifier.FieldSourceName, StringComparison.InvariantCultureIgnoreCase)
                       && FieldName.Equals(identifier.FieldName, StringComparison.InvariantCultureIgnoreCase);
            } else return base.Equals(obj);
        }

        public bool Equals(FieldIdentifier other)
        {
            return FieldSourceName.Equals(other.FieldSourceName, StringComparison.InvariantCultureIgnoreCase) 
                   && FieldName.Equals(other.FieldName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FieldSourceName.ToLower(), FieldName.ToLower());
        }

        public override string ToString()
        {
            return $"{FieldSourceName}.{FieldName}";
        }
    }
}