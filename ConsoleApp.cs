using System;

namespace WellTrajectoryPlot
{
    public class ConsoleApp
    {

        public void ConsoleOutputForReadingProcess(int readResult, string filePath)
        {
            switch (readResult)
            {
                case 0:
                    Console.WriteLine("Read file {0} successfully...", filePath);
                    break;
                case 1:
                    Console.WriteLine("Import Failed! The file {0} includes empty entry.", filePath);
                    break;
                case -1:
                    Console.WriteLine("Unit unrecognized.");
                    break;
                case -2:
                    Console.WriteLine("Read error for {0}", filePath);
                    break;
            }
        }

        public void ConsoleOutputForSavingProcess(int saveResult, string view)
        {
            switch (saveResult)
            {
                case 0:
                    Console.WriteLine("Saving plot for {0} successfully...", view);
                    break;
                case -1:
                    Console.WriteLine("Saving plot error for {0}.", view);
                    break;
            }
        }

        public void Run()
        {
            string[] pointOfView = new string[] { "FrontViewXZ", "EndViewYZ", "VerticleViewXY" }; // "FrontViewXZ", "EndViewYZ", 
            for (int i = 1; i <= 8; i = i + 1)
            {
                WellPointAndTrajectory myWell = new WellPointAndTrajectory();
                string importFilePath = "data/" + i + ".csv";

                int readResult = ImportData.ReadFile(importFilePath, myWell);
                ConsoleOutputForReadingProcess(readResult, importFilePath);
                if (readResult != 0)
                {
                    return;
                }

                DistanceUnit unit = DistanceUnit.Feet;
                WellPointAndTrajectory newWell = myWell.OutputForUnitConversion(unit);

                foreach (string view in pointOfView)
                {
                    int width = 150, height = 200;
                    Plot.PlotWellTrajectory(view, newWell, width, height);

                    string outputFilePath = "data/" + i + "-" + view + ".txt";
                    int saveResult = OutputData.PrintFileAsTxt(outputFilePath, newWell.WellTrajectoryGraph);
                    ConsoleOutputForSavingProcess(saveResult, view);
                }
            }
        }
    }
}