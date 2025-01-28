using System.ComponentModel;
using System.Text.Json.Serialization;

// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{
    public class Job : Primitive
    {
        [DisplayName("Job Name")]
        public override string Name { get; set; } = string.Empty;

        [JsonIgnore]
        [DisplayName("Job Description")]
        public override string Description { get; set; } = string.Empty;

        public Job(string name) : this()
        {
            Name = name;
        }
        public Job()
        {
            Tasks = [];
            MediaCollection = [];
        }


        public ICollection<Media> MediaCollection { get; set; }

        /* Navigation */
        public virtual ICollection<JobTask> Tasks { get; set; }

        public override IEnumerable<Primitive> GetEnumerator()
        {
            return Tasks;
        }
    }
}
