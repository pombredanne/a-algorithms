using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Accord.Statistics.Analysis;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is a demo application that combines Linear Discriminant Analysis (LDA) and Multilayer Perceptron(MLP).");
            double[,] inputs = 
            {
              {  4,  1 }, 
              {  2,  4 },
              {  2,  3 },
              {  3,  6 },
              {  4,  4 },
              {  9, 10 }, 
              {  6,  8 },
              {  9,  5 },
              {  8,  7 },
              { 10,  8 }
            };

            int[] output = 
            {
              1, 1, 2, 1, 1, 2, 2, 2, 1, 2
            };

            Console.WriteLine("\r\nProcessing sample data, pease wait...");

            //1.1
            var lda = new LinearDiscriminantAnalysis(inputs, output);

            //1.2 Compute the analysis
            lda.Compute();

            //1.3
            double[,] projection = lda.Transform(inputs);

            //both LDA and MLP have a little bit different inputs
            //e.x double[,] to double[][], etc. 
            //e.x. LDA needs int classes and MLP needs classes to be in the range [0..1]
            #region convertions
            int vector_count = projection.GetLength(0);
            int dimensions = projection.GetLength(1);

            //====================================================================

            // conver for NN
            double[][] input2 = new double[vector_count][];
            double[][] output2 = new double[vector_count][];

            for (int i = 0; i < input2.Length; i++)
            {
                input2[i] = new double[projection.GetLength(1)];
                for (int j = 0; j < projection.GetLength(1); j++)
                {
                    input2[i][j] = projection[i, j];
                }

                output2[i] = new double[1];

                //we turn classes from ints to doubles in the range [0..1], because we use sigmoid for the NN
                output2[i][0] = Convert.ToDouble(output[i]) / 10;
            }
            #endregion


            //2.1 create neural network
            ActivationNetwork network = new ActivationNetwork(
                new SigmoidFunction(2),
                dimensions, // inputs neurons in the network
                dimensions, // neurons in the first layer
                1); // one neuron in the second layer

            //2.2 create teacher
            BackPropagationLearning teacher = new BackPropagationLearning(network);
           
            //2.3 loop
            int p = 0;
            while (true)
            {
                // run epoch of learning procedure
                double error = teacher.RunEpoch(input2, output2);

                p++;
                if (p > 1000000) break;
                // instead of iterations we can check error values to see if we need to stop
            }

            //====================================================================

            //3. Classify
            double[,] sample = { { 10, 8 } };
            double[,] projectedSample = lda.Transform(sample);
            double[] projectedSample2 = new double[2];

            projectedSample2[0] = projectedSample[0, 0];
            projectedSample2[1] = projectedSample[0, 1];

            double[] classs = network.Compute(projectedSample2);

            Console.WriteLine("========================");

            //we convert back to int classes by first rounding and then multipling by 10 (because we devided to 10 before)
            //if you do not get the expected result
            //- rounding might be a problem
            //- try more training 

            Console.WriteLine(Math.Round(classs[0], 1, MidpointRounding.AwayFromZero)*10);
            Console.ReadLine();

        }
    }
}
