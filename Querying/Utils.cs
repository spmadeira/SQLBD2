using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Querying.Data;
using Querying.Query;

public class Utils
{
    public static Regex MainQueryRegex = 
        new Regex(@"(?:select) (.*) (?:from) (\w*)(.*)", 
            (RegexOptions.IgnoreCase));
    
    public static Regex WhereRegex =
        new Regex(@"(?:where) (.*?)(?=join|on|orderby|where|;)",
            (RegexOptions.IgnoreCase));
    
    public static Regex JoinRegex = 
        new Regex(@"(?:join) (.*?) (?:on) (.*?)(?=join|orderby|where|;)",
            (RegexOptions.IgnoreCase));
    
    public static Regex OrderByRegex = 
        new Regex(@"(?:orderby) (.*?)(?=join|where|orderby|;)",
            (RegexOptions.IgnoreCase));
    
    public static Regex AndRegex =
        new Regex(@"(.*) (?:and) (.*)",
            (RegexOptions.IgnoreCase));
    
    public static Regex OrRegex =
        new Regex(@"(.*) (?:or) (.*)",
            (RegexOptions.IgnoreCase));
    
    public static Regex LessThanOrEqualToRegex =
        new Regex(@"(.*) (?:<=) (.*)",
            (RegexOptions.IgnoreCase));
    
    public static Regex MoreThanOrEqualToRegex =
        new Regex(@"(.*) (?:>=) (.*)",
            (RegexOptions.IgnoreCase));
    
    public static Regex LessThanRegex =
        new Regex(@"(.*) (?:<) (.*)",
            (RegexOptions.IgnoreCase));
    
    public static Regex MoreThanRegex =
        new Regex(@"(.*) (?:>) (.*)",
            (RegexOptions.IgnoreCase));
    
    public static Regex EqualToRegex =
        new Regex(@"(.*) (?:=) (.*)",
            (RegexOptions.IgnoreCase));
    
    public static Regex UnequalToRegex =
        new Regex(@"(.*) (?:<>) (.*)",
            (RegexOptions.IgnoreCase));
    
    public static Regex InRegex = 
        new Regex(@"(.*) (?:in) \[(.*)\]",
            (RegexOptions.IgnoreCase));
    
    public static Regex NotInRegex = 
        new Regex(@"(.*) (?:not in) \[(.*)\]",
            (RegexOptions.IgnoreCase));
    
    public static void PrintEntries(IEnumerable<Entry> entries, string tableName = "")
    {
        Console.WriteLine(tableName);
        Console.WriteLine("========================");
        
        //TODO
        
    }

    public static IOperation BuildOperation(string queryString, Database database)
    {
        //var ctx = new QueryContext();
        var match = MainQueryRegex.Match(queryString);
        if (match.Success)
        {
            var mainAccess = match.Groups[2].Value.Trim();
            var access = new Access(mainAccess, database);
            var tree = BuildTree(access, match.Groups[3].Value.Trim(), database);
            
            string[] selectParams;
            var selectFields = match.Groups[1].Value.Trim();
            if (selectFields == "*") selectParams = new string[0];
            else selectParams = selectFields.Split(",").Select(s => s.Trim()).ToArray();
            return new Select(selectParams, tree);
        }
        else
        {
            throw new Exception("No match");
        }
    }

    public static IOperation BuildTree(Access mainAccess, string operationQuery, Database database)
    {
        IOperation lastOperation = mainAccess;
        
        var joinMatch = JoinRegex.Match(operationQuery);
        if (joinMatch.Success) //ver pra substituir por while .Sucess e botar um .NextMatch() no final
        {
            var groups = joinMatch.Groups.Values.Select(g => g.Value).ToArray();
            lastOperation = new Join
            (
                BuildConditionTree(joinMatch.Groups[2].Value.Trim()),
                mainAccess,
                new Access(joinMatch.Groups[1].Value.Trim(), database)
                );
        }

        var whereMatch = WhereRegex.Match(operationQuery);
        if (whereMatch.Success)
        {
            lastOperation = new Where(
                BuildConditionTree(whereMatch.Groups[1].Value.Trim()),
                lastOperation
                );
        }

        var orderByMatch = OrderByRegex.Match(operationQuery);
        if (orderByMatch.Success)
        {
            lastOperation = new Order(orderByMatch.Groups[1].Value.Trim(), lastOperation);
        }

        return lastOperation;
    }

