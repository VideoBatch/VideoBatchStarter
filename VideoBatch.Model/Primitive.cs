using NodaTime;

// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{
    public abstract class Primitive
    {
        public Guid ID { get; set; }
        public Guid ParentID { get; set; }
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public LocalDateTime DateCreated { get; set; } = Zone.UKClock.GetCurrentLocalDateTime();
        public LocalDateTime DateUpdated { get; set; } = Zone.UKClock.GetCurrentLocalDateTime();  // to be updated on commit
        public abstract IEnumerable<Primitive> GetEnumerator();
        public override string ToString()
        {
            return $"{Name} - Created: {DateCreated:yyyy-MMM-dd}";
        }
    }
}