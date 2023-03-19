using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DemoCenter.Models.Groups;
using DemoCenter.Models.GroupStudents;

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

        [JsonIgnore]
        public virtual IEnumerable<GroupStudent> GroupStudents { get; set; }
    
    }
}
