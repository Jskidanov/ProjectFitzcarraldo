using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NGramsProject
{
    internal class Database
    {
        private SortedDictionary<String, Int32> nGrams = new SortedDictionary<String, Int32>();
        private String[] words;
        private int nValue;
        private String temp;
        private int approxNumber;

        public SortedDictionary<string, int> NGrams { get => nGrams; set => nGrams = value; }
        public string[] Words { get => words; set => words = value; }
        public int NValue { get => nValue; set => nValue = value; }
        public string Temp { get => temp; set => temp = value; }
        public int ApproxNumber { get => approxNumber; set => approxNumber = value; }

        public Database(string file, string nLength, string average) //Change the two inputs to ints once finished
        {
            approxNumber = Int32.Parse(average);
            nValue = Int32.Parse(nLength);
            temp = System.IO.File.ReadAllText(file);

            temp = parseString(temp);
            words = temp.Split(' ');

            makeNGrams();

        }

        public Database(string[] input, int nLength)
        {
            words = input;
            nValue = nLength;

            makeNGrams();

        }

        public void testTemp()
        {
            string theWords = "";

            for (int i = 0; i < words.Length; i++)
            {
                theWords = theWords + words[i] + "\n";
            }

            System.IO.File.WriteAllText(@"C:\Users\jskid\Desktop\TestFolder\Temp.txt", theWords);

        }


        public string parseString(string theString)
        {
            //** Next to the ones that used the "ReplaceAll" method. Research further later.
            // You changed the Split method to a character delimiter so check up on that.

            string result = theString;

            char[] punctuation = new char[] { '-', '[', ']', '(', ')', '{', '}', '*' };
            // Notice  -,.!?;: are still there

            result = result.Replace("\"", "");
            result = result.Replace("\r", " ");
            result = result.Replace("\n", " ");
            result = result.Replace(". . .", "...");
            result = Regex.Replace(result, @"\\+", " ");
            result = Regex.Replace(result, @"\/+", " ");
            result = Regex.Replace(result, @"\s+", " ");
            string[] temp = result.Split(punctuation, StringSplitOptions.RemoveEmptyEntries);
            result = string.Join("", temp);

            result = result.Trim();

            return result;
        }

        public void makeNGrams()
        {
            LinkedList<string> previousSequence = new LinkedList<string>();

            for (int i = 0; i <= Words.Length - NValue; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j < NValue; j++)
                    {
                        string newString = words[i + j];

                        if (j != (NValue - 1))
                        {
                            newString = newString + " ";
                        }

                        if (j != 0)
                        {
                            previousSequence.AddLast(newString);
                        }

                    }

                }

                string newTerm = " " + words[i + (NValue - 1)];
                previousSequence.AddLast(newTerm);

                string sequenceToAdd = "";
                for (LinkedListNode<string> node = previousSequence.First; node != null; node = node.Next)
                {
                    sequenceToAdd = sequenceToAdd + node.Value;
                }

                sequenceToAdd = sequenceToAdd.Trim();
                addStringtoDictionary(sequenceToAdd);
                previousSequence.RemoveFirst();
            }
        }

        public void addStringtoDictionary(string theKey)
        {
            if (nGrams.ContainsKey(theKey))
            {
                nGrams[theKey] = nGrams[theKey] + 1;
            }

            else
            {
                nGrams.Add(theKey, 1);
            }
        }


        public void testNGrams()
        {

            using (StreamWriter file = new StreamWriter(@"C:\Users\jskid\Desktop\TestFolder\NGrams.txt"))
                foreach (var entry in nGrams)
                    file.WriteLine("[{0} {1}]", entry.Key, entry.Value);
        }

        //public String weightedRandomSelection(SortedDictionary<String, Int32> nGrams)
        //{
        //    Random _rnd = new Random();
        //    int randomNumber = _rnd.Next(0, nGrams.Values.Max());
        //    String text = "";

        //    foreach (KeyValuePair<String, Int32> word in nGrams)
        //    {
        //        if (randomNumber < word.Value)
        //        {
        //            text = word.Key;
        //            break;
        //        }

        //        randomNumber = randomNumber - word.Value;
        //    }

        //    return text;
        //}rn lastWord;
        //}
    }
}