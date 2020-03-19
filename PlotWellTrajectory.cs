using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace WellTrajectoryPlot
{
    public class Plot
    {
        public const double DegreeToRadian = Math.PI / 180.0;

        public string[,] Graph
        {
            get; set;
        }

        public int Width
        {
            get;
        }

        public int Height
        {
            get;
        }

        public Plot(int width, int height)
        {
            int reservedSpace = 300;
            Graph = new string[height + reservedSpace, width + reservedSpace];
            for (int i = 0; i < height + reservedSpace; i++)
            {
                for (int j = 0; j < width + reservedSpace; j++)
                {
                    Graph[i, j] = " ";
                }
            }

            this.Width = width;
            this.Height = height;
        }

        public int[] ResizePointToFitPlot(List<double> pointToResize, int range)
        {
            double minValue = pointToResize.Min();
            double maxValue = pointToResize.Max();

            int spaceForAxis = 2;
            double delta = maxValue - minValue;
            double zoom = (double)range / delta;
            int[] resizedPoint = pointToResize.Select(v => (int)Math.Round((v - minValue) * zoom + spaceForAxis)).ToArray();

            return resizedPoint;
        }

        public List<int[]> RizeLinePointToFitPlot(string view, WellPointAndTrajectory myWell)
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

            int[] myX = ResizePointToFitPlot(myWell[xIndex], Width);
            int[] myY = ResizePointToFitPlot(myWell[yIndex], Height);

            List<int[]> rizeLine = new List<int[]>();
            rizeLine.Add(myX);
            rizeLine.Add(myY);

            return rizeLine;
        }

        public void PlotWellTrajectory(string view, WellPointAndTrajectory myWell)
        {
            List<int[]> rizeLine = RizeLinePointToFitPlot(view, myWell);
            int[] myX = rizeLine[0];
            int[] myY = rizeLine[1];

            List<int> indexWithLargestCurvity = myWell.GetLineNodeIndexWithLargestCurvity();

            for (int i = 1; i < myY.Length; i = i + 1)
            {
                if (Graph[myY[i], myX[i]] == " ")
                {
                    Graph[myY[i], myX[i]] = "+";
                }
                ConnectingWellPoint(myX[i - 1], myY[i - 1], myX[i], myY[i]);
            }

            Graph[myY[0], myX[0]] = @"=";
            Graph[myY[^1], myX[^1]] = @"#";
            foreach (var i in indexWithLargestCurvity)
            {
                Graph[myY[i], myX[i]] = @"o";
            }

            AddCoordinateNotationForLineNode(myX, myY, myWell);
        }

        public void AddCoordinateNotationForLineNode(int[] myX, int[] myY, WellPointAndTrajectory myWell)
        {
            for (int i = 0; i < myX.Length; i++)
            {
                string coordinate = "(" + Math.Round(myWell[0][i], 1) + "," + Math.Round(myWell[1][i], 1) + "," + Math.Round(myWell[2][i], 1) + ")";
                int indexOfCoordinateY = myY[i] + 1;
                int indexOfCoordinateX = myX[i];

                foreach (char item in coordinate)
                {
                    Graph[indexOfCoordinateY, indexOfCoordinateX] = item.ToString();
                    indexOfCoordinateX = indexOfCoordinateX + 1;
                }
            }
        }

        public void ConnectingWellPoint(int indexX, int indexY, int indexNextX, int indexNextY)
        {

            double differenceinX = indexNextX - indexX;
            double differenceinY = indexNextY - indexY;
            if (differenceinX == 0)
            {
                for (int i = Math.Min(indexY, indexNextY) + 1; i < Math.Max(indexY, indexNextY); i = i + 1)
                {
                    if (Graph[i, indexX] == " ")
                    {
                        Graph[i, indexX] = @"|";
                    }
                }
            }
            else if (differenceinY == 0)
            {
                for (int i = Math.Min(indexX, indexNextX) + 1; i < Math.Max(indexNextX, indexX); i = i + 1)
                {
                    if (Graph[indexY, i] == " ")
                    {
                        Graph[indexY, i] = @"-";
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
                    if (Graph[updateIndex, i] == " ")
                    {
                        Graph[updateIndex, i] = ChooseSymbol(slope);
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
                    if (Graph[i, updateIndex] == " ")
                    {
                        Graph[i, updateIndex] = ChooseSymbol(1 / slope);
                    }
                }

            }
        }

        public int CalculateIndexThroughSlope(double slope, double difference, int baseIndex)
        {
            return (int)Math.Round(difference * slope + baseIndex, MidpointRounding.AwayFromZero);
        }

        public string ChooseSymbol(double slope)
        {

            double tangent10Degree = Math.Tan(DegreeToRadian * 10);
            double tangent80Degree = Math.Tan(DegreeToRadian * 80);
            double tangent100Degree = Math.Tan(DegreeToRadian * 100);
            double tangent170Degree = Math.Tan(DegreeToRadian * 170);

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

    }
}







