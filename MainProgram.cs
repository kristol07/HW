using WellTrajectoryPlot;

public class MainProgram
{
    public static void Main()
    {
        string[] pointOfView = new string[] { "FrontViewXZ" }; // "FrontViewXZ", "EndViewYZ", , "EndViewYZ", "VerticleViewXY"
        for (int i = 8; i <= 8; i = i + 1)
        {
            foreach (string point in pointOfView)
            {
                string importFilePath = "data\\" + i + ".csv";
                WellPointAndTrajectory myWellData = ImportData.ReadFile(importFilePath);
                string filePath = "data\\" + i + point + ".txt";
                string unit = "feet";
                Plot.InitializeAndPlotGraph(myWellData, point, 150, 200, filePath, unit);
            }
        }
    }
}



