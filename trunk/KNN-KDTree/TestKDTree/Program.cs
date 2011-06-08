using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KDTreeDLL;

namespace TestKDTree
{
    class Program
    {
        static void Main(string[] args)
        {
            KDTree tree = new KDTree(2);

            Random r=new Random();

            for(int i=0;i<2000000;i++)
            {
                double x=r.NextDouble();
                double y=r.NextDouble();

                double[] d = {x,y};

                tree.insert(d,x.ToString()+","+y.ToString());
            }

            double[] target={0.2,0.3};
            int k=4;
            Object[] result = tree.nearest(target,k);

            foreach (string s in result)
            {
                Console.WriteLine(s);
            }
        }
    }
}
