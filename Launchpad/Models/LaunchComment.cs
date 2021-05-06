using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Launchpad.Models
{
    public class LaunchComment
    {
        public string CommentText { get; set; }
        public string CommentUser { get; set; }
        public DateTime CommentDate { get; set; }
        public string CommentStatus { get; set; }
    }
}