using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for TextWindow.xaml
    /// </summary>
    public partial class TextWindow : Window
    {
        private string fileName;
        private string nGramsNum;
        private string nGramsAvg;
        Database nGramsDb;

        private SortedDictionary<String, Int32> nGrams;
        private String[] words;
        private int nValue;
        private String temp;
        private int aproxNumber;

        private LinkedList<String> parsedWordsList = new LinkedList<String>();


        public TextWindow(string fileName, string nGramsNum, string nGramsAvg)
        {
            this.fileName = fileName;
            this.nGramsNum = nGramsNum;
            this.nGramsAvg = nGramsAvg;
            nGramsDb = new Database(fileName, nGramsNum, nGramsAvg);

            nGrams = nGramsDb.NGrams;
            words = nGramsDb.Words;
            nValue = nGramsDb.NValue;
            temp = nGramsDb.Temp;
            aproxNumber = nGramsDb.ApproxNumber;

            
            InitializeComponent();

            LoadingText.Visibility = Visibility.Hidden;

            displayText();
        }

        private void displayText()
        {

            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(System.IO.File.ReadAllText(fileName));
            FlowDocument document = new FlowDocument(paragraph);
            NGramsFlowDoc.Document = document;
        }

        private void GetNGramsButton_Click(object sender, RoutedEventArgs e)
        {
            //REMEMBER YOU MAY NEED TO FIX THE NGRAMS WITH THE REPLACE FUNCTION TOO
            LoadingText.Visibility = Visibility.Visible;
            string segmentToSearch = NGramsFlowDoc.Selection.Text;
            segmentToSearch = segmentToSearch.Replace("\n", " ");
            segmentToSearch = segmentToSearch.Replace("\r", " ");
            segmentToSearch = System.Text.RegularExpressions.Regex.Replace(segmentToSearch, @"\s+", " ");

            NGramsDisplay nWindow = new NGramsDisplay(segmentToSearch, Int32.Parse(nGramsNum), nGrams);
            nWindow.Show();
            LoadingText.Visibility = Visibility.Hidden;

        }

        private void CustomSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadingText.Visibility = Visibility.Visible;
            string segmentToSearch = CustomSearchText.Text;
            segmentToSearch = segmentToSearch.Replace("\n", " ");
            segmentToSearch = segmentToSearch.Replace("\r", " ");
            segmentToSearch = System.Text.RegularExpressions.Regex.Replace(segmentToSearch, @"\s+", " ");

            NGramsDisplay nWindow = new NGramsDisplay(segmentToSearch, Int32.Parse(nGramsNum), nGrams);
            nWindow.Show();
            LoadingText.Visibility = Visibility.Hidden;
        }
    }
}
