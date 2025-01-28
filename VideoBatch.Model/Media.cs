using System.ComponentModel;
// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{
    public class Media
    {
        public string Name { get; set; } = String.Empty;
        public string Path { get; set; } = String.Empty;
        public MetaData MediaInfo { get; set; } = new MetaData();

        [DisplayName("Job")]
        public Guid JobID { get; set; }
    }
}