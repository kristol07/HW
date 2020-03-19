
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace WellTrajectoryPlot
{
    public class OutputData
    {

        public static int PrintFileAsTxt(string filePath, string[,] graph)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    for (int i = 0; i < graph.GetLength(0); i = i + 1)
                    {
                        for (int j = 0; j < graph.GetLength(1); j = j + 1)
                        {
                            writer.Write(graph[i, j]);
                        }
                        writer.Write("\r\n");
                    }
                    return 0;
                }

            }
            catch
            {
                return -1;
            }

            // The following conditions may cause an exception:
            //     - The file exists and is read-only.
            //     - The path name may be too long.
            //     - The disk may be full.

        }
    }
}