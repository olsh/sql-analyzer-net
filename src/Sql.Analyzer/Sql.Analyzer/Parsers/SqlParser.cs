using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sql.Analyzer.Parsers
{
    internal static class SqlParser
    {
        private static readonly Regex SqlParameterRegex = new Regex(@"(?<!@)@(?<variable>\w+)", RegexOptions.Compiled);
        private static readonly Regex SqlDeclareRegex = new Regex(@"declare\s(?<declaration>.*?)(select|update|delete|insert|merge)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static ICollection<string> FindParameters(string sql)
        {
            var sqlVariables = new HashSet<string>();
            var declaredVariables = new HashSet<string>();
            foreach (Match match in SqlDeclareRegex.Matches(sql))
            {
                foreach (Match declaration in SqlParameterRegex.Matches(match.Groups["declaration"].Value))
                {
                    declaredVariables.Add(declaration.Groups["variable"].Value);
                }
            }

            var matches = SqlParameterRegex.Matches(sql);
            foreach (Match match in matches)
            {
                sqlVariables.Add(match.Groups["variable"].Value);
            }

            sqlVariables.ExceptWith(declaredVariables);

            return sqlVariables;
        }
    }
}
