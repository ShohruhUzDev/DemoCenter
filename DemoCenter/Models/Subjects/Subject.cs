using DemoCenter.Models.Groups;
using System;
using System.Collections.Generic;

namespace DemoCenter.Models.Subjects
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string SubjectName { get; set; }
        public int Price { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
