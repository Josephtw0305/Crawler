using HtmlAgilityPack;
using System;
using System.IO;

namespace ApacheIssueCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Let's begin to crawl!");
            HtmlWeb webClient = new HtmlWeb();
            ApacheIssueContent apacheIssueContent = new ApacheIssueContent();

            // Set the target url that we desire to crawl.
            HtmlDocument doc = webClient.Load("https://issues.apache.org/jira/browse/CAMEL-10597");

            #region Step 01:get the raw data from the URL.

            // People. 

            // Assingee
            HtmlNodeCollection assgineeTitle = doc.DocumentNode.SelectNodes($"//*[@id='peopledetails']/li/dl[1]/dt");
            HtmlNodeCollection assingeeContent = doc.DocumentNode.SelectNodes($"//*[@id='issue_summary_assignee_davsclaus']/text()");

            // Reporter
            HtmlNodeCollection reporterTitle = doc.DocumentNode.SelectNodes($"//*[@id='peopledetails']/li/dl[2]/dt");
            HtmlNodeCollection reporterContent = doc.DocumentNode.SelectNodes($"//*[@id='issue_summary_reporter_bobpaulin']/text()");

            // Votes:
            HtmlNodeCollection votersTitle = doc.DocumentNode.SelectNodes($"//*[@id='peoplemodule']/div[2]/ul[2]/li/dl[1]/dt");
            HtmlNodeCollection votersContent = doc.DocumentNode.SelectNodes($"//*[@id='vote-data']");

            // Watchers:
            HtmlNodeCollection watchersTitle = doc.DocumentNode.SelectNodes($"//*[@id='peoplemodule']/div[2]/ul[2]/li/dl[2]/dt");
            HtmlNodeCollection watchersContent = doc.DocumentNode.SelectNodes($"//*[@id='watcher-data']");


            #endregion

            #region Step 02: process the data.

            // Check the content of nodecollection.
            try
            {
                // People/Assingee.
                foreach (var item in assgineeTitle)
                {
                    apacheIssueContent.assingeeTitle += item.InnerText.ToString();
                }

                foreach (var item in assingeeContent)
                {
                    apacheIssueContent.assingeeContent += item.InnerText.ToString();
                }

                foreach (var item in reporterTitle)
                {
                    apacheIssueContent.reportTitle += item.InnerText.ToString();
                }

                foreach (var item in reporterContent)
                {
                    apacheIssueContent.reporterContent += item.InnerText.ToString();
                }

                foreach (var item in votersTitle)
                {
                    apacheIssueContent.votersTitle += item.InnerText.ToString();
                }

                foreach (var item in votersContent)
                {
                    apacheIssueContent.numberofVoters += item.InnerText.ToString();
                }

                foreach (var item in watchersTitle)
                {
                    apacheIssueContent.watchersTitle += item.InnerText.ToString();

                }

                foreach (var item in watchersContent)
                {
                    apacheIssueContent.numberofWatchers += item.InnerText.ToString();

                }

                apacheIssueContent.assingeeTitle = apacheIssueContent.assingeeTitle.Replace(System.Environment.NewLine, "").Trim();
                apacheIssueContent.reportTitle = apacheIssueContent.reportTitle.Replace(System.Environment.NewLine, "").Trim();
                apacheIssueContent.votersTitle = apacheIssueContent.votersTitle.Replace(System.Environment.NewLine, "").Trim();
                apacheIssueContent.watchersTitle = apacheIssueContent.watchersTitle.Replace(System.Environment.NewLine, "").Trim();

                apacheIssueContent.assingeeContent = apacheIssueContent.assingeeContent.Replace(System.Environment.NewLine, "").Trim();
                apacheIssueContent.reporterContent = apacheIssueContent.reporterContent.Replace(System.Environment.NewLine, "").Trim();
                apacheIssueContent.numberofVoters = apacheIssueContent.numberofVoters.Replace(System.Environment.NewLine, "").Trim();
                apacheIssueContent.numberofWatchers = apacheIssueContent.numberofWatchers.Replace(System.Environment.NewLine, "").Trim();
                
            }
            catch (Exception ex)
            {

                throw ex;
            }

            // [TODO: export to CSV file.]
            #endregion

            #region export to CSV file.

            string directory = @"c:\tmp"; ;
            string fileName = $"result_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
            exportToCSV(directory, fileName, apacheIssueContent);

            #endregion
        }

        /// <summary>
        /// Export conetnt to CSV file.
        /// </summary>
        /// <param name="directory">target directory.</param>
        /// <param name="fileName">target file name.</param>
        /// <param name="data">output data.</param>
        static void exportToCSV(string directory, string fileName, ApacheIssueContent data)
        {
            // Checking whether the directory exist or not.
            string directoryName = directory;
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            // Checking whether the fileName exist or not.
            string fileFullPath = directory + @"\" + fileName;
            if (File.Exists(fileFullPath))
            {
                File.Create(fileFullPath);
            }

            // [TODO] Replace of the title.
            // Export the content to target file.
            using (var sm = new StreamWriter(fileFullPath))
            {
                string title = data.assingeeTitle + "," + data.reportTitle + "," + data.votersTitle + "," + data.watchersTitle;
                title = title.Replace(":", "");
                string content = data.assingeeContent + "," + data.reporterContent + "," + data.numberofVoters + "," + data.numberofWatchers;
                sm.WriteLineAsync(title);
                sm.WriteLineAsync(content);
                sm.Flush();
                sm.Close();
            }
        }
    }
}
