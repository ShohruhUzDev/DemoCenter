using System;
using System.Collections.Generic;
using DemoCenter.Models.GroupStudents;
using System.Text.Json.Serialization;
using DemoCenter.Models.Students;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Teachers;

namespace DemoCenter.Models.Groups
{
    public class Group
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public Guid TeacherId { get; set; }
        public Guid SubjectId { get; set; }    
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
       
        public Teacher Teacher { get; set; }
        public Subject Subject { get; set; }
        
        [JsonIgnore]
        public virtual IEnumerable<GroupStudent> GroupStudents { get; set; }
    }
}
