using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using PagedList;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System;
using System.Text;
namespace TestRun_History.Models
{
    public class DownloadFileActionResult : ActionResult
    {

        public IEnumerable<TestRun_Data> PagedDatas { get; set; }
        public string fileName { get; set; }

        public DownloadFileActionResult(IEnumerable<TestRun_Data> datas, string pFileName)
        {
            //Assigned the datas going to export
            PagedDatas = datas;
            //Assign the excell'name
            fileName = pFileName;            
        }


        public override void ExecuteResult(ControllerContext context)
        {

            //Create a response stream to create and write the Excel file
            HttpContext curContext = HttpContext.Current;
            curContext.Response.Clear();
            curContext.Response.ClearContent();
            curContext.Response.ClearHeaders();
            curContext.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xls");
            curContext.Response.AddHeader("Content-Type", "application/octet-stream");
            curContext.Response.Charset = "";
            curContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //the header is for uploading excel(97 or 2003)
            curContext.Response.ContentType = "application/vnd.ms-excel";





            int page = 1;
            //Each time, get 300 records and add it to response
            int pagesize = 300;
            //temp variable to store 300 records
            IPagedList<TestRun_Data> PageData;

            do
            {
                //get 300 records and then get next 300 records
                PageData = PagedDatas.ToPagedList(page++, pagesize);
                //bine the data with ExcelGridView
                GridView ExcelGridView = new GridView();
                ExcelGridView.DataSource = PageData;
                ExcelGridView.DataBind();

                //Convert the rendering of the gridview to a string representation 
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                ExcelGridView.RenderControl(htw);
                String toExcel = sw.ToString();

                //Modified the column header
                toExcel = toExcel.Replace("TR_Num", "Test Run Number");
                toExcel = toExcel.Replace("TC_Num", "Test Case Number");
                toExcel = toExcel.Replace("T_Summary", "Summary");
                toExcel = toExcel.Replace("T_Type", "Test Type");
                toExcel = toExcel.Replace("T_Main_Component", "Component");
                toExcel = toExcel.Replace("T_Feature_ID", "Feature ID");
                toExcel = toExcel.Replace("env_IDEA_Encoding", "IDEA Encoding");
                toExcel = toExcel.Replace("env_OS", "OS");
                toExcel = toExcel.Replace("env_User_Type", "User Type");
                toExcel = toExcel.Replace("T_Status", "Status");
                toExcel = toExcel.Replace("TR_Created_By", "Created By");
                toExcel = toExcel.Replace("TR_Date_Created", "Date Created");
                toExcel = toExcel.Replace("TR_Last_Mod_By", "Last Modified By");
                toExcel = toExcel.Replace("TR_Date_Last_Mod", "Date Modified");
                toExcel = toExcel.Replace("Estimated_Time", "Estimated Time");
                toExcel = toExcel.Replace("T_Variants", "Variants");
                toExcel = toExcel.Replace("ProblemStatement", "Problem Statement");
                toExcel = toExcel.Replace("PartialFailNotes", "Partial Fail Notes");

                //Open a memory stream that you can use to write back to the response
                byte[] byteArray = Encoding.ASCII.GetBytes(toExcel);
                MemoryStream s = new MemoryStream(byteArray);
                StreamReader sr = new StreamReader(s, Encoding.ASCII);

                //Write the stream back to the response
                curContext.Response.Write(sr.ReadToEnd());
                s.Close();
                sr.Close();
                htw.Close();
                sw.Close();

            } while (PageData.HasNextPage);//get next 300 records
            //add cookie to response indicating that response going to end
            //on client side, js will keep detecting the cookie until get "finish" token
            curContext.Response.Cookies["fileDownloadToken"].Value = "finish";
            //sent response
            curContext.Response.Flush();
            //end response
            curContext.Response.End();

        }

    }
}