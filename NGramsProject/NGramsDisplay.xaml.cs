using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NGramsProject
{
    /// <summary>
    /// Interaction logic for NGramsDisplay.xaml
    /// </summary>
    public partial class NGramsDisplay : Window
    {
        private string segmentToSearch;
        private int nGramsNumber;
        private SortedDictionary<String, Int32> nGrams;
        private List<KeyValuePair<String, Int32>> singleGramList = new List<KeyValuePair<String, Int32>>();
        LinkedList<string> wordCombinations = new LinkedList<string>();

        public NGramsDisplay(string segmentToSearch, int nGramsNumber, SortedDictionary<String, Int32> nGrams)
        {
            this.segmentToSearch = parseString(segmentToSearch);
            this.nGramsNumber = nGramsNumber;
            this.nGrams = nGrams;
            InitializeComponent();

            generateNGrams();
        }

        private void generateNGrams()
        {
            for (int i = 1; i < (nGramsNumber + 1); i++)
            {
                TextBlock header = new TextBlock() { Text = ("N = " + i) };
                header.FontWeight = FontWeights.Bold;
                header.FontSize = 20;

                NGramsStackPanel.Children.Add(header);


                narrowGrams(i);

            }
        }

        private void narrowGrams(int theCount)
        {
            
            //REMEMBER: YOU MAY HAVE TO GENREATE SEPERATE N-GRAM FOR THE SELECTED SEGMENT

            string[] words = segmentToSearch.Split(' ');

            if (theCount == 1)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    List<KeyValuePair<String, Int32>> results = new List<KeyValuePair<String, Int32>>();
                    results = results.Concat(searchSingleGrams(words[i])).ToList();
                    singleGramList = singleGramList.Concat(results).ToList();

                    TextBlock wordHeader = new TextBlock() { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]) };
                    wordHeader.FontStyle = FontStyles.Italic;
                    wordHeader.FontSize = 20;

                    NGramsStackPanel.Children.Add(wordHeader);

                    addNGrams(results);

                }
            }

            else
            {
                
                if (wordCombinations.Count == 0 && theCount == 2)
                {
                    
                    for (int i = 0; i < (words.Length - 1); i++)
                    {
                        string first = words[i] + " ";

                        for (int j = i + 1; j < words.Length; j++)
                        {
                            wordCombinations.AddLast(first + words[j]);
                        }

                    }
                }

                else
                {

                    LinkedList<string> newCombinations = new LinkedList<string>();

                    foreach (string prevCombo in wordCombinations)
                    {
                        for (int i = 0; i < words.Length; i++)
                        {
                            string newWord = words[i];

                            if (!prevCombo.Split(' ').Contains(newWord))
                            {
                                newCombinations.AddLast(prevCombo + " " + newWord);
                            }
                        }
                    }

                    wordCombinations = newCombinations;

                }


                foreach (string combination in wordCombinations)
                {

                    List<KeyValuePair<String, Int32>> results = searchMultiGrams(combination);

                    TextBlock wordHeader = new TextBlock() { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(combination) };
                    //wordHeader.FontStyle = FontStyles.Italic;
                    wordHeader.FontStyle = FontStyles.Oblique;
                    wordHeader.FontSize = 20;

                    NGramsStackPanel.Children.Add(wordHeader);

                    addNGrams(results);
                    
                }

            }
        }

        private List<KeyValuePair<String, Int32>> searchSingleGrams(string searchKey)
        {

            List<KeyValuePair<String, Int32>> result = new List<KeyValuePair<String, Int32>>();

            foreach (KeyValuePair<String, Int32> entry in nGrams)
            {
                string parsedSearchSegment = searchKey;

                string parsedKey = parseString(entry.Key);

                if ((parsedSearchSegment.Contains(parsedKey) || parsedKey.Contains(parsedSearchSegment)))
                {
                    result.Add(entry);
                }
            }

            return result;

        }

        private List<KeyValuePair<String, Int32>> searchMultiGrams(string searchKey)
        {

            List<KeyValuePair<String, Int32>> result = new List<KeyValuePair<String, Int32>>();

            foreach (KeyValuePair<String, Int32> entry in singleGramList)
            {
                string[] searchWords = searchKey.Split(' ');

                string parsedKey = parseString(entry.Key);

                bool allWordsIn = true;

                for (int i = 0; i < searchWords.Length; i++)
                {
                    string searchWord = searchWords[i];

                    if (!parsedKey.Contains(searchWord))
                    {
                        allWordsIn = false;
                        break;
                    }
                }

                if (allWordsIn)
                {
                    result.Add(entry);
                }
            }

            return result;

        }

        private string parseString(string theString)
        {
            //** Next to the ones that used the "ReplaceAll" method. Research further later.
            // You changed the Split method to a character delimiter so check up on that.

            string result = theString;

            char[] punctuation = new char[] { '-', '[', ']', '(', ')', '{', '}', '*', '-', ',', '.', '!', '?', ';', ':' };
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
        

        private void addNGrams(List<KeyValuePair<String, Int32>> selectedGrams)
        {

            foreach (KeyValuePair<String, Int32> gram in selectedGrams)
            {

                DockPanel panel = new DockPanel();
                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(2);
                border.Child = panel;
                panel.Background = Brushes.LightGray;

                TextBlock key = new TextBlock() { Text = gram.Key };
                key.FontSize = 12;
                TextBlock count = new TextBlock() { Text = gram.Value.ToString() };
                count.FontSize = 16;
                count.FontWeight = FontWeights.Bold;

                panel.Children.Add(key);
                panel.Children.Add(count);
                panel.LastChildFill = false;

                DockPanel.SetDock(key, Dock.Top);
                DockPanel.SetDock(count, Dock.Right);

                NGramsStackPanel.Children.Add(border);
            }

        }
    }
}
