using System;
using System.Collections.Generic;
using DemoCenter.Models.Groups;
using Newtonsoft.Json;

namespace DemoCenter.Models.Teachers
{
    public class Teacher
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        [JsonIgnore]
        public  IEnumerable<Group> Groups { get; set; }

    }
}
