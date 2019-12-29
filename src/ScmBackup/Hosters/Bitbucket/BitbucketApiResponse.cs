using System.Collections.Generic;

namespace ScmBackup.Hosters.Bitbucket
{
    internal class BitbucketApiResponse
    {
        public List<Repo> values { get; set; }
        public string next { get; set; }

        internal class Repo
        {
            public string scm { get; set; }
            public string slug { get; set; }
            public string description { get; set; }
            public string name { get; set; }
            public string language { get; set; }

            /// <summary>
            /// The concatenation of the repository owner's username and the slugified name, e.g. "evzijst/interruptingcow". This is the same string used in Bitbucket URLs.
            /// </summary>
            /// <value>
            /// The full name.
            /// </value>
            public string full_name { get; set; }

            public long size { get; set; }

            public bool has_wiki { get; set; }
            public bool has_issues { get; set; }
            public bool is_private { get; set; }
            public Links links { get; set; }

            internal class Links
            {
                public List<Clone> clone { get; set; }

                internal class Clone
                {
                    public string href { get; set; }
                    public string name { get; set; }
                }
            }
        }
    }
}
