namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Extensions;
    using MySql.Data.EntityFramework;

    /// <summary>
    /// 
    /// </summary>
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public partial class ScrapingContext : DbContext
    {
        public ScrapingContext(string connectionString) : base(connectionString)
        {
            DbInterception.Add(new FtsInterceptor());
        }

        public virtual DbSet<NewBukken> NewBukkens { get; set; }

        public virtual DbSet<Bukken> Bukkens { get; set; }

        public virtual DbSet<File> Files { get; set; }

        public virtual DbSet<BukkenFile> BukkenFiles { get; set; }
    }

    public class FtsInterceptor : IDbCommandInterceptor
    {
        private const string FullTextPrefix = "-FTSPREFIX-";

        public static string Fts(string search)
        {
            return string.Format("({0}{1})", FullTextPrefix, search);
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            RewriteFullTextQuery(command);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            RewriteFullTextQuery(command);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        public static void RewriteFullTextQuery(DbCommand cmd)
        {
            var text = cmd.CommandText;
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                var parameter = cmd.Parameters[i];
                var dbTypes = new DbType[] { DbType.String, DbType.AnsiString, DbType.StringFixedLength, DbType.AnsiStringFixedLength };

                if (dbTypes.Contains(parameter.DbType))
                {
                    if (parameter.Value == DBNull.Value)
                        continue;
                    var value = (string)parameter.Value;
                    if (value.IndexOf(FullTextPrefix) >= 0)
                    {
                        parameter.Size = 4096;
                        parameter.DbType = DbType.AnsiStringFixedLength;
                        value = value.Replace(FullTextPrefix, ""); // remove prefix we added n linq query
                        value = value.Substring(2, value.Length - 4); // remove %% escaping by linq translator from string.Contains to sql LIKE
                        //value = value.Substring(1, value.Length - 2); // remove %% escaping by linq translator from string.Contains to sql LIKE
                        parameter.Value = string.Join(" ", value.ToNGram(2).Select(m => "+" + m));

                        var pattern = string.Format(@"`(\w*)`.`(\w*)`\s*LIKE\s*@{0}", parameter.ParameterName);
                        var replacement = string.Format(@"MATCH(`$1`.`$2`) AGAINST(@{0} IN BOOLEAN MODE)", parameter.ParameterName);
                        cmd.CommandText = Regex.Replace(text, pattern, replacement);

                        if (text == cmd.CommandText)
                        {
                            throw new Exception("FTS was not replaced on: " + text);
                        }

                        text = cmd.CommandText;
                    }
                }
            }
        }
    }
}
