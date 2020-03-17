using PlotWellTrajectory;
using Import;
using WellPoint;
public class MainProgram
{
    public static void Main()
    {
        string[] pointOfView = new string[] { "FrontViewXZ", "EndViewYZ", "VerticleViewXY" };
        for (int i = 1; i < 6; i = i + 1)
        {
            foreach (string point in pointOfView)
            {
                string importFilePath = @"G:\HW5\HW5\\" + i + ".csv";
                WellPointAndTrajectory myWellData = ImportData.ReadFile(importFilePath);
                string filePath = @"G:\HW5\HW5\\" + i + point + ".txt";
                Plot.InitializeAndPlotGraph(myWellData, point, 150, 200, filePath);
            }
        }
    }
}



