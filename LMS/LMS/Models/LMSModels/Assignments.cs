using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submissions = new HashSet<Submissions>();
        }

        public int AssignmentId { get; set; }
        public string Name { get; set; }
        public DateTime? Due { get; set; }
        public string Contents { get; set; }
        public int? Points { get; set; }
        public bool SubmissionType { get; set; }
        public int Category { get; set; }

        public AssignmentCategories CategoryNavigation { get; set; }
        public ICollection<Submissions> Submissions { get; set; }
    }
}
