using System;
using DemoCenter.Models.Groups;
using DemoCenter.Models.Students;

namespace DemoCenter.Models.GroupStudents
{
    public class GroupStudent
    {
        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        public Guid StudentId { get; set; }
        public Student Student { get; set; }
    }
}
