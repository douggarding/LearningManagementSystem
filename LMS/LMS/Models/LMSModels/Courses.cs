using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public int CatalogId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Department { get; set; }

        public Departments DepartmentNavigation { get; set; }
        public ICollection<Classes> Classes { get; set; }
    }
}
