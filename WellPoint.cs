
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WellTrajectoryPlot
{
    public class WellPointAndTrajectory
    {
        public string[,] WellTrajectoryGraph
        {
            get; set;
        }

        public List<double> WellPointX
        {
            get; set;
        }
        public List<double> WellPointY
        {
            get; set;
        }
        public List<double> WellPointZ
        {
            get; set;
        }

        public List<double> this [int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return WellPointX;
                    case 1: return WellPointY;
                    case 2: return WellPointZ;
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
        }
    }
}
