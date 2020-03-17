
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WellPoint
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
    }
}
