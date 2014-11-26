using PagedList;
using System.Linq;
using System.Web.Mvc;
using TestRun_History.Filters;
using TestRun_History.Models;
using System;
using WebMatrix.WebData;
using System.Collections.Generic;
using System.Web.Security;
using System.Data;
using TestRun_History.ViewModels;
using Microsoft.Web.WebPages.OAuth;


namespace TestRun_History.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class HomeController : Controller
    {

        private UnitOfWork unitOfWork = new UnitOfWork();

        private void redirectToHomeIndex()
        {
            string url = HttpContext.Request.Url.AbsoluteUri;
            string path = HttpContext.Request.Url.AbsolutePath.ToUpper();
            Uri uri = HttpContext.Request.Url;
            string host = uri.Host + ":" + uri.Port;//		url	"http://localhost:41399/Account/Login"	string


            if (!uri.Host.Equals("localhost"))
            {
                //for testing
                //if run on local,redirect to "localhost/TestRun_test/Home/Index" if url is either "localhost/Testrun_test/"
                //or "localhost/Testrun_test"
                if (path.Equals(("/Testrun_test").ToUpper()) || path.Equals(("/Testrun_test/").ToUpper()))
                {
                    // the URI for which you want to change the host name
                    var oldUri = Request.Url;

                    // create a new UriBuilder, which copies all fragments of the source URI
                    var newUriBuilder = new UriBuilder(oldUri);

                    // set the new host (you can set other properties too)
                    newUriBuilder.Path = "/TestRun_test/Home/Index";

                    // get a Uri instance from the UriBuilder
                    var newUri = newUriBuilder.Uri;
                    Response.Redirect(newUri.AbsoluteUri);
                }
                if (path.Equals(("/Testrun").ToUpper()) || path.Equals(("/Testrun/").ToUpper()))
                {
                    // the URI for which you want to change the host name
                    var oldUri = Request.Url;

                    // create a new UriBuilder, which copies all fragments of the source URI
                    var newUriBuilder = new UriBuilder(oldUri);

                    // set the new host (you can set other properties too)
                    newUriBuilder.Path = "/TestRun/Home/Index";

                    // get a Uri instance from the UriBuilder
                    var newUri = newUriBuilder.Uri;
                    Response.Redirect(newUri.AbsoluteUri);
                }
            }
            else
            {
                //if run on server-for example"Nomandy/testrun",redirect to "Nomandy/testrun/TestRun_test/Home/Index" if url is either "Nomandy/testrun/TestRun_test"
                //or "Nomandy/testrun/TestRun_test/"
                if (!path.Equals(("/Home/Index").ToUpper()))
                {

                    // the URI for which you want to change the host name
                    var oldUri = Request.Url;

                    // create a new UriBuilder, which copies all fragments of the source URI
                    var newUriBuilder = new UriBuilder(oldUri);

                    // set the new host (you can set other properties too)
                    newUriBuilder.Path = "/Home/Index";

                    // get a Uri instance from the UriBuilder
                    var newUri = newUriBuilder.Uri;
                    Response.Redirect(newUri.AbsoluteUri);
                }


            }
        }
        /// <summary>
        /// Action name:Index
        /// Action description:
        ///                     1.It is start page for the application.
        ///                     2.It is Return different view base on whether 
        ///                       the user is authenticated.
        ///                     3.Allow Anonymous user to invoke the action
        ///  Errors and Exceptions: N/A 
        ///  Required libraries: System.Web.Security
        ///  Any warnings for maintenance: none
        ///  Unresolved issues: none
        /// </summary>
        /// <returns>ActionResult</returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            /**
             *  invoke this method to redirect Home page
             *  resolve the issue "url without slash will cause problem."
             * */
            redirectToHomeIndex();

            //instantiated a UserProfile and start to get the visitor information
            UserProfile visitor = new UserProfile();

            //this is single sign-on
            if (WebSecurity.IsAuthenticated)
            {
                //if the visitor has been authenticated, get all information and store in vistor instance
                visitor.UserId = WebSecurity.CurrentUserId;
                visitor.UserName = WebSecurity.CurrentUserName;
            }
            //route to index.cshtml and meanwhile, pass visitor instance to the view.
            return View(visitor);

        }
        /// <summary>
        /// Action name:detail
        /// Action description:
        ///                     1.It is to return a view with all detial of specific test run
        ///                     2.Allow Anonymous user to invoke the action
        ///  Errors and Exceptions: "No such record!" display on return view if no-existing test run is request 
        ///  Required libraries: System.Data
        ///  Any warnings for maintenance: none
        ///  Unresolved issues: none
        /// </summary>
        /// <param name="Product">the product of request test run-Idea or Monitor</param>
        /// <param name="testRunNum">the number of request test run</param>
        /// <returns>ActionResult</returns>
         [AllowAnonymous]
        public ActionResult detail(String Product, Int32 testRunNum)
        {
            //instantiate a TestRun_Data model using to store query result
            TestRun_Data TestRunData = new TestRun_Data();
            try
            {
                //get specific test run which contain specific product and test run number
                TestRunData = unitOfWork.TestRunDataRepository.GetTestRun_Datas().Where(i => i.product == Product && i.TR_Num == testRunNum).Single();
                //if get the record, rount to detail.cshtml with the instance
                return View(TestRunData);
            }
            catch(Exception)
            {
                //if no such record, catch the exception
                ViewBag.Exception = "No such record!";
                // route to detail.cshtml with the instance that contains all field with null value
                return View(TestRunData);
            }

         
        }

        /// <summary>
         /// method name:addChildren
         /// Action description:
         ///                     1.It is a recursive function
         ///                     2.From hign level to low to keep detect if a node has children node,
         ///                       append children node into the node children list
         ///                     3. It is only looking for node tree for specific ownner
         ///  Errors and Exceptions: "No such record!" display on return view if no-existing test run is request 
         ///  Required libraries: System;System.Linq
         ///  Any warnings for maintenance: none
         ///  Unresolved issues: none 
        /// </summary>
        /// <param name="node">the node need to be detect if it has children</param>
        /// <param name="visitorID">the owner of node tree</param>
        private void addChildren(TreeNode node, Int32? visitorID)
        {
            // Get the list of queries ordered by name
            var sub_Queries = unitOfWork.QueryRepository.GetByID(node.id).Query1.OrderBy(f => f.Name);
            if (visitorID != null)
            {
                // if parameter visitorID is not null, get the sub queries of parameter node
                sub_Queries = sub_Queries.Where(v => v.CreatedBy == visitorID).OrderBy(f => f.Name);
            }//otherwise, visitor is guest

            
            if (sub_Queries.Count() > 0)
            {
                //if parameter node has sub queries nodes, add them to its children list by using loop
                foreach (Query sub_Query in sub_Queries)
                {
                    TreeNode sub_node = new TreeNode
                    {
                        text = (sub_Query.Name.Trim() ?? ""),
                        leaf = Convert.ToBoolean(sub_Query.leaf),
                        id = sub_Query.QueryID
                    };
                    node.children.Add(sub_node);
                    //this is the recursive template
                    //add children for node's sub queries.
                    addChildren(sub_node, visitorID);
                }
            }

        }
        /// <summary>
        /// method name:addbranch
        /// Action description:
        ///                     1.It is to add the branch of the whole tree-my query or team query
        ///                     2.add my query branch if branch id equal 2 ;
        ///                       add team query branch if branch id equal 1 
        ///                     3.invoke recursive function addChildren
        ///                     4.pass visitorID to function  addChildren
        ///                     5.pass treeNodeList to function  alow treeNodeList to instore whole tree structure
        ///  Errors and Exceptions: "No such record!" display on return view if no-existing test run is request 
        ///  Required libraries: N/A
        ///  Any warnings for maintenance: none
        ///  Unresolved issues: none 
        /// </summary>
        /// <param name="branchID"></param>
        /// <param name="treeNodeList"></param>
        /// <param name="visitorID"></param>
        private void addbranch(int branchID, List<TreeNode> treeNodeList, Int32? visitorID)//,List<TreeNode> treeNodeList,Int16 id)
        {
            //Only two top folder-team query and my query
            //base on the branch ID to decide the branch going to add is for team query(branchID=1) or my query (branchID=2)
            //get the top node - team query(if branchID=1) or my query (if branchID=2)
            var topFolder = unitOfWork.QueryRepository.GetByID(branchID);
            //generate the object base on the query result
            var topNode = new TreeNode
            {
                text = topFolder.Name.Trim(),
                leaf = false,
                id = topFolder.QueryID
            };
            //default treeNodeList only contain 'root' node
            //add the topNode to 'root'
            treeNodeList.Add(topNode);

            //recursively adding childrent node from topNode to sub nodes
            //May pass visitor ID, to specific the node to add only for specific user
            addChildren(topNode, visitorID);

        }
        [AllowAnonymous]
        public JsonResult Load_Tree()
        {

            try
            {
                //instantiate a array to store top tree node by using add method
                List<TreeNode> treeNodeList = new List<TreeNode>();

                
                
                if (WebSecurity.HasUserId)
                {
                    // login user has ID which means he is authenticated
                    //add my query branch to array list
                    addbranch(2, treeNodeList, WebSecurity.CurrentUserId);

                }
                //add team query branch to every body
                addbranch(1, treeNodeList, null);

        

                return Json(treeNodeList, JsonRequestBehavior.AllowGet);


            }
            catch (DataException)
            {
                //return a json response with customized information if get a DataException
                return Json(new
                {
                    success = false,
                    message = "EntityException was caught. Please contact with administrator."
                });
            }

        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Create_TreeNode(Query data,Int32 ? createBy)
        {
            //instantiated a Query by using parameter
            Query query = new Query() { Name = data.Name, leaf = data.leaf, FolderID=data.FolderID };
            //createBy is nullable, so I split this code from above code, otherwise application will get exception
            query.CreatedBy = createBy;

            try
            {
                //insert the new tree node to table Query in database
                unitOfWork.QueryRepository.Insert(query);
                //commit
                unitOfWork.SaveTestRun();


                return Json(new
                {
                    //return a json response with customized information if no Exception
                    success = true,
                    message = "Create method called successfully"
                });

            }
            catch (DataException)
            {
                return Json(new
                {
                    //return a json response with customized information if catch a DataException
                    success = false,
                    message = "Unable to save changes. Try again, and if the problem persists, see your system administrator."
                });
            }


        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Update_TreeNode(TreeNode data)
        {
            //get tree node with ID equal data.id
            Query query = unitOfWork.QueryRepository.GetByID(data.id);
            //update the name using new name
            query.Name = data.text;

            try
            {
                //update the node
                unitOfWork.QueryRepository.Update(query);
                //commit
                unitOfWork.SaveTestRun();
                return Json(new
                {
                    //return a json response with customized information if no Exception
                    success = true,
                    message = "Update method called successfully"
                });

            }
            catch (DataException)
            {
                return Json(new
                {
                    //return a json response with customized information if catch a DataException
                    success = false,
                    message = "Unable to save changes. Try again, and if the problem persists, see your system administrator."
                });
            }
        }
        //the array is used with Home control
        private List<Query> SingelQueries = new List<Query>();

        //function DeleteSingelQueries and findSingelQueries
        //together are to implement "delete cascade" feature
        //SQL Server has "delete cascade" feature
        //but it only applies polybasic
        //not applies unary entity relationship
        private void DeleteSingelQueries(Query node)
        {

            if (node.Query1.Count > 0)
            {
                //if node has sub node, 
                //use function findSingelQueries to go through all node(top-down) 
                //and find a node without any descendant
                //store in array SingelQueries
                foreach (Query childnode in node.Query1)
                {

                    findSingelQueries(childnode);
                }
                //delete all node in array SingelQueries
                foreach (Query SingelQuery in SingelQueries)
                {

                    unitOfWork.QueryRepository.Delete(SingelQuery);
                }
                //clear array SingelQueries
                SingelQueries.Clear();
                //invoke DeleteSingelQueries
                //again, find nodes without any descendant and delete then
                //it is never stop until delete the parameter node
                DeleteSingelQueries(node);

            }
            else
            {   //if node has no children node
                //delete the node directory
                unitOfWork.QueryRepository.Delete(node);
            }


        }

        private void findSingelQueries(Query node)
        {
            
            if (node.Query1.Count > 0)
            {
                //if node has children node
                //invoke findSingelQueries with each child node
                //it is to go througn all descendant of the node
                //and find the descendants without child node
                //store in array SingelQueries 
                foreach (Query childNode in node.Query1)
                {

                    findSingelQueries(childNode);
                    
                }

            }
            else
            {
                //if node don't has child node
                //store the child node in array SingelQueries
                SingelQueries.Add(node);
            }

            
        }



        [AllowAnonymous]
        [HttpPost]
        public JsonResult Delete_TreeNode(TreeNode data)
        {


            try
            {
                //find the query in database
                Query query = unitOfWork.QueryRepository.GetByID(data.id);
                //delete the query
                DeleteSingelQueries(query);
                //commit
                unitOfWork.SaveTestRun();
                //return a json response with customized information if no Exception
                return Json(new
                {
                    success = true,
                    message = "Delete method called successfully"
                });

            }
            catch (DataException)
            {
                //return a json response with customized information if no Exception
                return Json(new
                {
                    success = false,
                    message = "Unable to save changes. Try again, and if the problem persists, see your system administrator."
                });
            }


        }

        
        private string stringTrim(string text)
        {
            if(text!=null)
            {
                //if parameter text is not null,
                //remove spaces from the beginning and end of the supplied string
                //and return the new text
                //if supplied string is null
                //return null
                text=text.Trim();   
            }

            return text;
        }


        [AllowAnonymous]
        public JsonResult Load_Query(Int32 id)
        {


            //from database, get all query in queries
            var queries = unitOfWork.QueryRepository.Get(
            );


            //from queries, get single query with supplied id 
            //and then, get a list of all clause in the single query
            IEnumerable<Clause> ClauseList = queries.Where(i => i.QueryID == id).Single().Clauses.OrderBy(o=>o.Index);

            //go througn all clause
            //remove spaces at the beginning and end of every attribute
            foreach (Clause a in ClauseList)
            {
                //invoke stringTrim to get null or FieldName 
                //without spaces at the beginning and end of the string from database
                a.FieldName = stringTrim(a.FieldName);
                //invoke stringTrim to get null or Operand 
                //without spaces at the beginning and end of the string from database
                a.Operand = stringTrim(a.Operand);
                //invoke stringTrim to get null or Operator 
                //without spaces at the beginning and end of the string from database
                a.Operator = stringTrim(a.Operator);
                //invoke stringTrim to get null or Value 
                //without spaces at the beginning and end of the string from database
                a.Value = stringTrim(a.Value);
            }

            //return Json object with the list of clauses
            return Json(new
            {
                success = true,
                total = ClauseList.Count(),
                data = ClauseList.ToList()
            }, JsonRequestBehavior.AllowGet);


        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Clear_Query(Int32 queryID)
        {
            //from database, get all query in queries
            var queries = unitOfWork.QueryRepository.Get(
            );


            //from queries, get single query with supplied id 
            //and then, get a list of all clause in the single query
            IEnumerable<Clause> ClauseList = queries.Where(i => i.QueryID == queryID).Single().Clauses.OrderBy(o => o.Index);


            if (ClauseList.Count() > 0)
            {
                //if the query with supplied ID has clauses
                //delete all the clauses
                foreach (Clause clause in ClauseList)
                {
                    unitOfWork.ClauseRepository.Delete(clause);
                }
                //commit
                unitOfWork.SaveTestRun();
            }
            //return a json response with customized information
            return Json(new
            {
                success = true,
                message = "Clear Query successfully"
            });
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Update_Query(Clause data)
        {
            try
            {
                //insert supplied clause to database
                unitOfWork.ClauseRepository.Insert(data);
                //commit
                unitOfWork.SaveTestRun();
                //return a json response with customized information
                return Json(new
                {
                    success = true,
                    message = "Update method called successfully"
                });

            }
            catch (DataException)
            {
                //return a json response with customized information if no Exception
                return Json(new
                {
                    success = false,
                    message = "Unable to save changes. Try again, and if the problem persists, see your system administrator."
                });
            }
        }
        //[HttpPost]
         [AllowAnonymous]
        public JsonResult LoadPage_TestRunData(string query, int? pageSize, int? page)
        {

                     
            //if supplied page size equal null return 30
            //otherwise, use the supplied page size
            int Size = (pageSize ?? 30);
            //get test run records from database by using supplied query
            var TestRunDatas = unitOfWork.TestRunDataRepository.GetWithRawSqlTestRun_Datas(query);
            //if supplied page number equal null return 1
            //otherwise, use the supplied page number
            int pageNumber = (page ?? 1);
            //get a page of test run records with specific page number, and page size
            var PageData = TestRunDatas.ToPagedList(pageNumber, Size);
            //return Json object with the test run records and paging information
            return Json(new
            {
                success = true,
                total = PageData.Count(),
                data = PageData.ToList(),
                ///////paging Info
                FirstItemOnpage = PageData.FirstItemOnPage,
                HasPreviousPage=PageData.HasPreviousPage,
                HasNextPage = PageData.HasNextPage,
                IsFirstPage=PageData.IsFirstPage,
                IsLastPage=PageData.IsLastPage,
                LastItemOnPage=PageData.LastItemOnPage,
                PageCount=PageData.PageCount,
                PageNumber=PageData.PageNumber,
                PageSize=PageData.PageSize,
                TotalItemCount = PageData.TotalItemCount
            }, JsonRequestBehavior.AllowGet);
        }

         [AllowAnonymous]
        public JsonResult Load_Steps(Int32? TestRun_DataID)
        {
            //get sorted steps from database with supplied test run id
            var Steps = unitOfWork.StepRepository.Get(            
                orderBy: P => P.OrderBy(s => s.ID),
                filter: f => f.TestRun_DataID == TestRun_DataID.Value
            );
            //array for store Steps_w_num type instance
            //view model Steps_w_num is Step with step number
            List<Steps_w_num> steps_w_nums = new List<Steps_w_num>();


            var i = 0;
            var stepNum = 1;
            //copy attributes of each step except comment
            //dont assign step number if the step is not a real step but a comment
            //assign step number if the step is a real step but a comment
            foreach (Step s in Steps)
            {
                var comm = (s.Comments).Trim();
                Steps_w_num steps_w_num = new Steps_w_num();
                steps_w_num.ID = s.ID;
                steps_w_num.TR_Num = s.TR_Num;
                steps_w_num.StepDetail = s.StepDetail;
                steps_w_num.ExpectedResult = s.ExpectedResult;
                steps_w_num.Checked = s.Checked;
                steps_w_num.TestRun_DataID = s.TestRun_DataID;



                if (comm.Length > 0)
                {
                    //if comment is not ""
                    //append "Comments : " at the bigging of the comment
                    steps_w_num.Comments = "Comments : " + s.Comments;
                    steps_w_num.Step_Num = "";
                }
                else
                {
                    //if if comment is ""
                    //copy it
                    steps_w_num.Comments = s.Comments;
                    //assign step number for the steps
                    steps_w_num.Step_Num = (stepNum++).ToString(); 
                }
                //store the step in array
                steps_w_nums.Add(steps_w_num);
                i++;
            }


            //return Json object with steps
            return Json(new
            {
                success = true,
                total = steps_w_nums.Count(),
                data = steps_w_nums.ToList(),

            }, JsonRequestBehavior.AllowGet);
        }//Load_Steps

        
        public JsonResult GET_language()
        {

            //get all test run 
            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            //get test run set with distinct language
            var TestRunDatas_languageDistinct = TestRunDatas.Where(r => r.Language != ""&& r.Language !=null).GroupBy(r => r.Language).Select(g => g.FirstOrDefault()).OrderBy(r => r.Language);

            List<string> LanguageList = new List<string>();
            //get a list of distinct language
            foreach (var a in TestRunDatas_languageDistinct)
            {
                LanguageList.Add(a.Language);
            }
            //return Json object with language list
            return Json(new
            {
                success = true,
                total = LanguageList.Count(),
                data = LanguageList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_T_Type()
        {

            //get all test run 
            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            //get test run set with distinct T_Type
            var TestRunDatas_T_TypeDistinct = TestRunDatas.Where(r => r.T_Type != "" && r.T_Type != null).GroupBy(r => r.T_Type).Select(g => g.FirstOrDefault()).OrderBy(r => r.T_Type);

            List<string> T_TypeList = new List<string>();
            //get a list of distinct T_Type
            foreach (var a in TestRunDatas_T_TypeDistinct)
            {
                T_TypeList.Add(a.T_Type);
            }
            //return Json object with T_Type list
            return Json(new
            {
                success = true,
                total = T_TypeList.Count(),
                data = T_TypeList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_T_Feature_ID()
        {

            //get all test run 
            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            //get test run set with distinct Feature_ID
            var TestRunDatas_T_Feature_IDDistinct = TestRunDatas.Where(r => r.T_Feature_ID != "" && r.T_Feature_ID != null).GroupBy(r => r.T_Feature_ID).Select(g => g.FirstOrDefault()).OrderBy(r => r.T_Feature_ID);

            List<string> T_Feature_IDList = new List<string>();
            //get a list of distinct Feature_ID
            foreach (var a in TestRunDatas_T_Feature_IDDistinct)
            {
                T_Feature_IDList.Add(a.T_Feature_ID);
            }
            //return Json object with Feature_ID list
            return Json(new
            {
                success = true,
                total = T_Feature_IDList.Count(),
                data = T_Feature_IDList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_env_IDEA_Encoding()
        {

            //get all test run 
            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            //get test run set with distinct env_IDEA_Encoding
            var TestRunDatas_env_IDEA_EncodingDistinct = TestRunDatas.Where(r => r.env_IDEA_Encoding != "" && r.env_IDEA_Encoding != null).GroupBy(r => r.env_IDEA_Encoding).Select(g => g.FirstOrDefault()).OrderBy(r => r.env_IDEA_Encoding);

            List<string> env_IDEA_EncodingList = new List<string>();
            //get a list of distinct env_IDEA_Encoding
            foreach (var a in TestRunDatas_env_IDEA_EncodingDistinct)
            {
                env_IDEA_EncodingList.Add(a.env_IDEA_Encoding);
            }
            //return Json object with env_IDEA_EncodingDistinct list
            return Json(new
            {
                success = true,
                total = env_IDEA_EncodingList.Count(),
                data = env_IDEA_EncodingList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_env_OS()
        {

            //get all test run 
            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            //get test run set with distinct env_OS
            var TestRunDatas_env_OSDistinct = TestRunDatas.Where(r => r.env_OS != "" && r.env_OS != null).GroupBy(r => r.env_OS).Select(g => g.FirstOrDefault()).OrderBy(r => r.env_OS);

            List<string> env_OSList = new List<string>();
            //get a list of distinct env_OS
            foreach (var a in TestRunDatas_env_OSDistinct)
            {
                env_OSList.Add(a.env_OS);
            }
            //return Json object with env_OS list
            return Json(new
            {
                success = true,
                total = env_OSList.Count(),
                data = env_OSList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_env_User_Type()
        {


            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            var TestRunDatas_env_User_TypeDistinct = TestRunDatas.Where(r => r.env_User_Type != "" && r.env_User_Type != null).GroupBy(r => r.env_User_Type).Select(g => g.FirstOrDefault()).OrderBy(r => r.env_User_Type);

            List<string> env_User_TypeList = new List<string>();

            foreach (var a in TestRunDatas_env_User_TypeDistinct)
            {
                env_User_TypeList.Add(a.env_User_Type);
            }

            return Json(new
            {
                success = true,
                total = env_User_TypeList.Count(),
                data = env_User_TypeList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_T_Status()
        {


            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            var TestRunDatas_T_StatusDistinct = TestRunDatas.Where(r => r.T_Status != "" && r.T_Status != null).GroupBy(r => r.T_Status).Select(g => g.FirstOrDefault()).OrderBy(r => r.T_Status);

            List<string> T_StatusList = new List<string>();

            foreach (var a in TestRunDatas_T_StatusDistinct)
            {
                T_StatusList.Add(a.T_Status);
            }

            return Json(new
            {
                success = true,
                total = T_StatusList.Count(),
                data = T_StatusList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_TR_Created_By()
        {


            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            var TestRunDatas_TR_Created_ByDistinct = TestRunDatas.Where(r => r.TR_Created_By != "" && r.TR_Created_By != null).GroupBy(r => r.TR_Created_By).Select(g => g.FirstOrDefault()).OrderBy(r => r.TR_Created_By);

            List<string> TR_Created_ByList = new List<string>();

            foreach (var a in TestRunDatas_TR_Created_ByDistinct)
            {
                TR_Created_ByList.Add(a.TR_Created_By);
            }

            return Json(new
            {
                success = true,
                total = TR_Created_ByList.Count(),
                data = TR_Created_ByList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_TR_Last_Mod_By()
        {


            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            var TestRunDatas_TR_Last_Mod_ByDistinct = TestRunDatas.Where(r => r.TR_Last_Mod_By != "" && r.TR_Last_Mod_By != null).GroupBy(r => r.TR_Last_Mod_By).Select(g => g.FirstOrDefault()).OrderBy(r => r.TR_Last_Mod_By);

            List<string> TR_Last_Mod_ByList = new List<string>();

            foreach (var a in TestRunDatas_TR_Last_Mod_ByDistinct)
            {
                TR_Last_Mod_ByList.Add(a.TR_Last_Mod_By);
            }

            return Json(new
            {
                success = true,
                total = TR_Last_Mod_ByList.Count(),
                data = TR_Last_Mod_ByList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_T_Main_Component()
        {


            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            var TestRunDatas_T_Main_ComponentDistinct = TestRunDatas.Where(r => r.T_Main_Component != "" && r.T_Main_Component != null).GroupBy(r => r.T_Main_Component).Select(g => g.FirstOrDefault()).OrderBy(r => r.T_Main_Component);

            List<string> T_Main_ComponentList = new List<string>();

            foreach (var a in TestRunDatas_T_Main_ComponentDistinct)
            {
                T_Main_ComponentList.Add(a.T_Main_Component);
            }

            return Json(new
            {
                success = true,
                total = T_Main_ComponentList.Count(),
                data = T_Main_ComponentList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_Task()
        {


            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            var TestRunDatas_TaskDistinct = TestRunDatas.Where(r => r.Task != "" && r.Task != null).GroupBy(r => r.Task).Select(g => g.FirstOrDefault()).OrderBy(r => r.Task);

            List<string> TaskList = new List<string>();

            foreach (var a in TestRunDatas_TaskDistinct)
            {
                TaskList.Add(a.Task);
            }

            return Json(new
            {
                success = true,
                total = TaskList.Count(),
                data = TaskList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GET_Flavour()
        {


            var TestRunDatas = unitOfWork.TestRunDataRepository.GetTestRun_Datas();
            var TestRunDatas_FlavourDistinct = TestRunDatas.Where(r => r.Flavour != "" && r.Flavour != null).GroupBy(r => r.Flavour).Select(g => g.FirstOrDefault()).OrderBy(r => r.Flavour);

            List<string> FlavourList = new List<string>();

            foreach (var a in TestRunDatas_FlavourDistinct)
            {
                FlavourList.Add(a.Flavour);
            }

            return Json(new
            {
                success = true,
                total = FlavourList.Count(),
                data = FlavourList.ToList()
            }, JsonRequestBehavior.AllowGet);
        }


         [AllowAnonymous]
        public ActionResult Download(string query, string fileName)
        {
            //get test run records from database by using supplied query
            IEnumerable<TestRun_Data> TestRunDatas = unitOfWork.TestRunDataRepository.GetWithRawSqlTestRun_Datas(query);
            //Use DownloadFileActionResult model object to download data with supplied fileName
            return new DownloadFileActionResult(TestRunDatas, fileName); 
        }
        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    
    }
}
