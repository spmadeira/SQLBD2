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
        
        return new Access("a",new Database());
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

    public static Func<Entry, bool> BuildConditionTree(string conditionString)
    {
        if (conditionString.Contains(" AND "))
        {
            var halves = conditionString.Split("AND");
            var c1 = BuildConditionTree(halves[0].Trim());
            var c2 = BuildConditionTree(halves[1].Trim());
            return (e) => c1(e) && c2(e);
        }
        
        if (conditionString.Contains(" and "))
        {
            var halves = conditionString.Split(" and ");
            var c1 = BuildConditionTree(halves[0].Trim());
            var c2 = BuildConditionTree(halves[1].Trim());
            return (e) => c1(e) && c2(e);
        }

        if (conditionString.Contains(" or "))
        {
            var halves = conditionString.Split(" or ");
            var c1 = BuildConditionTree(halves[0].Trim());
            var c2 = BuildConditionTree(halves[1].Trim());
            return (e) => c1(e) || c2(e);
        }

        if (conditionString.Contains(">="))
        {
            var halves = conditionString.Split(">=");
            var k1 = halves[0].Trim();
            var k2 = halves[1].Trim();
            return (e) =>
            {
                var v1 = e.Fields.ContainsKey(k1)
                    ? (int) e.Fields[k1]
                    : int.Parse(k1);

                var v2 = e.Fields.ContainsKey(k2)
                    ? (int) e.Fields[k2]
                    : int.Parse(k2);

                return v1 >= v2;
            };
        }
        
        if (conditionString.Contains("<="))
        {
            var halves = conditionString.Split("<=");
            var k1 = halves[0].Trim();
            var k2 = halves[1].Trim();
            return (e) =>
            {
                var v1 = e.Fields.ContainsKey(k1)
                    ? (int) e.Fields[k1]
                    : int.Parse(k1);

                var v2 = e.Fields.ContainsKey(k2)
                    ? (int) e.Fields[k2]
                    : int.Parse(k2);

                return v1 <= v2;
            };
        }
        
        if (conditionString.Contains("<>"))
        {
            var halves = conditionString.Split("<>");
            var k1 = halves[0].Trim();
            var k2 = halves[1].Trim();
            return (e) =>
            {
                var v1 = e.Fields.ContainsKey(k1)
                    ? e.Fields[k1]
                    : k1;

                var v2 = e.Fields.ContainsKey(k2)
                    ? e.Fields[k2]
                    : k2;

                return !v1.Equals(v2);
            };
        }
        
        if (conditionString.Contains("="))
        {
            var halves = conditionString.Split("=");
            var k1 = halves[0].Trim();
            var k2 = halves[1].Trim();
            return (e) =>
            {
                var v1 = e.Fields.ContainsKey(k1)
                    ? e.Fields[k1]
                    : k1;

                var v2 = e.Fields.ContainsKey(k2)
                    ? e.Fields[k2]
                    : k2;

                return v1.Equals(v2);
            };
        }
        
        if (conditionString.Contains(">"))
        {
            var halves = conditionString.Split(">");
            var k1 = halves[0].Trim();
            var k2 = halves[1].Trim();
            return (e) =>
            {
                var v1 = e.Fields.ContainsKey(k1)
                    ? (int) e.Fields[k1]
                    : int.Parse(k1);

                var v2 = e.Fields.ContainsKey(k2)
                    ? (int) e.Fields[k2]
                    : int.Parse(k2);

                return v1 > v2;
            };
        }
        
        if (conditionString.Contains("<"))
        {
            var halves = conditionString.Split("<");
            var k1 = halves[0].Trim();
            var k2 = halves[1].Trim();
            return (e) =>
            {
                var v1 = e.Fields.ContainsKey(k1)
                    ? (int) e.Fields[k1]
                    : int.Parse(k1);

                var v2 = e.Fields.ContainsKey(k2)
                    ? (int) e.Fields[k2]
                    : int.Parse(k2);

                return v1 < v2;
            };
        }

        // return e => false;
        throw new Exception($"{conditionString} is invalid condition");
    }
}
