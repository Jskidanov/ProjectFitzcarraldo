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

                    KeyValuePair<List<KeyValuePair<String, Int32>>, SortedDictionary<String, Int32>> pair = searchSingleGrams(words[i]);
                    List<KeyValuePair<String, Int32>> singleGramOutput = pair.Key;
                    SortedDictionary<String, Int32> topTen = pair.Value;


                    results = results.Concat(singleGramOutput).ToList();
                    singleGramList = singleGramList.Concat(results).ToList();

                    TextBlock wordHeader = new TextBlock() { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]) };
                    wordHeader.FontStyle = FontStyles.Italic;
                    wordHeader.FontSize = 20;

                    NGramsStackPanel.Children.Add(wordHeader);

                    addTopTenFindings(results, topTen);

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


                    KeyValuePair<List<KeyValuePair<String, Int32>>, SortedDictionary<String, Int32>> pair = searchMultiGrams(combination);
                    List<KeyValuePair<String, Int32>> results = pair.Key;
                    SortedDictionary<String, Int32> topTen = pair.Value;


                    TextBlock wordHeader = new TextBlock() { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(combination) };
                    //wordHeader.FontStyle = FontStyles.Italic;
                    wordHeader.FontStyle = FontStyles.Oblique;
                    wordHeader.FontSize = 20;

                    NGramsStackPanel.Children.Add(wordHeader);

                    addTopTenFindings(results, topTen);
                    
                }

            }
        }

        private KeyValuePair<List<KeyValuePair<String, Int32>>, SortedDictionary<String, Int32>> searchSingleGrams(string searchKey)
        {

            List<KeyValuePair<String, Int32>> result = new List<KeyValuePair<String, Int32>>();
            SortedDictionary<String, Int32> topTen = new SortedDictionary<String, Int32>();

            foreach (KeyValuePair<String, Int32> entry in nGrams)
            {
                string parsedSearchSegment = searchKey;

                string parsedKey = parseString(entry.Key);

                if ((parsedSearchSegment.Contains(parsedKey) || parsedKey.Contains(parsedSearchSegment)))
                {
                    result.Add(entry);

                    string[] entryWords = entry.Key.Split(' ');

                    for (int i = 0; i < entryWords.Length; i++)
                    {
                        string word = entryWords[i];

                        if (topTen.ContainsKey(word))
                        {
                            topTen[word] = topTen[word] + 1;
                        }

                        else if (!word.Contains(searchKey))
                        {
                            topTen.Add(word, 1);
                        }
                    }

                }
            }

            return new KeyValuePair<List<KeyValuePair<string, int>>, SortedDictionary<string, int>>(result, topTen);

        }

        private KeyValuePair<List<KeyValuePair<String, Int32>>, SortedDictionary<String, Int32>> searchMultiGrams(string searchKey)
        {

            List<KeyValuePair<String, Int32>> result = new List<KeyValuePair<String, Int32>>();
            SortedDictionary<String, Int32> topTen = new SortedDictionary<String, Int32>();

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

                    string[] entryWords = entry.Key.Split(' ');

                    for (int i = 0; i < entryWords.Length; i++)
                    {
                        string word = entryWords[i];

                        if (topTen.ContainsKey(word))
                        {
                            topTen[word] = topTen[word] + 1;
                        }

                        else if (!searchKey.Contains(word))
                        {
                            topTen.Add(word, 1);
                        }
                    }
                }
            }

            return new KeyValuePair<List<KeyValuePair<String, Int32>>, SortedDictionary<String, Int32>> (result, topTen);

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

        private void addTopTenFindings(List<KeyValuePair<String, Int32>> selectedGrams, SortedDictionary<String, Int32> topTen)
        {
            List<KeyValuePair<String, Int32>> sortedTen = topTen.ToList();
            sortedTen = sortedTen.OrderByDescending(x => x.Value).ToList();
            int endRange;
            if (sortedTen.Count > 9)
            {
                endRange = 10;
            }

            else
            {
                endRange = sortedTen.Count;
            }

            for (int i = 0; i < endRange; i++)
            {
                string word = sortedTen[i].Key;
                List<KeyValuePair<String, Int32>> matches = new List<KeyValuePair<String, Int32>>();

                foreach (KeyValuePair<String, Int32> entry in selectedGrams)
                {
                    if (entry.Key.Split(' ').Contains(word))
                    {
                        matches.Add(entry);
                    }

                }

                addAllFindings(matches, word, matches.Sum(x => x.Value));

            }

            if (selectedGrams.Count > 0)
            {
                addAllFindings(selectedGrams, "All Findings", selectedGrams.Sum(x => x.Value));
            }
            
        }
        

        private void addAllFindings(List<KeyValuePair<String, Int32>> selectedGrams, string header, int headerCount)
        {
            DockPanel mainPanel = new DockPanel();
            Border mainBorder = new Border();
            mainBorder.BorderBrush = Brushes.Black;
            mainBorder.BorderThickness = new Thickness(2);
            mainBorder.Child = mainPanel;
            mainPanel.Background = Brushes.LightGray;

            TextBlock mainKey = new TextBlock() { Text = header };
            mainKey.FontSize = 12;
            TextBlock mainCount = new TextBlock() { Text = headerCount.ToString() };
            mainCount.FontSize = 16;
            mainCount.FontWeight = FontWeights.Bold;

            mainPanel.Children.Add(mainKey);
            mainPanel.Children.Add(mainCount);
            mainPanel.LastChildFill = false;

            Expander mainExpander = new Expander();
            mainExpander.ExpandDirection = ExpandDirection.Down;
            mainExpander.HorizontalAlignment = HorizontalAlignment.Stretch;
            StackPanel expanderStack = new StackPanel();
            expanderStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid mainGrid = new Grid();
            mainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainExpander.Content = mainGrid;
            mainGrid.Children.Add(expanderStack);
            mainPanel.Children.Add(mainExpander);

            DockPanel.SetDock(mainKey, Dock.Top);
            DockPanel.SetDock(mainCount, Dock.Right);

            NGramsStackPanel.Children.Add(mainBorder);

            foreach (KeyValuePair<String, Int32> gram in selectedGrams)
            {

                DockPanel panel = new DockPanel();
                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(2);
                border.Child = panel;
                panel.Background = Brushes.WhiteSmoke;

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

                expanderStack.Children.Add(border);
            }

        }
    }
}
