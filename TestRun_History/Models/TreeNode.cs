using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TestRun_History.Models
{
    public class TreeNode
    {
        public int id { get; set; }
        public string text { get; set; }
        public bool leaf { get; set; }

        /// <summary>
        /// test
        /// </summary>
        public List<TreeNode> children { get; set; }




        /// <summary>
        /// 
        /// </summary>
        public TreeNode()
        {
            leaf = false;
            ////test
            children = new List<TreeNode>();
        }
    }
}