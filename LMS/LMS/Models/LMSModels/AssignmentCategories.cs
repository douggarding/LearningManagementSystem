using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategories
    {
        public AssignmentCategories()
        {
            Assignments = new HashSet<Assignments>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public int Class { get; set; }

        public Classes ClassNavigation { get; set; }
        public ICollection<Assignments> Assignments { get; set; }
    }
}
