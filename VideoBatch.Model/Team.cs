using System.ComponentModel;
using System.Text.Json.Serialization;

namespace VideoBatch.Model
{
    public class Team : Primitive
    {
        [DisplayName("Team Name")]
        public override string Name { get; set; } = string.Empty;

        [JsonIgnore]
        [DisplayName("Team Description")]
        public override string Description { get; set; } = string.Empty;

        public Team(string name) : this()
        {
            Name = name;
        }
        public Team()
        {
            Projects = new List<Project>();
        }
        /* Navigation */
        public virtual ICollection<Project> Projects { get; set; }

        public override IEnumerable<Primitive> GetEnumerator()
        {
            return Projects;
        }
    }
}
