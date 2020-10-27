using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Querying.Conditions;
using Querying.Data;
using Querying.Query;

public class Utils
{
    public static Regex MainQueryRegex = 
        new Regex(@"(?:select) (.*) (?:from) (\w*)(.*)", 
            (RegexOptions.IgnoreCase));
    
    public static Regex WhereRegex =
        new Regex(@"(?:where) (.*?)(?= join| on| orderby| where|;)",
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
    
    public static Regex LikeRegex =
        new Regex(@"(.*) (?:like) (.*)",
            (RegexOptions.IgnoreCase));
    
    public static void PrintEntries(IEnumerable<Entry> entries, string tableName = "")
    {
        Console.WriteLine(tableName);
        Console.WriteLine("========================");
        
        //TODO
        
    }

    public static IOperation BuildOperation(string queryString, Database database)
    {
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

    //ORGANIZAR E ARRUMAR
    public static IOperation BuildTree(Access mainAccess, string operationQuery, Database database)
    {
        // IOperation lastOperation = mainAccess;
        //
        // Where where = null;
        //
        // var whereMatch = WhereRegex.Match(operationQuery);
        // if (whereMatch.Success)
        // {
        //     where = new Where(
        //         BuildConditionTree(whereMatch.Groups[1].Value.Trim()),
        //         null
        //     );
        // }
        //
        // if (where != null &&
        //     (!where.Condition.InvolvedTables.Any() || where.Condition.InvolvedTables.Contains(mainAccess.TableName)))
        // {
        //     where.CollectionOperation = lastOperation;
        //     lastOperation = where;
        //     where = null;
        // }
        //
        // //SELECT * FROM USER JOIN CONTAS ON CONTAS.USERID = USER.ID WHERE USER.POINTS >= 30
        //
        // var joinMatch = JoinRegex.Match(operationQuery);
        // if (joinMatch.Success) //ver pra substituir por while .Sucess e botar um .NextMatch() no final
        // {
        //     var groups = joinMatch.Groups.Values.Select(g => g.Value).ToArray();
        //     var tableName = joinMatch.Groups[1].Value.Trim();
        //     IOperation access = new Access(tableName, database);
        //
        //     if (where != null
        //         && where.Condition.InvolvedTables.Length == 1
        //         && where.Condition.InvolvedTables[0].Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
        //     {
        //         where.CollectionOperation = access;
        //         access = where;
        //         where = null;
        //     }
        //
        //     lastOperation = new Join
        //     (
        //         BuildConditionTree(joinMatch.Groups[2].Value.Trim()),
        //         lastOperation,
        //         access
        //     );
        // }
        //
        // if (where != null)
        // {
        //     where.CollectionOperation = lastOperation;
        //     lastOperation = where;
        // }
        //
        // var orderByMatch = OrderByRegex.Match(operationQuery);
        // if (orderByMatch.Success)
        // {
        //     lastOperation = new Order(orderByMatch.Groups[1].Value.Trim(), lastOperation);
        // }
        //
        // return lastOperation;

        IOperation lastOperation = mainAccess;

        var wheres = new List<Where>();
        var whereMatches = WhereRegex.Matches(operationQuery);
        var involvedTables = new List<string>{mainAccess.TableName};

        foreach (Match match in whereMatches)
        {
            if (!match.Success) continue;
            
            var where = new Where(
                BuildConditionTree(match.Groups[1].Value.Trim()),
                null);
                
            if (@where.Condition.InvolvedTables.Length == 0)
                throw new Exception($"Constant condition: {@where.Condition.ConditionDescription}");

            if (@where.Condition.InvolvedTables.Length == 1 &
                @where.Condition.InvolvedTables[1]
                    .Equals(mainAccess.TableName, StringComparison.InvariantCultureIgnoreCase))
            {
                @where.CollectionOperation = lastOperation;
                lastOperation = @where;
            }
            else
            {
                wheres.Add(@where);
            }
        }

        var joinMatches = JoinRegex.Matches(operationQuery);
        
        foreach (Match match in joinMatches)
        {
            var tableName = match.Groups[1].Value.Trim();
            IOperation access = new Access(tableName, database);

            var joinWheres =
                wheres.Where(w => w.Condition.InvolvedTables.Length == 1
                                  && w.Condition.InvolvedTables[1]
                                      .Equals(tableName,
                                          StringComparison.InvariantCultureIgnoreCase))
                        .ToArray();

            if (joinWheres.Length > 0)
            {
                wheres = wheres.Except(joinWheres).ToList();
                foreach (var joinWhere in joinWheres)
                {
                    joinWhere.CollectionOperation = access;
                    access = joinWhere;
                }
            }
            
            lastOperation = new Join
            (
                BuildConditionTree(match.Groups[2].Value.Trim()),
                lastOperation,
                access
            );

            involvedTables.Add(tableName);

            var postJoinWheres =
                wheres.Where(w =>
                {
                    var tables = w.Condition.InvolvedTables.Select(t => t.ToLower());
                    var involved = involvedTables.Select(t => t.ToLower());
                    return !tables.Except(involved).Any();
                })
                    .ToArray();

            if (postJoinWheres.Length > 0)
            {
                wheres = wheres.Except(postJoinWheres).ToList();
                foreach (var where in postJoinWheres)
                {
                    where.CollectionOperation = lastOperation;
                    lastOperation = where;
                }
            }
        }

        if (wheres.Any())
        {
            throw new Exception($"Invalid where: {wheres.First().Condition.ConditionDescription}");
        }

        var orderByMatches = OrderByRegex.Matches(operationQuery);
        foreach (Match orderByMatch in orderByMatches)
        {
            if (orderByMatch.Success)
            {
                lastOperation = new Order(
                    orderByMatch.Groups[1].Value.Trim(), lastOperation);
            }
        }

        return lastOperation;
    }

    public static ICondition BuildConditionTree(string conditionString)
    {
        var andMatch = AndRegex.Match(conditionString);
        if (andMatch.Success)
        {
            var c1 = BuildConditionTree(andMatch.Groups[1].Value);
            var c2 = BuildConditionTree(andMatch.Groups[2].Value);
            return new AndCondition(c1, c2);
        }
        
        var orMatch = OrRegex.Match(conditionString);
        if (orMatch.Success)
        {
            var c1 = BuildConditionTree(orMatch.Groups[1].Value);
            var c2 = BuildConditionTree(orMatch.Groups[2].Value);
            return new OrCondition(c1, c2);
        }
        
        var ltoetMatch = LessThanOrEqualToRegex.Match(conditionString);
        if (ltoetMatch.Success)
        {
            var k1 = GetVariableFromTerm(ltoetMatch.Groups[1].Value.Trim());
            var k2 = GetVariableFromTerm(ltoetMatch.Groups[2].Value.Trim());
            return new MathCondition(k1,k2,MathCondition.MathOperation.LessThanOrEqualTo);
        }

        var mtoetMatch = MoreThanOrEqualToRegex.Match(conditionString);
        if (mtoetMatch.Success)
        {
            var k1 = GetVariableFromTerm(mtoetMatch.Groups[1].Value.Trim());
            var k2 = GetVariableFromTerm(mtoetMatch.Groups[2].Value.Trim());
            return new MathCondition(k1,k2,MathCondition.MathOperation.GreaterThanOrEqualTo);
        }

        var unequalMatch = UnequalToRegex.Match(conditionString);
        if (unequalMatch.Success)
        {
            var k1 = GetVariableFromTerm(unequalMatch.Groups[1].Value.Trim());
            var k2 = GetVariableFromTerm(unequalMatch.Groups[2].Value.Trim());
            return new EqualityCondition(k1,k2,false);
        }

        var equalToMatch = EqualToRegex.Match(conditionString);
        if (equalToMatch.Success)
        {
            var k1 = GetVariableFromTerm(equalToMatch.Groups[1].Value.Trim());
            var k2 = GetVariableFromTerm(equalToMatch.Groups[2].Value.Trim());
            return new EqualityCondition(k1,k2,true);
        }

        var lessMatch = LessThanRegex.Match(conditionString);
        if (lessMatch.Success)
        {
            var k1 = GetVariableFromTerm(lessMatch.Groups[1].Value.Trim());
            var k2 = GetVariableFromTerm(lessMatch.Groups[2].Value.Trim());
            return new MathCondition(k1,k2,MathCondition.MathOperation.LessThan);
        }

        var moreMatch = MoreThanRegex.Match(conditionString);
        if (moreMatch.Success)
        {
            var k1 = GetVariableFromTerm(moreMatch.Groups[1].Value.Trim());
            var k2 = GetVariableFromTerm(moreMatch.Groups[2].Value.Trim());
            return new MathCondition(k1,k2,MathCondition.MathOperation.GreaterThan);
        }

        var notInMatch = NotInRegex.Match(conditionString);
        if (notInMatch.Success)
        {
            var k1 = GetVariableFromTerm(notInMatch.Groups[1].Value.Trim());
            var k2 = notInMatch.Groups[2].Value
                .Split(",")
                .Select(s => GetVariableFromTerm(s.Trim()))
                .ToArray();
            return new InCondition(k1, k2, false);
        }
        
        var inMatch = InRegex.Match(conditionString);
        if (inMatch.Success)
        {
            var k1 = GetVariableFromTerm(notInMatch.Groups[1].Value.Trim());
            var k2 = notInMatch.Groups[2].Value
                .Split(",")
                .Select(s => GetVariableFromTerm(s.Trim()))
                .ToArray();
            return new InCondition(k1, k2, true);
        }

        var likeMatch = LikeRegex.Match(conditionString);
        if (likeMatch.Success)
        {
            var v1 = GetVariableFromTerm(likeMatch.Groups[1].Value.Trim());
            var v2 = GetVariableFromTerm(likeMatch.Groups[2].Value.Trim());
            return new LikeCondition(v1,v2);
        }
        
        // return e => false;
        throw new Exception($"{conditionString} is invalid condition");
    }

    public static Variable GetVariableFromTerm(string term)
    {
        if ((term[0] == '"' && term[^1] == '"')
            || (term[0] == '\'' && term[^1] == '\''))
        {
            var variable = new string(term.Skip(1).SkipLast(1).ToArray());
            return new Variable(typeof(string), variable);
        }

        if (int.TryParse(term, out var number))
        {
            return new Variable(typeof(int), number);
        }

        if (DateTime.TryParse(term, out var date))
        {
            return new Variable(typeof(DateTime), date);
        }
        
        var identifier = new FieldIdentifier(term);
        return new Variable(typeof(FieldIdentifier), identifier);
    }
}
