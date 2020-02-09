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


        public string reportTitle { get; set; }
        public string reporterContent { get; set; }

        public string votersTitle { get; set; }
        public string numberofVoters { get; set; }

        public string watchersTitle { get; set; }
        public string numberofWatchers { get; set; }
    }
}
