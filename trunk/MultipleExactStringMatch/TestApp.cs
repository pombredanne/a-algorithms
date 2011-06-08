using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ExactStringSearch
{
	class TestApp
	{
		/// <summary>
		/// Minimal and maximal word length
		/// </summary>
		const int MaxWordLength = 10;
		const int MinWordLength = 3;
		
		/// <summary>
		/// Allowed letters in word
		/// </summary>
		const string AllowedLetters="abcdefghijklmnopqrstuvwxyz";

		#region Utility methods

		static Random rnd=new Random(12345);

		/// <summary>
		/// Generate random word
		/// </summary>
		public static string GetRandomWord()
		{
			StringBuilder sb=new StringBuilder();
      for(int i=0; i<rnd.Next(MaxWordLength-MinWordLength)+MinWordLength; i++)
				sb.Append(AllowedLetters[rnd.Next(AllowedLetters.Length)]);
			return sb.ToString();
		}


		/// <summary>
		/// Generate list of random keywords
		/// </summary>
		public static string[] GetRandomKeywords(int count)
		{
			string[] ret=new string[count];
			for(int i=0; i<count; i++)
				ret[i]=GetRandomWord();
			return ret;
		}


		/// <summary>
		/// Generate random text
		/// </summary>
		public static string GetRandomText(int count)
		{
			StringBuilder sb=new StringBuilder();
			while(sb.Length<count) { sb.Append(GetRandomWord()); sb.Append(" "); }
			return sb.ToString();
		}

		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// set language to english
			// (because in czech "ch" is one letter and "abch".IndexOf("bc") returns -1)
			System.Threading.Thread.CurrentThread.CurrentCulture=System.Globalization.CultureInfo.CreateSpecificCulture("en");

			// algorithms
			IStringSearchAlgorithm[] algorithms=new IStringSearchAlgorithm[2];
			algorithms[0]=new AhoCorasickStringMatch();
            algorithms[1] = new HashingStringMatch();

			double[] results=new double[3];
			for(int j=0; j<50; j++)
			{
				// generate random keywords and random text
				string[] keywords=GetRandomKeywords(80);
				string text=GetRandomText(2000);

				// insert keyword into text
				text=text.Insert(rnd.Next(text.Length),keywords[rnd.Next(keywords.Length)]);
				
				// for each algorithm...
				for(int algIndex=0; algIndex<algorithms.Length; algIndex++)
				{
					IStringSearchAlgorithm alg=algorithms[algIndex];
                    Console.WriteLine(alg.ToString());
					alg.Keywords=keywords;

					// search for keywords and measure performance
					HiPerfTimer tmr=new HiPerfTimer();
					tmr.Start();

                    StringSearchResult[] r=alg.FindAll(text);

					tmr.Stop();

                    Console.WriteLine(tmr.Duration);
                    Console.WriteLine("Found: "+r.Length);
                    alg.TotalDuration += tmr.Duration;
                    
				}

                Console.WriteLine("===========================");
				Console.WriteLine();
			}

			Console.WriteLine("\n===SUMMARY===");

			for(int j=0; j<algorithms.Length; j++)
			{
				IStringSearchAlgorithm alg=algorithms[j];
				Console.WriteLine(alg.ToString()+"="+alg.TotalDuration);
			}

			Console.WriteLine();
		}
	}
}
