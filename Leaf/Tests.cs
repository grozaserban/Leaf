using Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Net.NeuralNet;

namespace Leaf
{
    public class Tests
    {
        public void TestHogOnAllNewDataWithPerformanceAndConfidence()
        {
            var dataPath = @"C:\Users\Serban\Pictures\20+\testData";
            var folds = 5;

            for (int i = 0; i < folds; i++)
            {
                var allData = Reading.ReadAllFiles(dataPath, folds);
                var testData = allData[i];
                allData.RemoveAt(i);
                var data = Reading.MergeReadings(allData);

                Link.RenewalFactor = 0.000003; // maybe decrease
                Link.Step = 0.05;
                var sut = NeuralNetFactory.Create(2, 10, data.InputValues[0], data.ExpectedResults[0]);

                var iterations = sut.TrainAdaptiveWithConfidenceAndPerformance(data.InputValues, data.ExpectedResults, -0.00005, 500000, testData, i);
            }
        }
    }
}