    public static Func<QueryContext, Entry, bool> BuildConditionTree(string conditionString)
    {
        {
            // if (conditionString.Contains("AND"))
            // {
            //     var halves = conditionString.Split("AND", 2);
            //     var c1 = BuildConditionTree(halves[0].Trim());
            //     var c2 = BuildConditionTree(halves[1].Trim());
            //     return (q,e) => c1(q,e) && c2(q,e);
            // }
            //
            // if (conditionString.Contains("and"))
            // {
            //     var halves = conditionString.Split("and", 2);
            //     var c1 = BuildConditionTree(halves[0].Trim());
            //     var c2 = BuildConditionTree(halves[1].Trim());
            //     return (q,e) => c1(q,e) && c2(q,e);
            // }
            //
            // if (conditionString.Contains("OR"))
            // {
            //     var halves = conditionString.Split("OR",2);
            //     var c1 = BuildConditionTree(halves[0].Trim());
            //     var c2 = BuildConditionTree(halves[1].Trim());
            //     return (q,e) => c1(q,e) || c2(q,e);
            // }
            //
            // if (conditionString.Contains("or"))
            // {
            //     var halves = conditionString.Split("or",2);
            //     var c1 = BuildConditionTree(halves[0].Trim());
            //     var c2 = BuildConditionTree(halves[1].Trim());
            //     return (q,e) => c1(q,e) || c2(q,e);
            // }
            //
            // if (conditionString.Contains(">="))
            // {
            //     var halves = conditionString.Split(">=",2);
            //     var k1 = halves[0].Trim();
            //     var k2 = halves[1].Trim();
            //     return (q, e) =>
            //     {
            //         var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
            //         var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
            //         return v1 >= v2;
            //     };
            // }
            //
            // if (conditionString.Contains("<="))
            // {
            //     var halves = conditionString.Split("<=", 2);
            //     var k1 = halves[0].Trim();
            //     var k2 = halves[1].Trim();
            //     return (q,e) =>
            //     {
            //         var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
            //         var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
            //         return v1 <= v2;
            //     };
            // }
            //
            // if (conditionString.Contains("<>"))
            // {
            //     var halves = conditionString.Split("<>",2);
            //     var k1 = halves[0].Trim();
            //     var k2 = halves[1].Trim();
            //     return (q,e) =>
            //     {
            //         //Ver como comparar fora int
            //         var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
            //         var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
            //         return !v1.Equals(v2);
            //     };
            // }
            //
            // if (conditionString.Contains("="))
            // {
            //     var halves = conditionString.Split("=",2);
            //     var k1 = halves[0].Trim();
            //     var k2 = halves[1].Trim();
            //     return (q,e) =>
            //     {
            //         //Ver como comparar fora int
            //         var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
            //         var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
            //         return v1.Equals(v2);
            //     };
            // }
            //
            // if (conditionString.Contains(">"))
            // {
            //     var halves = conditionString.Split(">",2);
            //     var k1 = halves[0].Trim();
            //     var k2 = halves[1].Trim();
            //     return (q,e) =>
            //     {
            //         var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
            //         var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
            //         return v1 > v2;
            //     };
            // }
            //
            // if (conditionString.Contains("<"))
            // {
            //     var halves = conditionString.Split("<",2);
            //     var k1 = halves[0].Trim();
            //     var k2 = halves[1].Trim();
            //     return (q,e) =>
            //     {
            //         var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
            //         var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
            //         return v1 < v2;
            //     };
            // }
        }

        var andMatch = AndRegex.Match(conditionString);
        if (andMatch.Success)
        {
            var c1 = BuildConditionTree(andMatch.Groups[1].Value);
            var c2 = BuildConditionTree(andMatch.Groups[2].Value);
            return (q, e) => c1(q, e) && c2(q, e);
        }
        
        var orMatch = OrRegex.Match(conditionString);
        if (orMatch.Success)
        {
            var c1 = BuildConditionTree(orMatch.Groups[1].Value);
            var c2 = BuildConditionTree(orMatch.Groups[2].Value);
            return (q, e) => c1(q, e) || c2(q, e);
        }
        
        var ltoetMatch = LessThanOrEqualToRegex.Match(conditionString);
        if (ltoetMatch.Success)
        {
            var k1 = ltoetMatch.Groups[1].Value.Trim();
            var k2 = ltoetMatch.Groups[2].Value.Trim();
            return (q, e) =>
            {
                var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
                var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
                return v1 <= v2;
            };
        }

        var mtoetMatch = MoreThanOrEqualToRegex.Match(conditionString);
        if (mtoetMatch.Success)
        {
            var k1 = mtoetMatch.Groups[1].Value.Trim();
            var k2 = mtoetMatch.Groups[2].Value.Trim();
            return (q, e) =>
            {
                var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
                var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
                return v1 >= v2;
            };
        }

        var unequalMatch = UnequalToRegex.Match(conditionString);
        if (unequalMatch.Success)
        {
            var k1 = unequalMatch.Groups[1].Value.Trim();
            var k2 = unequalMatch.Groups[2].Value.Trim();
            return (q, e) =>
            {
                var v1 = q.GetFieldByName(e, k1, () =>
                {
                    var isInt = int.TryParse(k1, out var val);
                    if (isInt) return val;
                    else return k1;
                });
                var v2 = q.GetFieldByName(e, k2, () =>
                {
                    var isInt = int.TryParse(k1, out var val);
                    if (isInt) return val;
                    else return k2;
                });
                return !v1.Equals(v2);
            };
        }

        var equalToMatch = EqualToRegex.Match(conditionString);
        if (equalToMatch.Success)
        {
            var k1 = equalToMatch.Groups[1].Value.Trim();
            var k2 = equalToMatch.Groups[2].Value.Trim();
            return (q, e) =>
            {
                var v1 = q.GetFieldByName(e, k1, () =>
                {
                    var isInt = int.TryParse(k1, out var val);
                    if (isInt) return val;
                    else return k1;
                });
                var v2 = q.GetFieldByName(e, k2, () =>
                {
                    var isInt = int.TryParse(k1, out var val);
                    if (isInt) return val;
                    else return k2;
                });
                return v1.Equals(v2);
            };
        }

        var lessMatch = LessThanRegex.Match(conditionString);
        if (lessMatch.Success)
        {
            var k1 = lessMatch.Groups[1].Value.Trim();
            var k2 = lessMatch.Groups[2].Value.Trim();
            return (q, e) =>
            {
                var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
                var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
                return v1 < v2;
            };
        }

        var moreMatch = MoreThanRegex.Match(conditionString);
        if (moreMatch.Success)
        {
            var k1 = moreMatch.Groups[1].Value.Trim();
            var k2 = moreMatch.Groups[2].Value.Trim();
            return (q, e) =>
            {
                var v1 = (int) q.GetFieldByName(e, k1, () => int.Parse(k1));
                var v2 = (int) q.GetFieldByName(e, k2, () => int.Parse(k2));
                return v1 > v2;
            };
        }

        var notInMatch = NotInRegex.Match(conditionString);
        if (notInMatch.Success)
        {
            var k1 = notInMatch.Groups[1].Value.Trim();
            var k2 = notInMatch.Groups[2].Value.Split(",").Select(s => s.Trim()).ToArray();
            return (q, e) =>
            {
                var v1 = q.GetFieldByName(e, k1, () =>
                {
                    var isInt = int.TryParse(k1, out var val);
                    if (isInt) return val;
                    else return k1;
                });

                foreach (var possible in k2)
                {
                    var v2 = q.GetFieldByName(e, possible, () =>
                    {
                        var isInt = int.TryParse(possible, out var val);
                        if (isInt) return val;
                        else return k2;
                    });

                    if (v1.Equals(v2))
                        return false;
                }

                return true;
            };
        }
        
        var inMatch = InRegex.Match(conditionString);
        if (inMatch.Success)
        {
            var k1 = inMatch.Groups[1].Value.Trim();
            var k2 = inMatch.Groups[2].Value.Split(",").Select(s => s.Trim()).ToArray();
            return (q, e) =>
            {
                var v1 = q.GetFieldByName(e, k1, () =>
                {
                    var isInt = int.TryParse(k1, out var val);
                    if (isInt) return val;
                    else return k1;
                });

                foreach (var possible in k2)
                {
                    var v2 = q.GetFieldByName(e, possible, () =>
                    {
                        var isInt = int.TryParse(possible, out var val);
                        if (isInt) return val;
                        else return k2;
                    });

                    if (v1.Equals(v2))
                        return true;
                }

                return false;
            };
        }

        // return e => false;
        throw new Exception($"{conditionString} is invalid condition");
    }
}
