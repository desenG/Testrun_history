using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Xml;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Globalization;
using System.Data;
using System.IO;
using System.Threading;
using System.Data.Objects;
namespace TestRun_History_Importer
{
    class Program
    {
        static void Main(string[] args)
        {

            List<string> trNums = new List<string>();

            List<TestRun> testrunList = new List<TestRun>();
            List<Step> steps = new List<Step>();
            TestRun testrun;
            XmlDocument doc = new XmlDocument();


            #region[import IDEA xml and txt]
            string configLocationIDEA = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\CII_Exports\\CIITestRun_Export.xml";


            doc.Load(configLocationIDEA);
            // nodelist is the the elements 
            XmlNodeList nodelist = doc.DocumentElement.ChildNodes;

            XmlNodeList testrunELements = null;

            foreach (XmlNode a in nodelist)
            {
                if (a.Name == "test-run")
                {
                    testrunELements = a.ChildNodes;
                    testrun = new TestRun();
                    testrun.NumGuid = System.Guid.NewGuid();
                    testrun.product = "IDEA";
                    for (int x = 0; x < testrunELements.Count; x++)
                    {



                        //---------------------------------------------Save Test Run in instance---------------------

                        if (testrunELements[x].Name == "test-run-number")
                        {
                            Console.WriteLine("doooo " + testrunELements[x].InnerText);
                            //Thread.Sleep(1000);
                            foreach (string num in trNums)
                            {
                                if (num != testrunELements[x].InnerText)
                                {
                                    Console.WriteLine("doooo " + testrunELements[x].InnerText);
                                    trNums.Add(testrunELements[x].InnerText);
                                }
                                else
                                {
                                    Console.Write(num + "duplicated--------------------------------");
                                    Thread.Sleep(5000);
                                }
                            }
                            testrun.TR_Num = Int32.Parse(testrunELements[x].InnerText);
                            Console.WriteLine(testrun.TR_Num);

                            // Thread.Sleep(2000);
                        }
                        #region[test run other info]
                        if (testrunELements[x].Name == "test-case-number")
                        {
                            testrun.TC_Num = Int32.Parse(testrunELements[x].InnerText);
                        }

                        if (testrunELements[x].Name == "summary")
                        {
                            string summary = testrunELements[x].InnerText;
                            if (summary.Contains("'"))
                            {
                                summary = summary.Replace("'", "''");
                            }
                            testrun.T_Summary = summary;
                        }
                        if (testrunELements[x].Name == "test-type")
                        {
                            string type = testrunELements[x].InnerText;
                            if (type.Contains("'"))
                            {
                                type = type.Replace("'", "''");
                            }
                            testrun.T_Type = type;
                        }
                        if (testrunELements[x].Name == "is-automated")
                        {
                            string isautomated = testrunELements[x].InnerText;
                            if (isautomated.Contains("'"))
                            {
                                isautomated = isautomated.Replace("'", "''");
                            }
                            testrun.IsAutomated = isautomated;
                        }
                        if (testrunELements[x].Name == "estimated-time")
                        {

                            int hour = Int32.Parse(testrunELements[x].Attributes[0].Value);
                            int min = Int32.Parse(testrunELements[x].Attributes[1].Value);
                            int sec = Int32.Parse(testrunELements[x].Attributes[2].Value);

                            testrun.Estimated_Time = hour * 3600 + min * 60 + sec;

                        }
                        if (testrunELements[x].Name == "custom-field-value")
                        {
                            if (testrunELements[x].Attributes[0].Value == "Main Component")
                            {
                                string component = testrunELements[x].Attributes[1].Value;
                                if (component.Contains("'"))
                                {
                                    component = component.Replace("'", "''");
                                }
                                testrun.T_Main_Component = component;
                            }

                            if (testrunELements[x].Attributes[0].Value == "Feature Id")
                            {
                                string featureid = testrunELements[x].Attributes[1].Value;
                                if (featureid.Contains("'"))
                                {
                                    featureid = featureid.Replace("'", "''");
                                }
                                testrun.T_Feature_ID = featureid;
                            }

                        }
                        if (testrunELements[x].Name == "test-run-coverage-value")
                        {
                            if (testrunELements[x].Attributes[0].Value == "IDEA Version")
                            {
                                string ideaversion = testrunELements[x].InnerText;
                                if (ideaversion.Contains("'"))
                                {
                                    ideaversion = ideaversion.Replace("'", "''");
                                }
                                testrun.Env_IDEA_Encoding = ideaversion;
                            }
                            if (testrunELements[x].Attributes[0].Value == "OS")
                            {
                                string os = testrunELements[x].InnerText;
                                if (os.Contains("'"))
                                {
                                    os = os.Replace("'", "''");
                                }
                                testrun.Env_OS = os;
                            }
                            if (testrunELements[x].Attributes[0].Value == "User Type")
                            {
                                string usertype = testrunELements[x].InnerText;
                                if (usertype.Contains("'"))
                                {
                                    usertype = usertype.Replace("'", "''");
                                }
                                testrun.Env_User_Type = usertype;
                            }

                        }

                        if (testrunELements[x].Name == "workflow-status")
                        {
                            string workflowstatus = testrunELements[x].InnerText;
                            if (workflowstatus.Contains("'"))
                            {
                                workflowstatus = workflowstatus.Replace("'", "''");
                            }
                            testrun.T_Status = workflowstatus;
                        }
                        if (testrunELements[x].Name == "created-by")
                        {
                            XmlNodeList createdbyELements = testrunELements[x].ChildNodes;
                            string lastname = "";
                            string firstname = "";
                            foreach (XmlNode nameNode in createdbyELements)
                            {
                                if (nameNode.Name == "last-name")
                                {
                                    lastname = nameNode.InnerText;
                                }
                                if (nameNode.Name == "first-name")
                                {
                                    firstname = nameNode.InnerText;
                                }
                            }
                            testrun.TR_Created_By = firstname + " " + lastname;
                        }
                        if (testrunELements[x].Name == "date-created")
                        {
                            testrun.TR_Date_Created = Convert.ToDateTime(testrunELements[x].InnerText);
                        }
                        if (testrunELements[x].Name == "last-modified-by")
                        {
                            XmlNodeList modifiedbyELements = testrunELements[x].ChildNodes;
                            string lastname = "";
                            string firstname = "";
                            foreach (XmlNode nameNode in modifiedbyELements)
                            {
                                if (nameNode.Name == "last-name")
                                {
                                    lastname = nameNode.InnerText;
                                }
                                if (nameNode.Name == "first-name")
                                {
                                    firstname = nameNode.InnerText;
                                }
                            }
                            testrun.TR_Last_Mod_By = firstname + " " + lastname;
                        }
                        if (testrunELements[x].Name == "date-last-modified")
                        {
                            testrun.TR_Date_Last_Mod = Convert.ToDateTime(testrunELements[x].InnerText);
                        }

                        //-----------------------------save step in an instance-------------------------------------------------

                        if (testrunELements[x].Name == "test-steps")
                        {
                            XmlNodeList stepsELements = testrunELements[x].ChildNodes;
                            bool newstep = false;
                            string stepdetail = "";
                            string stepexpected = "";
                            string comment = "";

                            Step step = null;
                            bool? stepchecked = null;
                            for (int m = 0; m < stepsELements.Count; m++)
                            {

                                if (stepsELements[m].Name == "step")
                                {
                                    newstep = false;
                                    if (stepsELements[m].Attributes[0].Value == "step")
                                    {
                                        stepexpected = "";
                                        newstep = true;
                                        stepdetail = stepsELements[m].InnerText;
                                        if (stepdetail.Contains("'"))
                                        {
                                            stepdetail = stepdetail.Replace("'", "''");

                                        }

                                        comment = "";
                                        if (stepsELements[m].Attributes.Count > 1)
                                        {
                                            if (stepsELements[m].Attributes[1].Value == "true")
                                            { stepchecked = true; }
                                            if (stepsELements[m].Attributes[1].Value == "false")
                                            { stepchecked = false; }

                                        }
                                    }
                                    else if (stepsELements[m].Attributes[0].Value == "comment")
                                    {
                                        newstep = true;
                                        comment = stepsELements[m].InnerText;
                                        stepdetail = "";
                                        stepexpected = "";
                                        stepchecked = null;
                                        if (comment.Contains("'"))
                                        {
                                            comment = comment.Replace("'", "''");

                                        }

                                    }
                                    else if (stepsELements[m].Attributes[0].Value == "expected-result")
                                    {

                                        newstep = false;
                                        stepexpected = stepsELements[m].InnerText;
                                        comment = "";
                                        if (stepexpected.Contains("'"))
                                        {
                                            stepexpected = stepexpected.Replace("'", "''");

                                        }

                                        steps[steps.Count - 1].ExpectedResult = stepexpected;
                                    }
                                    if (newstep)
                                    {
                                        step = new Step();
                                        step.NumGuid = testrun.NumGuid;
                                        step.TR_Num = testrun.TR_Num;
                                        step.StepDetail = stepdetail;
                                        step.Comments = comment;

                                        step.Checked = stepchecked;

                                        steps.Add(step);
                                    }


                                }

                            }

                        }

                        #endregion

                    }
                    testrunList.Add(testrun);
                }

            }
            #region[read txt]
            //------------------read txt file to get info
            //file path
            string buildnumber = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\CII_Exports\\TestRunID_BuildNumber.txt";
            string flavors = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\CII_Exports\\TestRunID_Flavors.txt";
            string folders = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\CII_Exports\\TestRunID_Folders.txt";
            string language = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\CII_Exports\\TestRunID_Language.txt";
            string partialFailNotes = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\CII_Exports\\TestRunID_PartialFailNotes.txt";
            string problemStatements = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\CII_Exports\\TestRunID_ProblemStatements.txt";
            string task = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\CII_Exports\\TestRunID_TASK.txt";
            string testVariants = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\CII_Exports\\TestRunID_TestVariants.txt";

            //-------------------get build number -------------------
            string[] lines = System.IO.File.ReadAllLines(buildnumber);

            for (int a = 1; a < lines.Length; a++)
            {

                string[] dr = lines[a].Split('\t');
                //    Add Contact for current row    new char[] { }   //      
                if (dr.Length > 1)
                {

                    foreach (TestRun tr in testrunList)
                    {

                        if (tr.TR_Num.ToString() == dr[0])
                        {
                            string build = dr[1];
                            if (build.Contains("'"))
                            {
                                build = build.Replace("'", "''");
                            }
                            tr.Build = build;
                            Console.WriteLine(dr[0] + "------------dr[0]" + tr.Build + "------------tr.Build");
                            // Thread.Sleep(2000);

                        }
                    }
                }
            }
            //----------------------------get flavors ----------------
            lines = System.IO.File.ReadAllLines(flavors);

            for (int a = 1; a < lines.Length; a++)
            {
                string[] dr = lines[a].Split(new char[] { '\t' });
                //    Add Contact for current row        //      
                if (dr.Length > 0)
                {
                    foreach (TestRun tr in testrunList)
                    {
                        if (tr.TR_Num.ToString() == dr[0])
                        {
                            string flavor = dr[1];
                            if (flavor.Contains("'"))
                            {
                                flavor = flavor.Replace("'", "''");
                            }
                            tr.Flavour = flavor;
                            Console.WriteLine(dr[0] + "------------dr[0]" + tr.Flavour + "------------tr.Flavour");
                        }
                    }
                }
            }

            //----------------------------get folders ----------------
            lines = System.IO.File.ReadAllLines(folders);

            for (int a = 1; a < lines.Length; a++)
            {
                string[] dr = lines[a].Split(new char[] { '\t' });
                //    Add Contact for current row        //      
                if (dr.Length > 0)
                {
                    foreach (TestRun tr in testrunList)
                    {
                        if (tr.TR_Num.ToString() == dr[0])
                        {
                            string folder = dr[1];
                            if (folder.Contains("'"))
                            {
                                folder = folder.Replace("'", "''");
                            }
                            tr.Folders = folder;
                            Console.WriteLine(dr[0] + "------------dr[0]" + tr.Folders + "------------tr.Folders");
                        }
                    }
                }
            }

            //----------------------------get language ----------------
            lines = System.IO.File.ReadAllLines(language);

            for (int a = 1; a < lines.Length; a++)
            {
                string[] dr = lines[a].Split(new char[] { '\t' });
                //    Add Contact for current row        //      
                if (dr.Length > 0)
                {
                    foreach (TestRun tr in testrunList)
                    {
                        if (tr.TR_Num.ToString() == dr[0])
                        {
                            string lang = dr[1];
                            if (lang.Contains("'"))
                            {
                                lang = lang.Replace("'", "''");
                            }
                            tr.Language = lang;
                            Console.WriteLine(dr[0] + "------------dr[0]" + tr.Language + "------------tr.Language");
                        }
                    }
                }
            }


            //----------------------------get partial fail notes ----------------
            lines = System.IO.File.ReadAllLines(partialFailNotes);

            for (int a = 1; a < lines.Length; a++)
            {
                string[] dr = lines[a].Split(new char[] { '\t' });
                //    Add Contact for current row        //      
                if (dr.Length > 0)
                {
                    foreach (TestRun tr in testrunList)
                    {
                        if (tr.TR_Num.ToString() == dr[0])
                        {
                            string notes = dr[1];
                            if (notes.Contains("'"))
                            {
                                notes = notes.Replace("'", "''");
                            }
                            tr.PartialFailNotes = notes;
                            Console.WriteLine(dr[0] + "------------dr[0]" + tr.PartialFailNotes + "------------tr.PartialFailNotes");
                        }
                    }
                }
            }

            //----------------------------get problem statements ----------------
            lines = System.IO.File.ReadAllLines(problemStatements);

            for (int a = 1; a < lines.Length; a++)
            {
                string[] dr = lines[a].Split(new char[] { '\t' });
                //    Add Contact for current row        //      
                if (dr.Length > 0)
                {
                    foreach (TestRun tr in testrunList)
                    {
                        if (tr.TR_Num.ToString() == dr[0])
                        {
                            string probsta = dr[1];
                            if (probsta.Contains("'"))
                            {
                                probsta = probsta.Replace("'", "''");
                            }
                            tr.ProblemStatement = probsta;
                            Console.WriteLine(dr[0] + "------------dr[0]" + tr.ProblemStatement + "------------tr.ProblemStatement");
                        }
                    }
                }
            }

            //----------------------------get task----------------
            lines = System.IO.File.ReadAllLines(task);

            for (int a = 1; a < lines.Length; a++)
            {
                string[] dr = lines[a].Split(new char[] { '\t' });
                //    Add Contact for current row        //      
                if (dr.Length > 0)
                {
                    foreach (TestRun tr in testrunList)
                    {
                        if (tr.TR_Num.ToString() == dr[0])
                        {
                            string task1 = dr[1];
                            if (task1.Contains("'"))
                            {
                                task1 = task1.Replace("'", "''");
                            }
                            tr.Task = task1;
                            Console.WriteLine(dr[0] + "------------dr[0]" + tr.Task + "------------tr.Task");
                        }
                    }
                }
            }
            //----------------------------get test variants----------------
            lines = System.IO.File.ReadAllLines(testVariants);

            for (int a = 1; a < lines.Length; a++)
            {
                string[] dr = lines[a].Split(new char[] { '\t' });
                //    Add Contact for current row        //      
                if (dr.Length > 0)
                {
                    foreach (TestRun tr in testrunList)
                    {
                        if (tr.TR_Num.ToString() == dr[0])
                        {
                            string variants = dr[1];
                            if (variants.Contains("'"))
                            {
                                variants = variants.Replace("'", "''");
                            }
                            tr.T_Variants = variants;
                            Console.WriteLine(dr[0] + "------------dr[0]" + tr.T_Variants + "------------tr.T_Variants");
                        }
                    }
                }
            }
            #endregion

            #endregion

            #region[import Monitor xml and txt]

            string configLocationMonitor = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\RCM_Exports\\RCMTestRun_Export.xml";


            doc.Load(configLocationMonitor);
            // nodelist is the the elements 
            XmlNodeList nodelistMonitor = doc.DocumentElement.ChildNodes;

            XmlNodeList testrunELementMonitor = null;

            foreach (XmlNode a in nodelistMonitor)
            {
                if (a.Name == "test-run")
                {
                    testrunELementMonitor = a.ChildNodes;
                    testrun = new TestRun();
                    testrun.NumGuid = System.Guid.NewGuid();
                    testrun.product = "Monitor";
                    for (int x = 0; x < testrunELementMonitor.Count; x++)
                    {



                        //---------------------------------------------Save Test Run in instance---------------------

                        if (testrunELementMonitor[x].Name == "test-run-number")
                        {

                            foreach (string num in trNums)
                            {
                                if (num != testrunELementMonitor[x].InnerText)
                                {
                                    Console.WriteLine("doooo " + testrunELementMonitor[x].InnerText);
                                    trNums.Add(testrunELementMonitor[x].InnerText);
                                }

                            }
                            testrun.TR_Num = Int32.Parse(testrunELementMonitor[x].InnerText);
                            Console.WriteLine(testrun.TR_Num);

                            // Thread.Sleep(2000);
                        }
                        #region[test run other info]
                        if (testrunELementMonitor[x].Name == "test-case-number")
                        {
                            testrun.TC_Num = Int32.Parse(testrunELementMonitor[x].InnerText);
                        }

                        if (testrunELementMonitor[x].Name == "summary")
                        {
                            string summary = testrunELementMonitor[x].InnerText;
                            if (summary.Contains("'"))
                            {
                                summary = summary.Replace("'", "''");
                            }
                            testrun.T_Summary = summary;
                        }
                        if (testrunELementMonitor[x].Name == "test-type")
                        {
                            string type = testrunELementMonitor[x].InnerText;
                            if (type.Contains("'"))
                            {
                                type = type.Replace("'", "''");
                            }
                            testrun.T_Type = type;
                        }
                        if (testrunELementMonitor[x].Name == "is-automated")
                        {
                            string isautomated = testrunELementMonitor[x].InnerText;
                            if (isautomated.Contains("'"))
                            {
                                isautomated = isautomated.Replace("'", "''");
                            }
                            testrun.IsAutomated = isautomated;
                        }
                        if (testrunELementMonitor[x].Name == "estimated-time")
                        {

                            int hour = Int32.Parse(testrunELementMonitor[x].Attributes[0].Value);
                            int min = Int32.Parse(testrunELementMonitor[x].Attributes[1].Value);
                            int sec = Int32.Parse(testrunELementMonitor[x].Attributes[2].Value);

                            testrun.Estimated_Time = testrun.Estimated_Time = hour * 3600 + min * 60 + sec; ;

                        }
                        if (testrunELementMonitor[x].Name == "custom-field-value")
                        {
                            if (testrunELementMonitor[x].Attributes[0].Value == "Main Component")
                            {
                                string component = testrunELementMonitor[x].Attributes[1].Value;
                                if (component.Contains("'"))
                                {
                                    component = component.Replace("'", "''");
                                }
                                testrun.T_Main_Component = component;
                            }

                            if (testrunELementMonitor[x].Attributes[0].Value == "Feature Id")
                            {
                                string featureid = testrunELementMonitor[x].Attributes[1].Value;
                                if (featureid.Contains("'"))
                                {
                                    featureid = featureid.Replace("'", "''");
                                }
                                testrun.T_Feature_ID = featureid;
                            }

                        }
                        if (testrunELementMonitor[x].Name == "test-run-coverage-value")
                        {
                            if (testrunELementMonitor[x].Attributes[0].Value == "IDEA Version")
                            {
                                string ideaversion = testrunELementMonitor[x].InnerText;
                                if (ideaversion.Contains("'"))
                                {
                                    ideaversion = ideaversion.Replace("'", "''");
                                }
                                testrun.Env_IDEA_Encoding = ideaversion;
                            }
                            if (testrunELementMonitor[x].Attributes[0].Value == "OS")
                            {
                                string os = testrunELementMonitor[x].InnerText;
                                if (os.Contains("'"))
                                {
                                    os = os.Replace("'", "''");
                                }
                                testrun.Env_OS = os;
                            }
                            if (testrunELementMonitor[x].Attributes[0].Value == "User Type")
                            {
                                string usertype = testrunELementMonitor[x].InnerText;
                                if (usertype.Contains("'"))
                                {
                                    usertype = usertype.Replace("'", "''");
                                }
                                testrun.Env_User_Type = usertype;
                            }

                        }

                        if (testrunELementMonitor[x].Name == "workflow-status")
                        {
                            string workflowstatus = testrunELementMonitor[x].InnerText;
                            if (workflowstatus.Contains("'"))
                            {
                                workflowstatus = workflowstatus.Replace("'", "''");
                            }
                            testrun.T_Status = workflowstatus;
                        }
                        if (testrunELementMonitor[x].Name == "created-by")
                        {
                            XmlNodeList createdbyELements = testrunELementMonitor[x].ChildNodes;
                            string lastname = "";
                            string firstname = "";
                            foreach (XmlNode nameNode in createdbyELements)
                            {
                                if (nameNode.Name == "last-name")
                                {
                                    lastname = nameNode.InnerText;
                                }
                                if (nameNode.Name == "first-name")
                                {
                                    firstname = nameNode.InnerText;
                                }
                            }
                            testrun.TR_Created_By = firstname + " " + lastname;
                        }
                        if (testrunELementMonitor[x].Name == "date-created")
                        {
                            testrun.TR_Date_Created = Convert.ToDateTime(testrunELementMonitor[x].InnerText);
                        }
                        if (testrunELementMonitor[x].Name == "last-modified-by")
                        {
                            XmlNodeList modifiedbyELements = testrunELementMonitor[x].ChildNodes;
                            string lastname = "";
                            string firstname = "";
                            foreach (XmlNode nameNode in modifiedbyELements)
                            {
                                if (nameNode.Name == "last-name")
                                {
                                    lastname = nameNode.InnerText;
                                }
                                if (nameNode.Name == "first-name")
                                {
                                    firstname = nameNode.InnerText;
                                }
                            }
                            testrun.TR_Last_Mod_By = firstname + " " + lastname;
                        }
                        if (testrunELementMonitor[x].Name == "date-last-modified")
                        {
                            testrun.TR_Date_Last_Mod = Convert.ToDateTime(testrunELementMonitor[x].InnerText);
                        }

                        //-----------------------------save step in an instance-------------------------------------------------

                        if (testrunELementMonitor[x].Name == "test-steps")
                        {
                            XmlNodeList stepsELements = testrunELementMonitor[x].ChildNodes;
                            bool newstep = false;
                            string stepdetail = "";
                            string stepexpected = "";
                            string comment = "";

                            Step step = null;
                            bool? stepchecked = null;
                            for (int m = 0; m < stepsELements.Count; m++)
                            {

                                if (stepsELements[m].Name == "step")
                                {
                                    newstep = false;
                                    if (stepsELements[m].Attributes[0].Value == "step")
                                    {
                                        stepexpected = "";
                                        newstep = true;
                                        stepdetail = stepsELements[m].InnerText;
                                        if (stepdetail.Contains("'"))
                                        {
                                            stepdetail = stepdetail.Replace("'", "''");

                                        }

                                        comment = "";
                                        if (stepsELements[m].Attributes.Count > 1)
                                        {
                                            if (stepsELements[m].Attributes[1].Value == "true")
                                            { stepchecked = true; }
                                            if (stepsELements[m].Attributes[1].Value == "false")
                                            { stepchecked = false; }

                                        }
                                    }
                                    else if (stepsELements[m].Attributes[0].Value == "comment")
                                    {
                                        newstep = true;
                                        comment = stepsELements[m].InnerText;
                                        stepdetail = "";
                                        stepexpected = "";
                                        stepchecked = null;
                                        if (comment.Contains("'"))
                                        {
                                            comment = comment.Replace("'", "''");

                                        }

                                    }
                                    else if (stepsELements[m].Attributes[0].Value == "expected-result")
                                    {

                                        newstep = false;
                                        stepexpected = stepsELements[m].InnerText;
                                        comment = "";
                                        if (stepexpected.Contains("'"))
                                        {
                                            stepexpected = stepexpected.Replace("'", "''");

                                        }

                                        steps[steps.Count - 1].ExpectedResult = stepexpected;
                                    }
                                    if (newstep)
                                    {
                                        step = new Step();
                                        step.NumGuid = testrun.NumGuid;
                                        step.TR_Num = testrun.TR_Num;
                                        step.StepDetail = stepdetail;
                                        step.Comments = comment;

                                        step.Checked = stepchecked;

                                        steps.Add(step);
                                    }


                                }

                            }

                        }

                        #endregion

                    }
                    testrunList.Add(testrun);
                }

            }
            string testMonitorFile = "C:\\Users\\bo.wang\\Desktop\\projects\\TestRun_History_Importer\\TestRun_History_Importer\\RCM_Exports\\TestRunID_AllOtherFields.txt";

            //-------------------get build number -------------------
            string[] linesMonitor = System.IO.File.ReadAllLines(testMonitorFile);

            for (int a = 1; a < linesMonitor.Length; a++)
            {

                string[] dr = linesMonitor[a].Split('\t');

                if (dr.Length > 1)
                {
                    foreach (TestRun tr in testrunList)
                    {

                        if (tr.TR_Num.ToString() == dr[0])
                        {
                            string variants = dr[1];
                            if (variants.Contains("'"))
                            {
                                variants = variants.Replace("'", "''");
                            }
                            tr.T_Variants = variants;

                            string problemstatement = dr[2];
                            if (problemstatement.Contains("'"))
                            {
                                problemstatement = problemstatement.Replace("'", "''");
                            }
                            tr.ProblemStatement = problemstatement;
                            string build = dr[3];
                            if (build.Contains("'"))
                            {
                                build = build.Replace("'", "''");
                            }
                            tr.Build = build;

                            string failnotes = dr[4];
                            if (failnotes.Contains("'"))
                            {
                                failnotes = failnotes.Replace("'", "''");
                            }
                            tr.PartialFailNotes = failnotes;

                            string folders1 = dr[5];
                            if (folders1.Contains("'"))
                            {
                                folders1 = folders1.Replace("'", "''");
                            }
                            tr.Folders = folders1;

                            string language1 = dr[6];
                            if (language1.Contains("'"))
                            {
                                language1 = language1.Replace("'", "''");
                            }
                            tr.Language = language1;

                            string flavour = dr[7];
                            if (flavour.Contains("'"))
                            {
                                flavour = flavour.Replace("'", "''");
                            }
                            tr.Flavour = flavour;

                            string task2 = dr[8];
                            if (task2.Contains("'"))
                            {
                                task2 = task2.Replace("'", "''");
                            }
                            tr.Task = task2;

                            Console.WriteLine(dr[0] + "------------dr[0]" + tr.T_Variants + "------------tr.T_Variants");

                        }
                    }
                }
            }
            #endregion

            #region[test run table]
            string connectionString = "Data Source=NORMANDY-SR-1\\AUTOMATIONSERVER;Integrated Security=false;Initial Catalog=TestRun_History;User Id=autouser;Password=1RT51D34!";

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = connectionString;
            connection.Open();

            int i = 0;
            SqlCommand cmd = new SqlCommand();

            //delete Steps table data
            string sqlDeleteSteps = "delete from dbo.Steps";

            cmd.CommandText = sqlDeleteSteps;
            cmd.Connection = connection;
            Console.WriteLine(cmd.ExecuteNonQuery() + "-----data");

            //delete old data from test run
            string sqlDelete = "delete from dbo.TestRun_Data";

            cmd.CommandText = sqlDelete;
            cmd.Connection = connection;
            Console.WriteLine(cmd.ExecuteNonQuery() + "-----data");

            foreach (TestRun tr in testrunList)
            {

                string sqlQuery =
                    "insert into dbo.TestRun_Data(TestRun_DataID,TR_Num,TC_Num,T_Summary,T_Type,T_Main_Component,T_Feature_ID,env_IDEA_Encoding,env_OS,env_User_Type,T_Status,TR_Created_By,TR_Date_Created,TR_Last_Mod_By,TR_Date_Last_Mod,product,isAutomated,Estimated_Time,T_Variants,ProblemStatement,Build,PartialFailNotes,Folders,Language,Flavour,Task) values ('"
                    + i + "', '" + tr.TR_Num + "', '" + tr.TC_Num + "', '" + tr.T_Summary + "', '" +
                    tr.T_Type + "', '" + tr.T_Main_Component + "', '" + tr.T_Feature_ID + "', '" +
                    tr.Env_IDEA_Encoding + "', '" + tr.Env_OS + "', '" + tr.Env_User_Type + "', '" +
                    tr.T_Status + "', '" + tr.TR_Created_By + "', '" + tr.TR_Date_Created + "', '" +
                    tr.TR_Last_Mod_By + "', '" + tr.TR_Date_Last_Mod + "', '" + tr.product + "', '" +
                    tr.IsAutomated + "', '" + tr.Estimated_Time + "', '" + tr.T_Variants + "', '" +
                    tr.ProblemStatement + "', '" + tr.Build + "', '" + tr.PartialFailNotes + "', '" +
                    tr.Folders + "', '" + tr.Language + "', '" + tr.Flavour + "', '" + tr.Task + "')";
                if (tr.TR_Num == 3744)
                {
                    Console.WriteLine(sqlQuery);
                }
                tr.TestRun_DataID = i;
                foreach (Step step in steps)
                {
                    if ((step.TR_Num == tr.TR_Num) && (step.NumGuid == tr.NumGuid))
                    {
                        step.TestRun_DataID = tr.TestRun_DataID;
                    }
                }
                cmd.CommandText = sqlQuery;
                cmd.Connection = connection;
                // Console.WriteLine(sqlQuery);

                Console.WriteLine(cmd.ExecuteNonQuery() + "-----data");

                i++;
            }


            #endregion



            #region[steps table]

            foreach (Step step in steps)
            {

                string sqlQuery = "insert into dbo.Steps(TR_Num,StepDetail,ExpectedResult,Checked,Comments,TestRun_DataID) values ('" + step.TR_Num + "', '" + step.StepDetail + "', '" + step.ExpectedResult + "', '" + step.Checked + "', '" + step.Comments + "', '" + step.TestRun_DataID + "')";

                cmd.CommandText = sqlQuery;
                cmd.Connection = connection;

                if (step.StepDetail == "")
                {
                    Console.WriteLine(sqlQuery);
                }

                Console.WriteLine(cmd.ExecuteNonQuery() + "-----steps");
                //
            }
            Thread.Sleep(500000);

            connection.Close();
            #endregion
        }



    }

}

