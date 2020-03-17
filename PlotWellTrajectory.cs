using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace WellTrajectoryPlot
{
    public class Plot
    {
        public static void InitializeAndPlotGraph(WellPointAndTrajectory myWell, string viewName, int width, int height, string filePath, string unit)
        {
            int[] adjustedWellPointHorizonal = { }, adjustedWellPointVertical = { };
            List<double> WellPointHorizonal = new List<double>(), WellPointVertical = new List<double>();

            if (unit == "feet")
            {
                myWell.WellPointX = MeterToFeetTransmitor(myWell.WellPointX);
                myWell.WellPointY = MeterToFeetTransmitor(myWell.WellPointY);
                myWell.WellPointZ = MeterToFeetTransmitor(myWell.WellPointZ);
            }

            switch (viewName)
            {
                //XZ
                case "FrontViewXZ":
                    WellPointHorizonal = myWell[0].ToList();
                    WellPointVertical = myWell[2].ToList();

                    break;
                //yz
                case "EndViewYZ":
                    WellPointHorizonal = myWell[1].ToList();
                    WellPointVertical = myWell[2].ToList();
                    break;
                //xy
                case "VerticleViewXY":
                    WellPointHorizonal = myWell[0].ToList();
                    WellPointVertical = myWell[1].ToList();
                    break;
            }

            adjustedWellPointHorizonal = ResizePointToFitPlot(WellPointHorizonal, width);
            adjustedWellPointVertical = ResizePointToFitPlot(WellPointVertical, height);

            myWell.WellTrajectoryGraph = PlotWellTrajectory(adjustedWellPointHorizonal, adjustedWellPointVertical, WellPointHorizonal, WellPointVertical, width, height);

            AddCoordinateNotationForLineNode(adjustedWellPointHorizonal, adjustedWellPointVertical, myWell);

            OutputData.PrintFileAsTxt(filePath, myWell.WellTrajectoryGraph);
        }

        public static List<double> MeterToFeetTransmitor(List<double> point)
        {
            for (int i = 0; i < point.Count; i++)
            {
                point[i] = point[i] * 3.2808;
            }
            return point;
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

        public static int GetLineNodeIndexWithLargestCurvity(List<double> coordinateX, List<double> coordinateY)
        {

            List<double> slopes = new List<double>();
            int result = 0;
            double maxCurvity = Math.PI;

            for (int i = 1; i < coordinateX.Count - 1; i++)
            {
                double side1 = Math.Sqrt(Math.Pow(coordinateX[i] - coordinateX[i - 1], 2) + Math.Pow(coordinateY[i] - coordinateY[i - 1], 2));
                double side2 = Math.Sqrt(Math.Pow(coordinateX[i] - coordinateX[i + 1], 2) + Math.Pow(coordinateY[i] - coordinateY[i + 1], 2));

                if (side1 == 0 || side2 == 0)
                {
                    continue;
                }

                double side3 = Math.Sqrt(Math.Pow(coordinateX[i - 1] - coordinateX[i + 1], 2) + Math.Pow(coordinateY[i - 1] - coordinateY[i + 1], 2));

                double cosPoint = Math.Acos((Math.Pow(side1, 2) + Math.Pow(side2, 2) - Math.Pow(side3, 2)) / (2 * side1 * side2));

                // Console.WriteLine("{0}: {1} {2} {3} {4} | {5}", i, side1, side2, side3, (side1 + side2 == side3), cosPoint);

                if (cosPoint <= maxCurvity)
                {
                    result = i;
                    maxCurvity = cosPoint;
                }
            }

            // Console.WriteLine(result);

            return result;
        }

        public static void AddCoordinateNotationForLineNode(int[] myX, int[] myY, WellPointAndTrajectory myWell)
        {
            string[,] graph = myWell.WellTrajectoryGraph;

            for (int i = 0; i < myX.Length; i++)
            {
                string coordinate = "(" + Math.Round(myWell[0][i]) + "," + Math.Round(myWell[1][i]) + "," + Math.Round(myWell[2][i]) + ")";
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

        public static string[,] PlotWellTrajectory(int[] myX, int[] myY, List<double> WellPointHorizonal, List<double> WellPointVertical, int width, int height)
        {
            int reservedSpace = 300;
            string[,] graph = new string[height + reservedSpace, width + reservedSpace];

            int indexToLabelo = GetLineNodeIndexWithLargestCurvity(WellPointHorizonal, WellPointVertical);

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
            graph[myY[indexToLabelo], myX[indexToLabelo]] = @"o";

            return graph;

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







