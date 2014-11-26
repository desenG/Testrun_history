using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestRun_History.ViewModels
{
    public class Steps_w_num
    {
        public int ID { get; set; }
        public string Step_Num { get; set; }
        public int TR_Num { get; set; }
        public string StepDetail { get; set; }
        public string ExpectedResult { get; set; }
        public string Checked { get; set; }
        public string Comments { get; set; }
        public int TestRun_DataID { get; set; }
    }
}