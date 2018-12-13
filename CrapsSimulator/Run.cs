using System.Collections.Generic;

namespace CrapsSimulator
{
    public class Run
    {
        public Roll Roll { get; set; }
        public bool IsCraps { get; set; }
        public bool PointOn { get; set; }
        public bool SevensOut { get; set; }
        public int Point { get; set; }
        public int TotalPointsHit { get; set; }
        public List<Roll> AllRoles { get; set; }
        public int RunNumber { get; set; }
    }

    public class Roll
    {
        public int DieOne { get; set; }
        public int DieTwo { get; set; }
        public int DiceTotal => DieOne + DieTwo;
        public bool RollIsPointHit { get; set; }
    }
}
