using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enrolled = new HashSet<Enrolled>();
        }

        public int ClassId { get; set; }
        public string Location { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Season { get; set; }
        public string TaughtBy { get; set; }
        public int Offering { get; set; }
        public int Year { get; set; }

        public Courses OfferingNavigation { get; set; }
        public Professors TaughtByNavigation { get; set; }
        public ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public ICollection<Enrolled> Enrolled { get; set; }
    }
}
