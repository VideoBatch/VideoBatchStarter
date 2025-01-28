using System.ComponentModel;
using System.Text.Json.Serialization;

// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{
    public class JobTask : Primitive
    {
        [DisplayName("Task Name")]
        public override string Name { get; set; } = String.Empty;

        [JsonIgnore]
        [DisplayName("Task Description")]
        public override string Description { get; set; } = string.Empty;

        public JobTask(string name) : this()
        {
            Name = name;
        }
        public JobTask()
        {
        }
        public override string ToString()
        {
            return $"{Name} - {ID}";
        }

        public override IEnumerable<Primitive> GetEnumerator()
        {
            yield break;
        }
    }
}
