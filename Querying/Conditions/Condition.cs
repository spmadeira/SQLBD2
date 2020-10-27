// using System.Text.RegularExpressions;
// using Querying.Data;
//
// namespace Querying.Conditions
// {
//     public abstract class Condition : ICondition
//     {
//         public abstract Regex ConditionRegex { get; }
//
//         public abstract Condition Build(Match match);
//
//         public abstract bool IsTrue(QueryContext context, Entry entry);
//
//         public abstract string[] InvolvedTables { get; }
//     }
// }