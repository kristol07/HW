
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
            try
            {
                using (StreamReader myStreamReader = new StreamReader(filePath)) // filepath invalid
                {
                    WellPointAndTrajectory myWell = new WellPointAndTrajectory();
                    DistanceUnit unit;
                    bool parsingResult = Enum.TryParse(myStreamReader.ReadLine(), out unit);
                    if(parsingResult == false)
                    {
                        Console.WriteLine("Unit unrecognized. Check first line in the file");
                        return null;
                    }

                    String[] onePoint;
                    while (!myStreamReader.EndOfStream)
                    {
                        onePoint = myStreamReader.ReadLine().Split(',');
                        double myX = Double.Parse(onePoint[0]);  // parse error
                        double myY = Double.Parse(onePoint[1]);
                        double myZ = Double.Parse(onePoint[2]);  // index error if data loss
                        myWell.AddPoint(myX, myY, myZ, unit);
                    }

                    return myWell;
                }
            }
            catch (FileNotFoundException ex)
            {
                // throw new FileNotFoundException($"Invalid filepath, no such file found.", ex);
                Console.WriteLine($"Invalid filepath, no such file found.\n{ex.ToString()}");
                return null;
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Point data loss, check each line has 3 coordinates.\n{ex.ToString()}");
                return null;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Point data parse error, check that all are double values.\n{ex.ToString()}");
                return null;
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
