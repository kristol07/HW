
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WellTrajectoryPlot
{
    public class ImportData
    {
        public static WellPointAndTrajectory ReadFile(string filePath)
        {
            WellPointAndTrajectory myWell = new WellPointAndTrajectory();
            List<double> myWellPointX = new List<double>();
            List<double> myWellPointY = new List<double>();
            List<double> myWellPointZ = new List<double>();

            try
            {
                using (StreamReader myStreamReader = new StreamReader(filePath))
                {
                    String[] onePoint;
                    while (!myStreamReader.EndOfStream)
                    {
                        onePoint = myStreamReader.ReadLine().Split(',');
                        double myX = Double.Parse(onePoint[0]);
                        double myY = Double.Parse(onePoint[1]);
                        double myZ = Double.Parse(onePoint[2]);

                        myWellPointX.Add(myX);
                        myWellPointY.Add(myY);
                        myWellPointZ.Add(myZ);
                    }
                }
                myWell.WellPointX = myWellPointX;
                myWell.WellPointY = myWellPointY;
                myWell.WellPointZ = myWellPointZ;

                return myWell;
            }
            catch (IOException)
            {
                Console.WriteLine("Import Failed! The file includes empty entry.");
                return null;
            }
        }
    }
}
