using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Querying.Data;

namespace BD2App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Database Database;
        
        public MainWindow()
        {
            InitializeComponent();
            Database = new Database();
            MockData.Seed(Database);
            KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                {
                    QueryClick(sender, null);
                }
            };
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void QueryClick(object sender, RoutedEventArgs e)
        {
            var text = QueryInput.Text;
            QueryInput.Text = "";
            LogBox.Text += $"\n{text}";
            try
            {
                var sw = Stopwatch.StartNew();
                var op = Utils.BuildOperation(text + ";", Database);
                
                var result = op.RunOperation();
                sw.Stop();
                LogBox.Text += $" -- Duration: {sw.ElapsedMilliseconds}ms";
                
                var parsedResults = new List<Dictionary<string, string>>();
                foreach (var entry in result.EntryCollection.Entries)
                {
                    var subdict = new Dictionary<string,string>();
                    foreach (var key in result.EntryCollection.Keys)
                    {
                        subdict.Add(key.ToString(),entry.Fields[key].ToString());
                    }
                    parsedResults.Add(subdict);
                }
            
                DataGrid.Columns.Clear();
                foreach (var key in result.EntryCollection.Keys)
                {
                    DataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = key.ToString(),
                        Binding = new Binding($"[{key.ToString()}]")
                    });
                }
                
                DataGrid.ItemsSource = parsedResults;
            }
            catch (IndexOutOfRangeException ex)
            {
                LogBox.Text += $"\nQuery error -- {ex.Message}";
                
            }
        }
    }
}