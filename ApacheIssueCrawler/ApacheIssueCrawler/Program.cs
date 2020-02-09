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
            string outputValue = "";

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

            #region Dates

            // Create date.
            // [TODO] get the epoch.
            title = doc.DocumentNode.SelectNodes($"//*[@id='datesmodule']/div[2]/ul/li/dl[1]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='created-val']/time");
            foreach (var item in title)
            {
                outputValue += item.InnerText.ToString().Replace(System.Environment.NewLine, "").Trim().Replace(":","");
            }
            apacheIssueContent.title.Add(outputValue+=",");

            outputValue = "";
            foreach (var item in content)
            {
                outputValue += item.InnerText.ToString().Replace(System.Environment.NewLine, "").Trim();
            }
            apacheIssueContent.content.Add(outputValue += ",");

            // Update date.
            title = doc.DocumentNode.SelectNodes($"//*[@id='datesmodule']/div[2]/ul/li/dl[2]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='updated-val']/time");

            outputValue = "";
            foreach (var item in title)
            {
                outputValue += item.InnerText.ToString().Replace(System.Environment.NewLine, "").Trim().Replace(":", "");
            }
            apacheIssueContent.title.Add(outputValue += ",");

            outputValue = "";
            foreach (var item in content)
            {
                outputValue += item.InnerText.ToString().Replace(System.Environment.NewLine, "").Trim();
            }
            apacheIssueContent.content.Add(outputValue += ",");

            // Resolved date.
            title = doc.DocumentNode.SelectNodes($"//*[@id='datesmodule']/div[2]/ul/li/dl[3]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='resolutiondate-val']/time");

            outputValue = "";
            foreach (var item in title)
            {
                outputValue += item.InnerText.ToString().Replace(System.Environment.NewLine, "").Trim().Replace(":", "");
            }
            apacheIssueContent.title.Add(outputValue += ",");

            outputValue = "";
            foreach (var item in content)
            {
                outputValue += item.InnerText.ToString().Replace(System.Environment.NewLine, "").Trim();
            }
            apacheIssueContent.content.Add(outputValue += ",");

            #endregion

            #region Description & Comment

            // Description.
            title = doc.DocumentNode.SelectNodes($"//*[@id='descriptionmodule_heading']/h4");
            content = doc.DocumentNode.SelectNodes($"//*[@id='description-val']/div");
            addValuetoOutput(title,content, ref apacheIssueContent, false, false);

            // Comment.
            /*
            title = doc.DocumentNode.SelectNodes($"/html/body/div[1]/section/div[2]/div/div/div/div/div[2]/div/div/div/div[1]/div[5]/div[2]/div[1]/ul/li[2]/a/text()");
            content = doc.DocumentNode.SelectNodes($"//*[@id='comment-15748543']/div[1]/div[1]/div[2]/text()");
            */

            // addValuetoOutput(title, content, ref apacheIssueContent);

            #endregion

            

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

        private static void addValuetoOutput(HtmlNodeCollection title, HtmlNodeCollection content, ref ApacheIssueContent result, bool isRemoveContentInnerSpace, bool isReplaceInnerComma)
        {
           
            string outputValue = "";
            foreach (var item in title)
            {
                outputValue += item.InnerText.ToString().Replace(System.Environment.NewLine, "").Trim().Replace(":", "");
            }
          
            result.title.Add(outputValue += ",");

            outputValue = "";
            foreach (var item in content)
            {
                outputValue += item.InnerText.ToString().Replace(System.Environment.NewLine, "").Trim();
                if (isRemoveContentInnerSpace)
                {
                    outputValue = outputValue.Replace(",", "/");
                }
            }

            if (isRemoveContentInnerSpace)
            {
                outputValue = outputValue.Replace(" ", "");
            }
            result.content.Add(outputValue += ",");
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
                string title = data.assingeeTitle + "," + data.reportTitle + "," + data.votersTitle + "," + data.watchersTitle + ",";
                title = title.Replace(":", "");

                foreach (var item in data.title)
                {
                    title += item;
                }

                string content = data.assingeeContent + "," + data.reporterContent + "," + data.numberofVoters + "," + data.numberofWatchers + ",";

                foreach (var item in data.content)
                {
                    content += item;
                }

                sm.WriteLineAsync(title);
                sm.WriteLineAsync(content);
                //sm.Flush();
                //sm.Close();
            }
        }
    }
}
