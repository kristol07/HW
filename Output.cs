
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace WellTrajectoryPlot
{
    public class OutputData
    {

        public static void PrintFileAsTxt(string filePath, string[,] graph)
        {
            StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write));
            for (int i = 0; i < graph.GetLength(0); i = i + 1)
            {

                for (int j = 0; j < graph.GetLength(1); j = j + 1)
                {
                    if (graph[i, j] == null)
                    {
                        writer.Write(" ");
                    }
                    else
                    {
                        writer.Write(graph[i, j]);
                    }
                }
                writer.Write("\r\n");
            }
            writer.Flush();
            writer.Close();
        }
    }
}