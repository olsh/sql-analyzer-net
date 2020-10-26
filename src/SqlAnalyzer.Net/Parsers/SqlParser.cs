using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SqlAnalyzer.Net.Parsers
{
    internal static class SqlParser
    {
        private static readonly Regex SqlDeclareRegex = new Regex(
            @"declare\s(?<declaration>.*?)\s(select|update|delete|insert|merge|if|begin|set|;)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex SqlParameterRegex = new Regex(@"(?<!@)@(?<variable>\w+)", RegexOptions.Compiled);

        private static readonly Regex SqlLiteralRegex = new Regex(@"{=(?<variable>\w+)}", RegexOptions.Compiled);

        private static readonly Regex SqlParameterAssignmentRegex = new Regex(@"@(?<declaration>\w+)\s*=\s*@", RegexOptions.Compiled);

        private static readonly Regex SingleLineComment = new Regex(@"--.*$", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex MultiLineComment = new Regex(@"/\*.*?\*/", RegexOptions.Compiled | RegexOptions.Singleline);

        public static ICollection<string> FindParameters(string sql)
        {
            sql = MultiLineComment.Replace(sql, string.Empty);
            sql = SingleLineComment.Replace(sql, string.Empty);

            var sqlVariables = new HashSet<string>();
            var declaredVariables = new HashSet<string>();
            foreach (Match match in SqlDeclareRegex.Matches(sql))
            {
                foreach (Match declaration in SqlParameterRegex.Matches(
                    match.Groups["declaration"]
                        .Value))
                {
                    declaredVariables.Add(declaration.Groups["variable"].Value);
                }
            }

            foreach (Match match in SqlParameterAssignmentRegex.Matches(sql))
            {
                declaredVariables.Add(match.Groups["declaration"].Value);
            }

            var matches = SqlParameterRegex.Matches(sql);
            foreach (Match match in matches)
            {
                sqlVariables.Add(match.Groups["variable"].Value);
            }

            sqlVariables.ExceptWith(declaredVariables);

            return sqlVariables;
        }

        public static ICollection<string> FindDapperLiterals(string sql)
        {
            var sqlLiterals = new HashSet<string>();
            foreach (Match match in SqlLiteralRegex.Matches(sql))
            {
                sqlLiterals.Add(match.Groups["variable"].Value);
            }

            return sqlLiterals;
        }
    }
}
