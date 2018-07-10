using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Departments
    {
        public Departments()
        {
            Courses = new HashSet<Courses>();
            Professors = new HashSet<Professors>();
            Students = new HashSet<Students>();
        }

        public string DId { get; set; }
        public string Name { get; set; }

        public ICollection<Courses> Courses { get; set; }
        public ICollection<Professors> Professors { get; set; }
        public ICollection<Students> Students { get; set; }
    }
}
