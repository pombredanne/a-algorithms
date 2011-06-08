using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Milo
{
    class Program
    {
        static string[] customers; //= { "Angietttttttttttttttttttttt", "Peter","Anton"};
        static string[] products; //= { "aaaaaaaaaaaaaaaaa","PlayStationsdfsdf", "Xboxreiyujfcfh", "Iphonegdsgsdtetr", "Pravecgdupoijh" , "Carsfdsdbaaaaaaaaaaaaaafds", "Captivafsdfoiteraaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaggndsfkgvdsfk" };

        static void Main(string[] args)
        {
            if (args.Length > 0)
                ReadFile(args[0]);
            else { Console.WriteLine("No input file!"); return; }

            //1. Generate assignment matrix
            double[,] AssignmentMatrix = new double[customers.Length, products.Length];
            
            //rows are customers
            //columns are products
            double maxSS = 0;
            for(int i=0;i<customers.Length;i++)
            {
                for(int j=0;j<products.Length;j++)
                {
                    double SS = CalculateSS(products[j],customers[i]);
                    if (SS > maxSS) maxSS = SS;
                    AssignmentMatrix[i,j] = SS;
                }
            }

            //2. Invert because we are searching for max not min
            for (int i = 0; i < AssignmentMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < AssignmentMatrix.GetLength(1); j++)
                {
                    AssignmentMatrix[i, j] = maxSS - AssignmentMatrix[i, j];
                }
            }

            //3. Apply Hungarian algorithm
            int[] result = HungarianAlgorithm.FindAssignments(AssignmentMatrix);

            //4. Save result
            double TotalSS=0;
            for (int i = 0; i < result.Length; i++)
            {
                //Console.WriteLine("Customer=" + customers[i] + ",product=" + products[result[i]]);
                TotalSS += AssignmentMatrix[i, result[i]] + maxSS;
            }
            //Console.WriteLine("TotalSS="+TotalSS);
            //Console.ReadKey();
            WriteFile("output.txt", result, TotalSS);
        }

        public static double CalculateSS(string product, string customer)
        {
            double SS = 0;

            const string vowels = "aeiou";

            if (product.Length % 2 == 0)//even
            {
                int nvowels = customer.Count(chr => vowels.Contains(char.ToLower(chr)));
                SS = nvowels * 1.5;

            }
            else//odd
            {
                int nconsonants = customer.Count(chr => !vowels.Contains(char.ToLower(chr)));
                SS = nconsonants;
            }

            int gcd = GCD(product.Length, customer.Length);

            if (gcd!=1)
            {
                SS = SS * 1.5; 
            }

            return SS;
        }

        static int GCD(int a, int b)
        {
            int Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }

        static void ReadFile(string filename)
        {
            using (StreamReader r = new StreamReader(filename))
            {
                products = r.ReadLine().Split(' ');
                customers = r.ReadLine().Split(' ');
            }
        }

        static void WriteFile(string filename,int[] result,double TotalSS)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
            {
                file.WriteLine(TotalSS);
                for (int i = 0; i < result.Length; i++)
                {
                    file.WriteLine("Customer=" + customers[i] + ",product=" + products[result[i]]);
                }
            }

            //Console.WriteLine("Customer=" + customers[i] + ",product=" + products[result[i]]);
        }
    }
}
