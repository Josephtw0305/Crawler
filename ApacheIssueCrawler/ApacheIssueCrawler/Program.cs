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

            #region Details
            HtmlNodeCollection title = null;
            HtmlNodeCollection content = null;

            // Type.
            title = doc.DocumentNode.SelectNodes($"//*[@id='issuedetails']/li[1]/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='type-val']/text()");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            // Status.
            title = doc.DocumentNode.SelectNodes($"//*[@id='issuedetails']/li[2]/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='status-val']/span");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            // Priority.
            title = doc.DocumentNode.SelectNodes($"//*[@id='issuedetails']/li[3]/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='priority-val']/text()");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            // Resolution.
            title = doc.DocumentNode.SelectNodes($"//*[@id='issuedetails']/li[4]/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='resolution-val']/text()");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            // Affects Version.
            title = doc.DocumentNode.SelectNodes($"//*[@id='issuedetails']/li[5]/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='versions-field']/span");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            // Fix Version.
            title = doc.DocumentNode.SelectNodes($"//*[@id='issuedetails']/li[6]/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='fixVersions-field']");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            // Component.
            title = doc.DocumentNode.SelectNodes($"//*[@id='issuedetails']/li[7]/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='components-field']/a");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            // Labels.
            title = doc.DocumentNode.SelectNodes($"//*[@id='issuedetails']/li[8]/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='labels-13028113-value']");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            // Patch Info.
            title = doc.DocumentNode.SelectNodes($"//*[@id='rowForcustomfield_12310041']/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='customfield_12310041-field']/span");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            // Estimated Complexity.
            title = doc.DocumentNode.SelectNodes($"//*[@id='rowForcustomfield_12310060']/div/strong");
            content = doc.DocumentNode.SelectNodes($"//*[@id='customfield_12310060-val']");
            addValuetoOutput(title, content, ref apacheIssueContent, true, true);

            #endregion

            #region People.

            // Assingee
            title = doc.DocumentNode.SelectNodes($"//*[@id='peopledetails']/li/dl[1]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='issue_summary_assignee_davsclaus']/text()");
            addValuetoOutput(title, content, ref apacheIssueContent, false, true);

            // Reporter
            title = doc.DocumentNode.SelectNodes($"//*[@id='peopledetails']/li/dl[2]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='issue_summary_reporter_bobpaulin']/text()");
            addValuetoOutput(title, content, ref apacheIssueContent, false, true);

            // Votes:
            title = doc.DocumentNode.SelectNodes($"//*[@id='peoplemodule']/div[2]/ul[2]/li/dl[1]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='vote-data']");
            addValuetoOutput(title, content, ref apacheIssueContent, false, true);

            // Watchers:
            title = doc.DocumentNode.SelectNodes($"//*[@id='peoplemodule']/div[2]/ul[2]/li/dl[2]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='watcher-data']");
            addValuetoOutput(title, content, ref apacheIssueContent, false, true);
            #endregion

            #region Dates

            // Create date.
            title = doc.DocumentNode.SelectNodes($"//*[@id='datesmodule']/div[2]/ul/li/dl[1]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='created-val']/time");
            addValuetoOutput(title, content, ref apacheIssueContent, false, true);

            // Update date.
            title = doc.DocumentNode.SelectNodes($"//*[@id='datesmodule']/div[2]/ul/li/dl[2]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='updated-val']/time");
            addValuetoOutput(title, content, ref apacheIssueContent, false, true);

            title = doc.DocumentNode.SelectNodes($"//*[@id='datesmodule']/div[2]/ul/li/dl[3]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='resolutiondate-val']/time");
            addValuetoOutput(title, content, ref apacheIssueContent, false, true);

            #endregion

            #region Description & Comment

            // Description.
            title = doc.DocumentNode.SelectNodes($"//*[@id='descriptionmodule_heading']/h4");
            content = doc.DocumentNode.SelectNodes($"//*[@id='description-val']/div");
            addValuetoOutput(title, content, ref apacheIssueContent, true, false);
s
            #endregion

            #endregion

            #region step2: export to CSV file.

            string directory = @"c:\tmp"; ;
            string fileName = $"result_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
            exportToCSV(directory, fileName, apacheIssueContent);

            #endregion
        }

        private static void addValuetoOutput(HtmlNodeCollection title, HtmlNodeCollection content, ref ApacheIssueContent result, bool isRemoveContentInnerSpace, bool isReplaceInnerComma)
        {
           
            string outputValue = "";
            foreach (var item in title)
            {
                outputValue += item.InnerText.ToString().Replace(System.Environment.NewLine, "").Trim().Replace(":", "");
            }
          
            result.titles.Add(outputValue += ",");

            outputValue = "";
            foreach (var item in content)
            {
                outputValue += item.InnerText.ToString().Replace("\n", "").Trim();
                if (isRemoveContentInnerSpace)
                {
                    outputValue = outputValue.Replace(",", "/");
                }
            }

            if (isRemoveContentInnerSpace)
            {
                outputValue = outputValue.Replace(" ", "");
            }
            result.contents.Add(outputValue += ",");
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

            // Export the contents to target file.
            using (var sm = new StreamWriter(fileFullPath))
            {

 
                string title = "";
                string content = "";

                foreach (var item in data.titles)
                {
                    title += item;
                }


                foreach (var item in data.contents)
                {
                    content += item;
                }

                // remove the last comma of titles and contents.
                title = title.Substring(0, title.Length - 1);
                content = content.Substring(0, content.Length - 1);


                sm.WriteLineAsync(title);
                sm.WriteLineAsync(content);
               
            }
        }
    }
}
