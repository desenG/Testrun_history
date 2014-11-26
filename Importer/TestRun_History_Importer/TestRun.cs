using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestRun_History_Importer
{
  public  class TestRun
    {
       
        public string       product { get; set; }
        public int          TR_Num { get; set; }
        public int          TC_Num { get; set; }
        public string       T_Summary { get; set; }
        public string       T_Type { get; set; }
        public string       Steps { get; set; }
        public string       T_Main_Component { get; set; }
        public string       T_Feature_ID { get; set; }
        public string       Env_IDEA_Encoding { get; set; }
        public string       Env_OS { get; set; }
        public string       Env_User_Type { get; set; }
        public string       T_Status { get; set; }
        public string       TR_Created_By { get; set; }
        public DateTime     TR_Date_Created { get; set; }
        public string       TR_Last_Mod_By { get; set; }
        public DateTime     TR_Date_Last_Mod { get; set; }
               
        public string       IsAutomated { get; set; }
      //estimated time is in seconds
        public int          Estimated_Time { get; set; }
        public string       T_Variants { get; set; }
        public string       ProblemStatement { get; set; }
        public string       Build { get; set; }
        public string       PartialFailNotes { get; set; }
        public string       Folders { get; set; }
        public string       Language { get; set; }
        public string       Flavour { get; set; }
        public string       Task { get; set; }
        public int          TestRun_DataID { get; set; }

    }
}
