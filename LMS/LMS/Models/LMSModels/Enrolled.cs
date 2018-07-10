using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrolled
    {
        public string Student { get; set; }
        public int Class { get; set; }
        public string Grade { get; set; }

        public Classes ClassNavigation { get; set; }
        public Students StudentNavigation { get; set; }
    }
}
