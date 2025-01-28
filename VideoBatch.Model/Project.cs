using System.ComponentModel;
using System.Text.Json.Serialization;

// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{
    public class Project : Primitive
    {

        [DisplayName("Project Name")]
        public override string Name { get; set; } = string.Empty;

        public Dictionary<string, object> MetaData { get; set; } = [];


        [JsonIgnore]
        [DisplayName("Project Description")]
        public override string Description { get; set; } = string.Empty;


        public Project(string name) : this()
        {
            Name = name;
        }
        public Project()
        {
            Jobs = [];
        }
        /* Navigation */
        public virtual ICollection<Job> Jobs { get; set; } = [];

        public override IEnumerable<Primitive> GetEnumerator()
        {
            return Jobs;
        }
    }
}
