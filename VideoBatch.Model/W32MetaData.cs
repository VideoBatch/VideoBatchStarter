// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Aug 2021

namespace VideoBatch.Model
{

    public struct W32MetaData
    {
        public UInt64? MediaDuration;
        public UInt32? VideoFrameHeight;
        public UInt32? VideoFrameWidth;
        public UInt32? VideoFrameRate;
        public UInt16? VideoStreamNumber; //number of streams
        public UInt32? VideoTotalBitrate;
        public UInt32? VideoEncodingBitrate;
    }

}