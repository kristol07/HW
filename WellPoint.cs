
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WellTrajectoryPlot
{

    public enum DistanceUnit
    {
        Meter,
        Feet
    }

    public class WellPointAndTrajectory
    {
        public const double FeetToMeter = 0.3048;

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

        public WellPointAndTrajectory()
        {
            WellPointX = new List<double>();
            WellPointY = new List<double>();
            WellPointZ = new List<double>();
            // WellTrajectoryGraph = new string[,]{};
        }

        public void AddPoint(double x, double y, double z, DistanceUnit unit = DistanceUnit.Meter)
        {
            double convertion = 1.0;
            if (unit == DistanceUnit.Feet)
            {
                convertion = FeetToMeter;
            }

            WellPointX.Add(x * convertion);
            WellPointY.Add(y * convertion);
            WellPointZ.Add(z * convertion);
        }

        public WellPointAndTrajectory OutputForUnitConversion(DistanceUnit unit)
        {
            WellPointAndTrajectory newWell = new WellPointAndTrajectory();

            if (unit == DistanceUnit.Feet && WellPointX.Count != 0)
            {
                for (int i = 0; i < WellPointX.Count; i++)
                {
                    newWell.WellPointX.Add(this.WellPointX[i] / FeetToMeter);
                    newWell.WellPointY.Add(this.WellPointY[i] / FeetToMeter);
                    newWell.WellPointZ.Add(this.WellPointZ[i] / FeetToMeter);
                }
            }

            return newWell;
        }

        public List<int> GetLineNodeIndexWithLargestCurvity()
        {
            List<int> result = new List<int>();

            double[] vector1 = new double[3], vector2 = new double[3];

            double maxCurvity = -2;

            for (int i = 1; i < WellPointX.Count - 1; i++)
            {
                vector1[0] = WellPointX[i - 1] - WellPointX[i]; vector1[1] = WellPointY[i - 1] - WellPointY[i]; vector1[2] = WellPointZ[i - 1] - WellPointZ[i];
                vector2[0] = WellPointX[i + 1] - WellPointX[i]; vector2[1] = WellPointY[i + 1] - WellPointY[i]; vector2[2] = WellPointZ[i + 1] - WellPointZ[i];

                double norm1 = Math.Sqrt(Math.Pow(vector1[0], 2) + Math.Pow(vector1[1], 2) + Math.Pow(vector1[2], 2));
                double norm2 = Math.Sqrt(Math.Pow(vector2[0], 2) + Math.Pow(vector2[1], 2) + Math.Pow(vector2[2], 2));

                double cosAngle;
                if (norm1 == 0 || norm2 == 0)
                {
                    cosAngle = -2;
                    continue;
                }

                cosAngle = (vector1[0] * vector2[0] + vector1[1] * vector2[1] + vector1[2] * vector2[2]) / (norm1 * norm2);

                if (cosAngle >= maxCurvity)
                {
                    if (cosAngle > maxCurvity)
                    {
                        result.Clear();
                    }
                    result.Add(i);
                    maxCurvity = cosAngle;
                }
            }

            return result;
        }

        public List<double> this[int index]
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
