// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{
    public class MetaData
    {
        public DateTime CreateDate { get; set; }
        public long FileSize { get; set; }
        public long Duration { get; set; }
        public string Resolution { get; set; } = string.Empty;
        public float FPS { get; set; }
        public long BitRateKbs { get; set; }

        public MetaData()
        {

        }
    }
}
