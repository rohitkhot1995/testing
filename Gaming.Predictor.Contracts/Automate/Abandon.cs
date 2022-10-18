using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Predictor.Contracts.Automate
{
    public class Abandon
    {

    }

    public class AbdPointPrcGet
    {
        public int TourGamedayId { get; set; }
        public int PhaseID { get; set; }
        public int MatchDay { get; set; }
        public int Tourid { get; set; }
        public List<Int32> TeamIds { get; set; }
    }
}
