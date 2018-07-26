using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professors
    {
        public Professors()
        {
            Classes = new HashSet<Classes>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string WorksIn { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Departments WorksInNavigation { get; set; }
        public ICollection<Classes> Classes { get; set; }
    }
}
