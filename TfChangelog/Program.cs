
namespace TfChangelog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;

    class Program
    {
        static int Main(string[] args)
        {
            // parse args

            if (args.Length < 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("TfChangelog /collection:TeamProjectCollectionUrl /path:$/ProjectName/Branch/ [/min:min] [/max:max] [/format:CSV]");
                Console.WriteLine("    /collection: Required. The URL the the TFS collection");
                Console.WriteLine("    /path:       Required. The source control path starting with $/");
                Console.WriteLine("    /min:        Optional. Minimum desired changeset ID");
                Console.WriteLine("    /max:        Optional. Maximum desired changeset ID");
                Console.WriteLine("    /format:     Optional. Not implemented yet");
                Console.WriteLine();
                return 1;
            }

            bool unknown = false;
            var config = new Config();
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var upper = arg.ToUpperInvariant();
                string prefix = null;
                string value = null;
                if (arg.Length > 2 && arg[0] == '/' && arg.IndexOf(':') > 0)
                {
                    prefix = arg.Substring(1, upper.IndexOf(':') - 1);
                    value = arg.Substring(1 + upper.IndexOf(':'));
                }
                else
                {
                    Console.WriteLine("invalid argument '" + arg + "'");
                    unknown = true;
                    continue;
                }

                switch (prefix.ToUpperInvariant())
                {
                    case "PATH":
                        config.SourcePath = value;
                        break;

                    case "COLLECTION":
                        Uri uri;
                        if (Uri.TryCreate(value, UriKind.Absolute, out uri))
                        {
                            config.CollectionUrl = uri;
                        }
                        else
                        {
                            unknown = true;
                            Console.WriteLine("invalid uri '" + value + "'");
                        }
                        break;

                    case "MIN":
                        int val;
                        if (int.TryParse(value, out val))
                        {
                            config.MinChangeset = val;
                        }
                        else
                        {
                            Console.WriteLine("cannot parse integer '" + value + "'");
                            unknown = true;
                        }
                        break;

                    case "MAX":
                        if (int.TryParse(value, out val))
                        {
                            config.MaxChangeset = val;
                        }
                        else
                        {
                            Console.WriteLine("cannot parse integer '" + value + "'");
                            unknown = true;
                        }
                        break;

                    default:
                        Console.WriteLine("unknown command '" + prefix + "'");
                        unknown = true;
                        break;
                }
            }

            if (string.IsNullOrEmpty(config.SourcePath))
            {
                Console.WriteLine("Missing /path. Please specify the source control path.");
                Console.WriteLine("For exemple: $/Project/");
                unknown = true;
            }

            if (config.CollectionUrl == null)
            {
                Console.WriteLine("Missing /collection. Please specify collection URL.");
                Console.WriteLine("For exemple: http://tfs.company.com/tfs/CollectionName/");
                unknown = true;
            }

            if (unknown)
            {
                return 2;
            }

            // connect

            try
            {
                var connection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(config.CollectionUrl);
                connection.EnsureAuthenticated();

                // get source control service

                var vcs = connection.GetService<VersionControlServer>();

                // get changesets

                VersionSpec range;
                var versionFrom = config.MinChangeset > 0 ? VersionSpec.ParseSingleSpec("C" + config.MinChangeset, "") : null;
                var versionTo = config.MaxChangeset > 0 ? VersionSpec.ParseSingleSpec("C" + config.MaxChangeset, "") : null;
                range = VersionSpec.Latest;

                var histories = vcs.QueryHistory(config.SourcePath, range, 0, RecursionType.Full, null, versionFrom, versionTo, int.MaxValue, false, false, false);

                var listChanges = new List<Changeset>();
                listChanges.AddRange(histories.Cast<Changeset>());

                // display
                string format = @"C{0} by {1} on {2}

{3}

----
";
                Console.WriteLine(Environment.NewLine + "----" + Environment.NewLine);
                foreach (var item in listChanges)
                {
                    Console.WriteLine(string.Format(format, item.ChangesetId, item.OwnerDisplayName, item.CreationDate, item.Comment));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured");
                Console.WriteLine();
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                return 3;
            }


            return 0;
        }
    }

    class Config
    {
        public string Server { get; set; }

        public Uri CollectionUrl { get; set; }
        public string Project { get; set; }
        public string SourcePath { get; set; }

        public int MinChangeset { get; set; }

        public int MaxChangeset { get; set; }
    }
}
