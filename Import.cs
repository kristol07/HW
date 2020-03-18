
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WellTrajectoryPlot
{
    public class ImportData
    {
        public static int ReadFile(string filePath, WellPointAndTrajectory myWell)
        {
            try
            {
                using (StreamReader myStreamReader = new StreamReader(filePath))
                {
                    DistanceUnit unit;
                    Enum.TryParse(myStreamReader.ReadLine(), out unit);

                    String[] onePoint;
                    while (!myStreamReader.EndOfStream)
                    {
                        onePoint = myStreamReader.ReadLine().Split(',');
                        double myX = Double.Parse(onePoint[0]);
                        double myY = Double.Parse(onePoint[1]);
                        double myZ = Double.Parse(onePoint[2]);
                        myWell.AddPoint(myX, myY, myZ, unit);
                    }

                    return 0;
                }
            }
            catch (IOException)
            {
                return 1;
            }
            catch (ArgumentException)
            {
                return -1;
            }
            catch
            {
                return -2;
            }
        }
    }
}
