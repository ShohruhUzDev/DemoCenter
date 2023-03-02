using DemoCenter.Models.Groups;
using System;
using System.Collections.Generic;

namespace DemoCenter.Models.Students
{
    public class Student
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
