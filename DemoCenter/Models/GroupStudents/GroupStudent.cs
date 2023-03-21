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

        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
