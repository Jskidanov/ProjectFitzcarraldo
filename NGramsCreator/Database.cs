using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGramsCreator
{
    class Database
    {
        private SortedDictionary<String, Int32> nGrams = new SortedDictionary<String, Int32>();
        private String[] words;
        private int nvalue;
        private String temp;
        private int aproxnumber;

        public Database(string file, string nLength, string average) //Change the two inputs to ints once finished
        {
            aproxnumber = Int32.Parse(average);
            nvalue = Int32.Parse(nLength);
            temp = corpustoString(file);

        }

        public String corpustoString(String fileName)
        {
            string text;
            var fileStream = new FileStream(@"c:\" + fileName, FileMode.Open, FileAccess.Read); //Get correct path here
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }

            return text;
		}


        public void stringParse()
        {
            //** Next to the ones that used the "ReplaceAll" method. Research further later.
            // You changed the Split method to a character delimiter so check up on that.




            temp = temp.Replace("[\r\n]+", " "); //replaces line breaks and lines with a space   //**       
            temp = temp.Replace("\"", ""); //removes quotes
            temp = temp.Replace("\\\\", "");// removes \                                          //**
            temp = temp.Replace("[,.!?;:]", " $0").Replace("\\s+", " "); //adds a space before punctuation      //**
            temp = temp.Replace("[-]*", ""); //remove dashes                                                 //**
            temp = temp.Replace("[\\[\\](){}*]", ""); //removes parentheses/brackets/*                    //**
            temp = temp.Trim(); // removes beginning and ending spaces


            words = temp.Split(' '); //creates an array of strings 
        }

        public SortedDictionary<String, Int32> makeNGrams()
        {

            for (int i = 0; i <= words.Length - nvalue; i++)
            {

                StringBuilder sb = new StringBuilder();

                int j = 0;

                while (j < nvalue)
                {
                    sb.Append(words[i + j]);
                    j++;
                    if (j < nvalue) sb.Append(" ");
                }

                String ngramtobeadded = sb.ToString();

                if (!nGrams.ContainsKey(ngramtobeadded))
                {
                    nGrams[ngramtobeadded] = 1;
                }

                else if (nGrams.ContainsKey(ngramtobeadded))
                {
                    nGrams[ngramtobeadded] = nGrams[ngramtobeadded + 1];
                }
            }
            return nGrams;
        }

        public String weightedRandomSelection(SortedDictionary<String, Int32> nGrams)
        {
            Random _rnd = new Random();
            int randomNumber = _rnd.Next(0, nGrams.Values.Max());
            String text = "";

            foreach (KeyValuePair<String, Int32> word in nGrams)
            {
                if (randomNumber < word.Value)
                {
                    text = word.Key;
                    break;
                }

                randomNumber = randomNumber - word.Value;
            }

            return text;
        }

        public String generateText(int a)
        {
            String text = weightedRandomSelection(nGrams); //REEXAMINE IF YOU NEED DOUBLES

            String punctuations = ".,:;!?&/";
            while (char.IsLower(text[0]) || punctuations.Contains(returnFirstWord(text)))
            { //checks if first word chosen is punctuation or lower case
                text = weightedRandomSelection(nGrams);
            }

            for (int n = 0; n < a; n++)
            { //loops adding to the text for the amount of times of the inputed number
                text += " " + removeFirstWord(weightedRandomSelection(sortByFirstWord(nGrams, returnLastWord(text))));
            }

            //THIS DID HAVE REPLACEALL.
            text = text.Replace("\\s+(?=\\p{Punct})", ""); //removes spaces before punctuation 


            StringBuilder sb = new StringBuilder(text);

            int i = 0;
            while ((i = sb.indexOf(" ", i + 50)) != -1)
            {
                sb.Replace(i, i + 1, "\n");
            } //adds line breaks after the word after 50 characters


            return sb.ToString();
        }


        public void run()
        {
            stringParse();
            //System.out.println(temp);
            //System.out.println(words[0]);
            makeNGrams();
            //System.out.println(makeNGrams());
            //System.out.println(Arrays.toString(words));
            Console.WriteLine(generateText(aproxnumber));
        }



    }

}
