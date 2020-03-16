
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class WellData
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

public class Import
{
    //remember to change the well datat to well point?
    public static WellData ReadFile(string filePath)
    {
        WellData myWell = new WellData();
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


public class Plot
{

    public static void InitializeAndPlotGraph(WellData myWell, string pointOfView, int rangeX, int rangeY, string filePath)
    {
        double maxX = myWell.WellPointX.Max();
        double maxY = myWell.WellPointY.Max();
        double maxZ = myWell.WellPointZ.Max();

        double minX = myWell.WellPointX.Min();
        double minY = myWell.WellPointY.Min();
        double minZ = myWell.WellPointZ.Min();

        int[] adjustedWellPointX, adjustedWellPointZ, adjustedWellPointY;

        switch (pointOfView)
        {
            //XZ
            case "FrontView":
                adjustedWellPointX = ResizePointToFitPlot(myWell.WellPointX, rangeX, minX, maxX);
                adjustedWellPointZ = ResizePointToFitPlot(myWell.WellPointZ, rangeY, minZ, maxZ);
                myWell.WellTrajectoryGraph = PlotWellTrajectory(adjustedWellPointX, adjustedWellPointZ, myWell.WellPointX, myWell.WellPointY, myWell.WellPointZ, rangeX, rangeY);
                //myWell.WellTrajectoryGraph = PlotAixs(myWell.WellTrajectoryGraph, "x", "z", spaceForAxis);
                Output.PrintFileAsTxt(filePath, myWell.WellTrajectoryGraph);
                break;
            //yz
            case "EndView":
                adjustedWellPointY = ResizePointToFitPlot(myWell.WellPointY, rangeX, minY, maxY);
                adjustedWellPointZ = ResizePointToFitPlot(myWell.WellPointZ, rangeY, minZ, maxZ);
                myWell.WellTrajectoryGraph = PlotWellTrajectory(adjustedWellPointY, adjustedWellPointZ, myWell.WellPointX, myWell.WellPointY, myWell.WellPointZ, rangeX, rangeY);
                //myWell.WellTrajectoryGraph = PlotAixs(myWell.WellTrajectoryGraph, "y", "z", spaceForAxis);
                Output.PrintFileAsTxt(filePath, myWell.WellTrajectoryGraph);
                break;
            //xy
            case "VerticleView":
                adjustedWellPointX = ResizePointToFitPlot(myWell.WellPointX, rangeX, minX, maxX);
                adjustedWellPointY = ResizePointToFitPlot(myWell.WellPointY, rangeY, minY, maxY);
                myWell.WellTrajectoryGraph = PlotWellTrajectory(adjustedWellPointX, adjustedWellPointY, myWell.WellPointX, myWell.WellPointY, myWell.WellPointZ, rangeX, rangeY);
                //myWell.WellTrajectoryGraph = PlotAixs(myWell.WellTrajectoryGraph, "x", "z", spaceForAxis);
                Output.PrintFileAsTxt(filePath, myWell.WellTrajectoryGraph);
                break;
        }
    }

    public static int[] ResizePointToFitPlot(List<double> originInput, int range, double minValue, double maxValue)
    {
        int spaceForAxis = 2;
        double zoom = (maxValue - minValue) / (double)range;
        int[] resizedPoint = originInput.Select(v => (int)Math.Round((v - minValue) / zoom + spaceForAxis)).ToArray();
        return resizedPoint;
    }

    //IGNORE THIS FOR NOW 
    /*
        public static string[,] PlotAixs(string[,] graph, string axis1, string axis2, int spaceForAxis)
        {
            graph[0, 2] = axis1;
            graph[0, 4] = ">";
            graph[2, 0] = axis1;
            graph[4, 0] = "v";

            graph[spaceForAxis, spaceForAxis + 2] = axis2;
            for (int i = 0; i < graph.GetLength(0); i = i + 1)
            {

                if (graph[i, 0] == null)
                {
                    graph[i, 0] = @"|";
                }

            }
            for (int j = spaceForAxis; j < graph.GetLength(1); j = j + 1)
            {
                if (graph[0, j] == null)
                {
                    graph[0, j] = @"-";
                }
            }
            return graph;
        }
        public static int[] FindOriginalPoint(int minValueX, int maxValueX, int maxValueY, int minValueY, int rangeX, int rangeY, int spaceForAxis)
        {
            int[] originalPoint = new int[2];
            double zoom1 = (maxValueX - minValueX) / (double)rangeX;
            double zoom2 = (maxValueY - minValueY) / (double)rangeY;
            originalPoint[0] = (int)Math.Round((double)minValueX / zoom1) + spaceForAxis;
            originalPoint[1] = (int)Math.Round((double)minValueY / zoom2) + spaceForAxis;
            return originalPoint;

        }

    */

    public static string[,] PlotWellTrajectory(int[] myX, int[] myY, List<double> coordinateX, List<double> coordinateY, List<double> coordinateZ, int rangeX, int rangeY)
    {
        int reservedSpace = 80;
        string[,] graph = new string[rangeY + reservedSpace, rangeX + reservedSpace];
        graph[myY[0], myX[0]] = @"+";
        string coordinate = "(" + coordinateX[0] + "," + coordinateY[0] + "," + coordinateZ[0] + ")";
        int indexOfCoordinateY = myY[0];
        int indexOfCoordinateX = myX[0] + 1;
        foreach (char item in coordinate)
        {
            graph[indexOfCoordinateY, indexOfCoordinateX] = item.ToString();
            indexOfCoordinateX = indexOfCoordinateX + 1;
        }
        for (int i = 1; i < myY.Length; i = i + 1)
        {
            int indexX = myX[i];
            int indexY = myY[i];
            graph[indexY, indexX] = "+";
            graph = ConnectingWellPoint(graph, myX[i - 1], myY[i - 1], indexX, indexY);

            coordinate = "(" + coordinateX[i] + "," + coordinateY[i] + "," + coordinateZ[i] + ")";
            indexOfCoordinateY = indexY;
            indexOfCoordinateX = indexX + 1;
            foreach (char item in coordinate)
            {
                Console.WriteLine(item);
                graph[indexOfCoordinateY, indexOfCoordinateX] = item.ToString();
                indexOfCoordinateX = indexOfCoordinateX + 1;
            }
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
        return (int)Math.Round(difference * slope + baseIndex);
    }

    public static string ChooseSymbol(double slope)
    {
        //10 degree to 80 degree, -100 degree to -170 degree
        if (0.176 < slope && slope < 5.671282)
        {
            return @"\";
        }
        //100 degree to 170 degree, -10 degree to -80 degree
        else if (-5.671282 < slope && slope < -0.176327)
        {
            return @"/";
        }
        else if (-0.176327 < slope && slope < 0.176327)
        {
            return @"|";
        }
        //10 degree to -10 degree, 170 degree to -170 degree
        else
        {
            return @"-";
        }
    }
}


public class Output
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

public class Program
{
    public static void Main()
    {

        string[] pointOfView = new string[] { "FrontView", "EndView", "VerticleView" };
        for (int i = 5; i < 6; i = i + 1)
        {
            string importFilePath = @"G:\HW5\HW5\\" + i + ".csv";

            WellData myWellData = Import.ReadFile(importFilePath);
            foreach (string point in pointOfView)
            {
                string filePath = @"G:\HW5\HW5\\" + i + point + ".txt";
                Plot.InitializeAndPlotGraph(myWellData, point, 150, 200, filePath);
            }
        }
    }
}







