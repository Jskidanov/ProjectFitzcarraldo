using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NGramsProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int nGramNum = 0;
        private string path;
        private int nGramsAvg = 5;
        string name;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void load_FileClick(object sender, RoutedEventArgs e)
        {
            //MODIFY THIS TO WORK BETTER
            path = System.IO.Path.Combine(Environment.CurrentDirectory, "Corpus\\");
            OpenFileDialog theDialogue = new OpenFileDialog();
            theDialogue.DefaultExt = ".txt";
            theDialogue.Filter = "Text documents (.txt)|*.txt";


            if (Directory.Exists(path))
            {
                theDialogue.InitialDirectory = path;
            }
            else
            {
                theDialogue.InitialDirectory = @"C:\";
            }


            theDialogue.ShowDialog();

            name = theDialogue.FileName;

        }

        private void start_Click(object sender, RoutedEventArgs e)
        {

            if ((nGramNum != 0) && (name != null))
            {
                nGramNum++; //Off by one error I'll fix later
                TextWindow textWin = new TextWindow(name, nGramNum.ToString(), nGramsAvg.ToString());
                textWin.Show();
                this.Close();
            }

            else
            {
                MessageBoxResult result = MessageBox.Show("Please load a compatible file and select an N-Gram length", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
            
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nGramNum = nGramBox.SelectedIndex;
        }

        private void GenerateStatistics_Click(object sender, RoutedEventArgs e)
        {
            string theText = CustomText.Text;
            int theLength = theText.Split(' ').Length;
            NGramsDisplay nWindow = new NGramsDisplay(theText, 5, new Database(theText, 5).NGrams);
            nWindow.Show();
        }
    }
}
