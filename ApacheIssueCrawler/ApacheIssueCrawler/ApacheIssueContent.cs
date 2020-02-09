using System;
using System.Collections.Generic;
using System.Text;

namespace ApacheIssueCrawler
{
    class ApacheIssueContent
    {
        /// <summary>
        /// Assingee's Title.
        /// </summary>
        public string assingeeTitle { get; set; }

        /// <summary>
        /// Assingee's Content.
        /// </summary>
        public string assingeeContent { get; set; }

        /// <summary>
        /// Reporter's Title.
        /// </summary>
        public string reportTitle { get; set; }

        /// <summary>
        /// Report's Content.
        /// </summary>
        public string reporterContent { get; set; }

        /// <summary>
        /// Voters' Title.
        /// </summary>
        public string votersTitle { get; set; }

        /// <summary>
        /// Voters' Content.
        /// </summary>
        public string numberofVoters { get; set; }

        /// <summary>
        /// Watchers' Title.
        /// </summary>
        public string watchersTitle { get; set; }

        /// <summary>
        /// Watehrs' Content.
        /// </summary>
        public string numberofWatchers { get; set; }


        public List<string> title = new List<string>();

        public List<string> content = new List<string>();
    }
}
