using NodaTime;
using System.ComponentModel;

// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{
    public class Account : Primitive
    {
        [DisplayName("Account Name")]
        public override string Name { get; set; } = string.Empty;

        [DisplayName("Account Description")]
        public override string Description { get; set; } = string.Empty;

        // All Accounts must have a TimeZone or we get in trouble later. 
        // Default set to UKZone and we can change in startup or settings
        public DateTimeZone TimeZone { get; set; } = Zone.UKZone;


        public Account(string name) : this()
        {
            Name = name;
        }
        public Account()
        {
            Teams = [];
        }

        /* Navigation */
        public virtual ICollection<Team> Teams { get; set; }

        public override IEnumerable<Primitive> GetEnumerator()
        {
            return Teams;
        }
    }
}