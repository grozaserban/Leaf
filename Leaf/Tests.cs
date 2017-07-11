using Net;
using System;
using System.Collections.Generic;
using static Net.NeuralNet;

namespace Leaf
{
    public class Tests
    {
        public void TestHogOnAllNewDataWithPerformanceAndConfidence(string dataPath)
        {
            var folds = 5;

            for (int i = 0; i < folds; i++)
            {
                var allData = Reading.ReadAllFiles(dataPath, folds);
                var testData = allData[i];
                allData.RemoveAt(i);
                var data = Reading.MergeReadings(allData);

                Link.RenewalFactor = 0.000003; // maybe decrease
                Link.Step = 0.05;
                var sut = NeuralNetFactory.Create(2,
                    10,
                    data.InputValues[0].Count,
                    data.ExpectedResults[0].Count);

                var iterations = sut.TrainAdaptiveWithConfidenceAndPerformance(data.InputValues, data.ExpectedResults, -0.00005, 150000, testData, i, dataPath);
            }
        }

        public static void SplitHistogramsInSeparateFiles(string sourcePath, string destinationFolderPath)
        {
            var data = new Reading(sourcePath);

            data.CreateTestData(destinationFolderPath);
        }
    }
}
