

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Plot
{
    public static void InitializeAndPlotGraph(WellPointAndTrajectory myWell, string pointOfView, int rangeX, int rangeY, string filePath)
    {
        int[] adjustedWellPointX, adjustedWellPointZ, adjustedWellPointY;

        for (int i = 0; i < myWell.WellPointX.Count; i = i + 1)
        {
            myWell.WellPointX[i] = Plot.MeterToFeetTransmitor(myWell.WellPointX[i]);
            myWell.WellPointY[i] = Plot.MeterToFeetTransmitor(myWell.WellPointY[i]);
            myWell.WellPointZ[i] = Plot.MeterToFeetTransmitor(myWell.WellPointZ[i]);
        }

        double maxX = myWell.WellPointX.Max();
        double maxY = myWell.WellPointY.Max();
        double maxZ = myWell.WellPointZ.Max();

        double minX = myWell.WellPointX.Min();
        double minY = myWell.WellPointY.Min();
        double minZ = myWell.WellPointZ.Min();
        switch (pointOfView)
        {
            //XZ
            case "FrontViewXZ":
                adjustedWellPointX = ResizePointToFitPlot(myWell.WellPointX, rangeX, minX, maxX);
                adjustedWellPointZ = ResizePointToFitPlot(myWell.WellPointZ, rangeY, minZ, maxZ);
                myWell.WellTrajectoryGraph = PlotWellTrajectory(adjustedWellPointX, adjustedWellPointZ, myWell.WellPointX, myWell.WellPointY, myWell.WellPointZ, rangeX, rangeY);
                //myWell.WellTrajectoryGraph = PlotAixs(myWell.WellTrajectoryGraph, "x", "z", spaceForAxis);
                OutputData.PrintFileAsTxt(filePath, myWell.WellTrajectoryGraph);
                break;
            //yz
            case "EndViewYZ":
                adjustedWellPointY = ResizePointToFitPlot(myWell.WellPointY, rangeX, minY, maxY);
                adjustedWellPointZ = ResizePointToFitPlot(myWell.WellPointZ, rangeY, minZ, maxZ);
                myWell.WellTrajectoryGraph = PlotWellTrajectory(adjustedWellPointY, adjustedWellPointZ, myWell.WellPointX, myWell.WellPointY, myWell.WellPointZ, rangeX, rangeY);
                //myWell.WellTrajectoryGraph = PlotAixs(myWell.WellTrajectoryGraph, "y", "z", spaceForAxis);
                OutputData.PrintFileAsTxt(filePath, myWell.WellTrajectoryGraph);
                break;
            //xy
            case "VerticleViewXY":
                adjustedWellPointX = ResizePointToFitPlot(myWell.WellPointX, rangeX, minX, maxX);
                adjustedWellPointY = ResizePointToFitPlot(myWell.WellPointY, rangeY, minY, maxY);
                myWell.WellTrajectoryGraph = PlotWellTrajectory(adjustedWellPointX, adjustedWellPointY, myWell.WellPointX, myWell.WellPointY, myWell.WellPointZ, rangeX, rangeY);
                //myWell.WellTrajectoryGraph = PlotAixs(myWell.WellTrajectoryGraph, "x", "z", spaceForAxis);
                OutputData.PrintFileAsTxt(filePath, myWell.WellTrajectoryGraph);
                break;
        }
    }

    public static double MeterToFeetTransmitor(double meter)
    {
        return meter * 3.2808;
    }

    public static int[] ResizePointToFitPlot(List<double> originInput, int range, double minValue, double maxValue)
    {
        int spaceForAxis = 2;
        double delta = maxValue - minValue;
        double zoom = (double)range / delta;
        int[] resizedPoint = originInput.Select(v => (int)Math.Round((v - minValue) * zoom + spaceForAxis)).ToArray();

        return resizedPoint;
    }


    public static string[,] PlotWellTrajectory(int[] myX, int[] myY, List<double> coordinateX, List<double> coordinateY, List<double> coordinateZ, int rangeX, int rangeY)
    {
        int reservedSpace = 300;
        string[,] graph = new string[rangeY + reservedSpace, rangeX + reservedSpace];
        graph[myY[0], myX[0]] = @"=";
        string coordinate = "(" + Math.Round(coordinateX[0]) + "," + Math.Round(coordinateY[0]) + "," + Math.Round(coordinateZ[0]) + ")";
        int indexOfCoordinateY = myY[0] + 1;
        int indexOfCoordinateX = myX[0];

        foreach (char item in coordinate)
        {
            graph[indexOfCoordinateY, indexOfCoordinateX] = item.ToString();
            indexOfCoordinateX = indexOfCoordinateX + 1;
        }
        for (int i = 1; i < myY.Length - 1; i = i + 1)
        {
            int indexX = myX[i];
            int indexY = myY[i];
            if (graph[indexY, indexX] == null)
            {
                graph[indexY, indexX] = "+";
            }
            graph = ConnectingWellPoint(graph, myX[i - 1], myY[i - 1], indexX, indexY);

            coordinate = "(" + Math.Round(coordinateX[i], 1) + "," + Math.Round(coordinateY[i], 1) + "," + Math.Round(coordinateZ[i], 1) + ")";
            //coordinate = "(" + myX[i] + "," + myY[i] + ")";

            indexOfCoordinateY = indexY + 1;
            indexOfCoordinateX = indexX;
            foreach (char item in coordinate)
            {
                Console.WriteLine(item);
                graph[indexOfCoordinateY, indexOfCoordinateX] = item.ToString();
                indexOfCoordinateX = indexOfCoordinateX + 1;
            }
        }

        graph[myY[myY.Length - 1], myX[myY.Length - 1]] = @"#";
        graph = ConnectingWellPoint(graph, myX[myX.Length - 2], myY[myY.Length - 2], myX[myX.Length - 1], myY[myY.Length - 1]);
        coordinate = "(" + Math.Round(coordinateX[myY.Length - 1], 1) + "," + Math.Round(coordinateY[myY.Length - 1], 1) + "," + Math.Round(coordinateZ[myY.Length - 1], 1) + ")";
        indexOfCoordinateY = myY[myY.Length - 1] + 1;
        indexOfCoordinateX = myX[myY.Length - 1];
        foreach (char item in coordinate)
        {
            graph[indexOfCoordinateY, indexOfCoordinateX] = item.ToString();
            indexOfCoordinateX = indexOfCoordinateX + 1;
        }
        return graph;
    }


    public static string[,] ConnectingWellPoint(string[,] graph, int indexX, int indexY, int indexNextX, int indexNextY)
    {

        double differenceinX = indexNextX - indexX;
        double differenceinY = indexNextY - indexY;
        if (differenceinX == 0)
        {
            for (int i = Math.Min(indexY, indexNextY) + 1; i < Math.Max(indexY, indexNextY); i = i + 1)
            {
                if (graph[i, indexX] == null)
                {
                    graph[i, indexX] = @"|";
                }
            }
        }
        else if (differenceinY == 0)
        {
            for (int i = Math.Min(indexX, indexNextX) + 1; i < Math.Max(indexNextX, indexX); i = i + 1)
            {
                if (graph[i, indexX] == null)
                {
                    graph[indexY, i] = @"-";
                }
            }
        }
        else if (Math.Abs(differenceinX) >= Math.Abs(differenceinY))
        {
            double slope = differenceinY / differenceinX;
            int updateIndex;
            if (indexX > indexNextX)
            {
                int tempraryX = indexX;
                int tempraryY = indexY;
                indexX = indexNextX;
                indexY = indexNextY;
                indexNextX = tempraryX;
                indexNextY = tempraryY;
            }
            for (int i = indexX + 1; i < indexNextX; i = i + 1)
            {
                updateIndex = CalculateIndexThroughSlope(slope, i - indexX, indexY);
                if (graph[updateIndex, i] == null)
                {
                    graph[updateIndex, i] = ChooseSymbol(slope);
                }
            }
        }
        else
        {
            double slope = differenceinX / differenceinY;
            int updateIndex;
            if (indexY > indexNextY)
            {
                int tempraryX = indexX;
                int tempraryY = indexY;
                indexX = indexNextX;
                indexY = indexNextY;
                indexNextX = tempraryX;
                indexNextY = tempraryY;
            }
            for (int i = indexY + 1; i < indexNextY; i = i + 1)
            {
                updateIndex = CalculateIndexThroughSlope(slope, i - indexY, indexX);
                if (graph[i, updateIndex] == null)
                {
                    graph[i, updateIndex] = ChooseSymbol(slope);
                }
            }

        }
        return graph;
    }

    public static int CalculateIndexThroughSlope(double slope, double difference, int baseIndex)
    {
        return (int)Math.Round(difference * slope + baseIndex, MidpointRounding.AwayFromZero);
    }

    public static string ChooseSymbol(double slope)
    {
        const double pi = System.Math.PI;

        if (Math.Tan(170 * pi / 180) <= slope && slope < Math.Tan(80 * pi / 180))
        {
            return @"\";
        }
        else if (Math.Tan(100 * pi / 180) <= slope && slope < Math.Tan(170 * pi / 180))
        {
            return @"/";
        }
        else if (Math.Tan(170 * pi / 180) <= slope && slope <= Math.Tan(10 * pi / 180))
        {
            return @"|";
        }
        else
        {
            return @"-";
        }
    }
}









