﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Administrators
    {
        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public DateTime DOB { get; set; }
    }
}
