using System.Collections.Generic;
using System;
using DemoCenter.Models.Teachers;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Students;

namespace DemoCenter.Models.Groups
{
    public class Group
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public Guid? TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public Guid? SubjectId { get; set; }
        public Subject Subject { get; set; }
        public virtual ICollection<Student> Students { get; set; }

    }
}
