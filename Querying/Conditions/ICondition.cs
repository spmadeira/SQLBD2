﻿using Querying.Data;

namespace Querying.Conditions
{
    public interface ICondition
    {
        bool IsTrue(QueryContext context, Entry entry);
        string[] InvolvedTables { get; }
        public string ConditionDescription { get; }
        int Complexity { get; }
    }
}