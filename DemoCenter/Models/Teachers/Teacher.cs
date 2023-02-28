using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using DemoCenter.Models.Groups;

namespace DemoCenter.Models.Teachers
{
    public class Teacher
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public virtual ICollection<Group> Groups { get; set; }

    }
}
