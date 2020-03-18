using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace WellTrajectoryPlot
{
    public class Plot
    {
        public static string[,] InitGraph(int width, int height)
        {

            int reservedSpace = 300;
            string[,] initGraph = new string[height + reservedSpace, width + reservedSpace];

            return initGraph;
        }

        public static int[] ResizePointToFitPlot(List<double> pointToResize, int range)
        {
            double minValue = pointToResize.Min();
            double maxValue = pointToResize.Max();

            int spaceForAxis = 2;
            double delta = maxValue - minValue;
            double zoom = (double)range / delta;
            int[] resizedPoint = pointToResize.Select(v => (int)Math.Round((v - minValue) * zoom + spaceForAxis)).ToArray();

            return resizedPoint;
        }

        public static List<int[]> RizeLinePointToFitPlot(string view, int width, int height, WellPointAndTrajectory myWell)
        {
            int xIndex = -1, yIndex = -1;

            switch (view)
            {
                case "FrontViewXZ":
                    xIndex = 0;
                    yIndex = 2;
                    break;
                case "EndViewYZ":
                    xIndex = 1;
                    yIndex = 2;
                    break;
                case "VerticleViewXY":
                    xIndex = 0;
                    yIndex = 1;
                    break;
            }

            List<double> WellPointHorizonal = myWell[xIndex];
            List<double> WellPointVertical = myWell[yIndex];

            int[] myX = ResizePointToFitPlot(WellPointHorizonal, width);
            int[] myY = ResizePointToFitPlot(WellPointVertical, height);

            List<int[]> rizeLine = new List<int[]>();
            rizeLine.Add(myX);
            rizeLine.Add(myY);

            return rizeLine;

        }

        public static void PlotWellTrajectory(string view, WellPointAndTrajectory myWell, int width, int height)
        {

            string[,] graph = InitGraph(width, height);

            List<int[]> rizeLine = RizeLinePointToFitPlot(view, width, height, myWell);
            int[] myX = rizeLine[0];
            int[] myY = rizeLine[1];

            List<int> indexWithLargestCurvity = myWell.GetLineNodeIndexWithLargestCurvity();

            for (int i = 1; i < myY.Length; i = i + 1)
            {
                int indexX = myX[i];
                int indexY = myY[i];
                if (graph[indexY, indexX] == null)
                {
                    graph[indexY, indexX] = "+";
                }
                ConnectingWellPoint(ref graph, myX[i - 1], myY[i - 1], indexX, indexY);
            }

            graph[myY[0], myX[0]] = @"=";
            graph[myY[myY.Length - 1], myX[myY.Length - 1]] = @"#";
            foreach (var i in indexWithLargestCurvity)
            {
                graph[myY[i], myX[i]] = @"o";
            }

            myWell.WellTrajectoryGraph = graph;

            AddCoordinateNotationForLineNode(myX, myY, myWell);
        }

        public static void AddCoordinateNotationForLineNode(int[] myX, int[] myY, WellPointAndTrajectory myWell)
        {
            string[,] graph = myWell.WellTrajectoryGraph;

            for (int i = 0; i < myX.Length; i++)
            {
                string coordinate = "(" + Math.Round(myWell[0][i], 1) + "," + Math.Round(myWell[1][i], 1) + "," + Math.Round(myWell[2][i], 1) + ")";
                int indexOfCoordinateY = myY[i] + 1;
                int indexOfCoordinateX = myX[i];

                foreach (char item in coordinate)
                {
                    graph[indexOfCoordinateY, indexOfCoordinateX] = item.ToString();
                    indexOfCoordinateX = indexOfCoordinateX + 1;
                }
            }

            myWell.WellTrajectoryGraph = graph;
        }

        public static void ConnectingWellPoint(ref string[,] graph, int indexX, int indexY, int indexNextX, int indexNextY)
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
                    if (graph[indexY, i] == null)
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
                        graph[i, updateIndex] = ChooseSymbol(1 / slope);
                    }
                }

            }
        }

        public static int CalculateIndexThroughSlope(double slope, double difference, int baseIndex)
        {
            return (int)Math.Round(difference * slope + baseIndex, MidpointRounding.AwayFromZero);
        }

        public static string ChooseSymbol(double slope)
        {

            double tangent10Degree = Math.Tan(DegreeToRadian(10));
            double tangent80Degree = Math.Tan(DegreeToRadian(80));
            double tangent100Degree = Math.Tan(DegreeToRadian(100));
            double tangent170Degree = Math.Tan(DegreeToRadian(170));

            if (slope > 0)
            {
                if (slope > tangent10Degree && slope < tangent80Degree)
                {
                    return @"\";
                }
                else if (slope >= tangent80Degree)
                {
                    return @"|";
                }
                else
                {
                    return @"-";
                }
            }
            else
            {
                if (slope > tangent100Degree && slope < tangent170Degree)
                {
                    return @"/";
                }
                else if (slope >= tangent170Degree)
                {
                    return @"-";
                }
                else
                {
                    return @"|";
                }
            }
        }

        public static double DegreeToRadian(double degree)
        {
            const double pi = System.Math.PI;
            return degree * pi / 180.0;
        }
    }



}







