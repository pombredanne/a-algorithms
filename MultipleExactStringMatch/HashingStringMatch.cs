using System;
using System.Collections.Generic;
using System.Text;

namespace ExactStringSearch
{
    /// <summary>
    /// Algorithm for exact string based on hashes
    /// Author Anton Andreev
    /// </summary>
    class HashingStringMatch : IStringSearchAlgorithm
    {
        struct Tokens
        {
            public char Character;
            public UInt64 Code;

            public Tokens(Char character, UInt64 code)
            {
                Character = character;
                Code = code;
            }
        }

        static Tokens[] tokens;

        static HashingStringMatch()
        {
            tokens = new Tokens[300];

            Random r = new Random();

            //1. Build codes for each token

            for (char c = 'A'; c <= 'Z'; c++)
            {
                Tokens t = new Tokens(c, Convert.ToUInt64(r.Next(1, Int32.MaxValue)));

                tokens[Convert.ToInt32(c)] = t;
            }
            for (char c = 'a'; c <= 'z'; c++)
            {
                Tokens t = new Tokens(c, Convert.ToUInt64(r.Next(1, Int32.MaxValue)));

                tokens[Convert.ToInt32(c)] = t;
            }
            tokens[Convert.ToInt32(' ')] = new Tokens(' ', (UInt64)r.Next(1, Int32.MaxValue));
            //tokens[Convert.ToInt32('\'')] = new Tokens('\'', (UInt64)r.Next(1, Int32.MaxValue));
            //tokens[Convert.ToInt32('"')] = new Tokens('"', (UInt64)r.Next(1, Int32.MaxValue));
            //tokens[Convert.ToInt32('.')] = new Tokens('.', (UInt64)r.Next(1, Int32.MaxValue));
            //tokens[Convert.ToInt32(',')] = new Tokens(',', (UInt64)r.Next(1, Int32.MaxValue));
            //tokens[Convert.ToInt32('!')] = new Tokens('!', (UInt64)r.Next(1, Int32.MaxValue));
            //tokens[Convert.ToInt32('?')] = new Tokens('?', (UInt64)r.Next(1, Int32.MaxValue));
            //tokens[Convert.ToInt32('-')] = new Tokens('-', (UInt64)r.Next(1, Int32.MaxValue));
            //tokens[Convert.ToInt32(':')] = new Tokens(':', (UInt64)r.Next(1, Int32.MaxValue));

            //tokens[Convert.ToInt32('t')] = new Tokens('t', 7);
            //tokens[Convert.ToInt32('e')] = new Tokens('e', 8);
            //tokens[Convert.ToInt32('s')] = new Tokens('s', 9);
            //tokens[Convert.ToInt32('a')] = new Tokens('t', 5);
        }

        public string[] Keywords
        {
            get;
            set;
        }

        public StringSearchResult[] FindAll(string text)
        {
            List<StringSearchResult> results = new List<StringSearchResult>();
           
            string[] patterns = Keywords;

            //2. Calculate Hash per pattern
            UInt64[] PatternHashes = new UInt64[patterns.Length]; //Hash per Pattern
            int i = 0;

            foreach (string p in patterns)
            {
                UInt64 hash = 0;
                foreach (char cc in p)
                {
                    UInt64 code = tokens[Convert.ToInt32(cc)].Code;
                    hash += code;
                }
                PatternHashes[i] = hash;
                i++;
            }

            UInt64[] CurrentHashes = new UInt64[patterns.Length]; //one current hash per pattern
            int[] CurrentHashesLength = new int[patterns.Length]; //hash length must be equal to pattern length to compare 

            //2. Search
            for (int position = 0; position < text.Length; position++)
            {
                for (int j = 0; j < patterns.Length; j++)
                {
                    int word_start = position - patterns[j].Length;

                    if (CurrentHashesLength[j] < patterns[j].Length)
                    {
                        CurrentHashesLength[j]++;
                        CurrentHashes[j] += tokens[Convert.ToInt32((char)text[position])].Code;
                    }

                    else
                        if (word_start >= 0 && CurrentHashesLength[j] == patterns[j].Length)
                        {
                            //add next 
                            CurrentHashes[j] += tokens[Convert.ToInt32((char)text[position])].Code;
                            //remove first
                            CurrentHashes[j] -= tokens[Convert.ToInt32((char)text[word_start])].Code;
                        }

                    if (CurrentHashesLength[j] == patterns[j].Length && CurrentHashes[j] == PatternHashes[j])
                    {
                        //we have a possible candidate
                        string very_good_candidate = text.Substring(word_start + 1, patterns[j].Length);

                        if (very_good_candidate == patterns[j])
                        {
                            //Console.WriteLine("Match found:" + text.Substring(word_start + 1, patterns[j].Length) + " " + (word_start + 1));

                            StringSearchResult res = new StringSearchResult(word_start + 1, very_good_candidate);
                            results.Add(res);
                        }
                    }

                }
            }

            return results.ToArray();
        }

        public StringSearchResult FindFirst(string text)
        {
            throw new NotImplementedException();
        }

        public bool ContainsAny(string text)
        {
            throw new NotImplementedException();
        }

        public double TotalDuration { get; set; }
    }
}