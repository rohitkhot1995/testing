﻿using System;
using System.Collections.Generic;

namespace Gaming.Predictor.Contracts.BackgroundServices
{
    public class LockList
    {
        public List<Int32> MatchIdList { get; set; }
        public List<Int32> MatchdayIdList { get; set; }
    }
}
