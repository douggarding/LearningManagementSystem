using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Students
    {
        public Students()
        {
            Enrolled = new HashSet<Enrolled>();
            Submissions = new HashSet<Submissions>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Major { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Departments MajorNavigation { get; set; }
        public ICollection<Enrolled> Enrolled { get; set; }
        public ICollection<Submissions> Submissions { get; set; }
    }
}
