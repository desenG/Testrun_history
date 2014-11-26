using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestRun_History_Importer
{
   public  class Step
    {
        public int id { get; set; }
        public int TR_Num { get; set; }
        public string StepDetail { get; set; }
        public string ExpectedResult { get; set; }
       
        public bool ? Checked { get; set; }
        public string Comments { get; set; }
        public int TestRun_DataID { get; set; }
    }
}
