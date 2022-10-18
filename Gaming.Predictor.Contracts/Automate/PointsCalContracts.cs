using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Predictor.Contracts.Automate
{
    public class Matchdays
    {
        public Int32 GamedayId { get; set; }
        public Int32 PhaseId { get; set; }
        public Int32 Matchday { get; set; }
        public List<Int32> TeamIds { get; set; }
    }

    #region " More-Or-Less "

    public class MOLPointTrigger
    {
        public int RetVal { get; set; }
        public int TourId { get; set; }
        public int WeekId { get; set; }
        public int GameDayId { get; set; }
    }

    #endregion
}
