using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submissions
    {
        public int Assignment { get; set; }
        public string Student { get; set; }
        public int? Score { get; set; }
        public string TextContents { get; set; }
        public DateTime? Time { get; set; }
        public byte[] BinaryContents { get; set; }

        public Assignments AssignmentNavigation { get; set; }
        public Students StudentNavigation { get; set; }
    }
}
