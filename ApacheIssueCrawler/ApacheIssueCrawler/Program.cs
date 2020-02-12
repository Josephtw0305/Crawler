using HtmlAgilityPack;
using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace ApacheIssueCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Let's begin to crawl!");
            
            int startNumber = 0;
            int maxNumber = 0;

            while (true)
            {
                ApacheIssueContent apacheIssueContent = new ApacheIssueContent();
                while (startNumber == 0)
                {
                    Console.Write("Please enter the start number of issue (-9999 to end):");
                    var inputNumber = Console.ReadLine();
                    Int32.TryParse(inputNumber, out startNumber);

                    if (startNumber == -9999)
                    {
                        Environment.Exit(-1);
                    }

                    if (startNumber == 0)
                    {
                        Console.WriteLine("Please input number, thanks!");
                    }
                }

                while (maxNumber == 0 || (maxNumber < startNumber))
                {
                    Console.Write("Please enter the end number of issue (-9999 to end):");
                    var inputNumber = Console.ReadLine();
                    Int32.TryParse(inputNumber, out maxNumber);

                    if (maxNumber == -9999)
                    {
                        Environment.Exit(-1);
                    }

                    if (maxNumber == 0)
                    {
                        Console.WriteLine("Please input number, thanks!");
                        continue;
                    }

                    if (maxNumber < startNumber)
                    {
                        Console.WriteLine("MaxNumber need to be smaller than startNumber, please input again!");
                    }
                }

                string fileName = $"result_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
               

                // Set titles.
                setTitleforOutput(ref apacheIssueContent);

                for (int issueNo = startNumber; issueNo <= maxNumber; issueNo++)
                {
                    bool isExportCSV = (issueNo == maxNumber) ? true : false;
                    processIssue(issueNo, fileName, isExportCSV, ref apacheIssueContent);
                }

                Console.WriteLine($"Process issues from {startNumber} to {maxNumber} completed, fileName:{fileName}");

                // Reset control parameters.
                apacheIssueContent.titles.Clear();
                apacheIssueContent.contents.Clear();
                startNumber = 0;
                maxNumber = 0;
            }
          
        }

        private static void processIssue(int issueNo, string exportFileName, bool isExportCSV, ref ApacheIssueContent apacheIssueContent)
        {
            HtmlWeb webClient = new HtmlWeb();
            

            // Set the target url that we desire to crawl.
            string urlforOtherFields = $"https://issues.apache.org/jira/browse/CAMEL-{issueNo}";
            string urlforComment = $"https://issues.apache.org/jira/si/jira.issueviews:issue-xml/CAMEL-{issueNo}/CAMEL-{issueNo}.xml";

            // Add issue no to output.
            apacheIssueContent.contents.Add($"{issueNo};");

            #region Step 01:get the raw data from the URL.

            HtmlDocument doc = webClient.Load(urlforOtherFields);

            #region Details
            HtmlNodeCollection title = null;
            HtmlNodeCollection content = null;

            // Type.
            content = doc.DocumentNode.SelectNodes($"//*[@id='type-val']/text()");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            // Status.
            content = doc.DocumentNode.SelectNodes($"//*[@id='status-val']/span");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            // Priority. 
            content = doc.DocumentNode.SelectNodes($"//*[@id='priority-val']/text()");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            // Resolution.
            content = doc.DocumentNode.SelectNodes($"//*[@id='resolution-val']/text()");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            // Affects Version.
            content = doc.DocumentNode.SelectNodes($"//*[@id='versions-field']/span");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            // Fix Version.
            content = doc.DocumentNode.SelectNodes($"//*[@id='fixVersions-field']");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            // Component.
            content = doc.DocumentNode.SelectNodes($"//*[@id='components-field']/a");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            // Labels.
            content = doc.DocumentNode.SelectNodes($"//*[@id='labels-13028113-value']");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            // Patch Info.   
            content = doc.DocumentNode.SelectNodes($"//*[@id='customfield_12310041-field']/span");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            // Estimated Complexity.
            content = doc.DocumentNode.SelectNodes($"//*[@id='customfield_12310060-val']");
            addValuetoOutput(content, ref apacheIssueContent, true, true);

            #endregion

            #region People.

            // Assingee
            content = doc.DocumentNode.SelectNodes($"//*[@id='issue_summary_assignee_davsclaus']/text()");
            addValuetoOutput(content, ref apacheIssueContent, false, true);

            // Reporter
            content = doc.DocumentNode.SelectNodes($"//*[@id='issue_summary_reporter_bobpaulin']/text()");
            addValuetoOutput(content, ref apacheIssueContent, false, true);

            // Votes:
            content = doc.DocumentNode.SelectNodes($"//*[@id='vote-data']");
            addValuetoOutput(content, ref apacheIssueContent, false, true);

            // Watchers:
            content = doc.DocumentNode.SelectNodes($"//*[@id='watcher-data']");
            addValuetoOutput(content, ref apacheIssueContent, false, true);
            #endregion

            #region Dates

            // Create date. 
            content = doc.DocumentNode.SelectNodes($"//*[@id='created-val']/time");
            addValuetoOutput(content, ref apacheIssueContent, false, true);

            // Update date.
            content = doc.DocumentNode.SelectNodes($"//*[@id='updated-val']/time");
            addValuetoOutput(content, ref apacheIssueContent, false, true);

            //title = doc.DocumentNode.SelectNodes($"//*[@id='datesmodule']/div[2]/ul/li/dl[3]/dt");
            content = doc.DocumentNode.SelectNodes($"//*[@id='resolutiondate-val']/time");
            addValuetoOutput(content, ref apacheIssueContent, false, true);

            #endregion

            #region Description

            // Description.
            content = doc.DocumentNode.SelectNodes($"//*[@id='description-val']/div");
            addValuetoOutput(content, ref apacheIssueContent, false, true);

            #endregion

            #region Comment

            // Retrieve the content of comment field.

            XmlTextReader reader = new XmlTextReader(urlforComment);
            string outComment = "";
            bool isStartOutComment = false;
            try
            {

                while (reader.Read())
                {

                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // The node is an element.

                            if (string.Equals(reader.Name, "comment"))
                            {
                                isStartOutComment = true;
                            }

                            if (isStartOutComment)
                            {
                                var author = reader.GetAttribute("author");
                                var createTime = reader.GetAttribute("createTime");
                                outComment += $"{author} added a comment - {createTime}:";
                            }

                            //Console.Write("<" + reader.Name);
                            //Console.WriteLine(">");
                            break;

                        case XmlNodeType.Text: //Display the text in each element.

                            //Console.WriteLine(reader.Value);
                            if (isStartOutComment)
                            {
                                outComment += $"{reader.Value}";
                            }

                            break;

                        case XmlNodeType.EndElement: //Display the end of the element.
                            //Console.Write("</" + reader.Name);
                            //Console.WriteLine(">");
                            if (string.Equals(reader.Name, "comment"))
                            {
                                isStartOutComment = false;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                outComment = "Privacy issue: This issue need to login first.";

            }
            finally
            {
                if (string.IsNullOrEmpty(outComment))
                {
                    outComment = "No comment";
                }
                apacheIssueContent.contents.Add($"{outComment.Replace("\n", "").Replace(";", ".")}\n");
                Console.WriteLine($"Issue:{issueNo} process completed!");
            }

            #endregion

            #endregion

            #region Step 02: export to CSV file.

            if (isExportCSV)
            {
                string directory = @"c:\tmp"; ;
                exportToCSV(directory, exportFileName, apacheIssueContent, isExportCSV);
            }
           
            #endregion

        }

        /// <summary>
        /// Set the titles.
        /// </summary>
        /// <param name="apacheIssueContent"></param>
        private static void setTitleforOutput(ref ApacheIssueContent apacheIssueContent)
        {
            apacheIssueContent.titles.Add($"Issue No;");
            apacheIssueContent.titles.Add($"Type;");
            apacheIssueContent.titles.Add($"Status;");
            apacheIssueContent.titles.Add($"Priority;");
            apacheIssueContent.titles.Add($"Resolution;");
            apacheIssueContent.titles.Add($"Affects Version/s;");
            apacheIssueContent.titles.Add($"Fix Version/ s;");
            apacheIssueContent.titles.Add($"Component;");
            apacheIssueContent.titles.Add($"Labels;");
            apacheIssueContent.titles.Add($"Patch Info;");
            apacheIssueContent.titles.Add($"Estimated Complexity;");
            apacheIssueContent.titles.Add($"Assignee;");
            apacheIssueContent.titles.Add($"Reporter;");
            apacheIssueContent.titles.Add($"Votes;");
            apacheIssueContent.titles.Add($"Watchers;");
            apacheIssueContent.titles.Add($"Created;");
            apacheIssueContent.titles.Add($"Updated;");
            apacheIssueContent.titles.Add($"Resolved;");
            apacheIssueContent.titles.Add($"Description;");
            apacheIssueContent.titles.Add($"Comment;");
        }

        private static void addValuetoOutput(HtmlNodeCollection content, ref ApacheIssueContent result, bool isRemoveContentInnerSpace, bool isReplaceInnerSemicolon)
        {

            string outputValue = "";

            if (content != null)
            {
                foreach (var item in content)
                {
                    outputValue += item.InnerText.ToString().Replace("\n", "").Trim();
                    if (isReplaceInnerSemicolon)
                    {
                        outputValue = outputValue.Replace(";", " ");
                    }
                }
                if (isRemoveContentInnerSpace)
                {
                    outputValue = outputValue.Replace(" ", "");
                }
            }
            else
            {
                outputValue = "None";
            }
            result.contents.Add(outputValue += ";");
        }

        /// <summary>
        /// Export conetnt to CSV file.
        /// </summary>
        /// <param name="directory">target directory.</param>
        /// <param name="fileName">target file name.</param>
        /// <param name="data">output data.</param>
        static void exportToCSV(string directory, string fileName, ApacheIssueContent data, bool isExportTitle)
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

                if (isExportTitle)
                {
                    foreach (var item in data.titles)
                    {
                        title += item;
                    }
                }

                foreach (var item in data.contents)
                {
                    content += item;
                }

                // remove the last comma of titles and contents.
                title = title.Substring(0, title.Length - 1);
                content = content.Substring(0, content.Length - 1);

                sm.WriteLine(title);
                sm.WriteLine(content);
                sm.Close();               
            }
        }
    }
}
