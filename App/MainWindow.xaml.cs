using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using App;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.LargeGraphLayout;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using Microsoft.Msagl.Prototype.Ranking;
using Microsoft.Msagl.WpfGraphControl;
using Querying.Data;
using Querying.Query;
using Database = Querying.Data.Database;

namespace BD2App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database Database;
        private IOperation CurrentOperation;
        private BackgroundWorker DatabaseLoader = new BackgroundWorker();
        
        public MainWindow()
        {
            InitializeComponent();

            QueryInput.IsEnabled = false;
            AnalysisButton.IsEnabled = false;
            
            DatabaseLoader.DoWork += (sender, args) =>
            {
                Database = new Database();
                MockData.Seed(Database);    
            };
            DatabaseLoader.RunWorkerCompleted += (sender, args) =>
            {
                LogBox.Text += "Database loaded\n";
                QueryInput.IsEnabled = true;
            };
            
            DatabaseLoader.RunWorkerAsync();
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
            if (Database == null) return;
            
            var text = QueryInput.Text;
            QueryInput.Text = "";
            
            if (text.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
            {
                LogBox.Text = "";
                DataGrid.Columns.Clear();
                DataGrid.ItemsSource = null;
                return;
            }

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
                // BuildOperationDiagram(op);
                CurrentOperation = op;
                AnalysisButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                LogBox.Text += $"\nQuery error -- {ex.Message}";
            }
        }

        private void QueryAnalysis(object sender, RoutedEventArgs e)
        {
            BuildOperationDiagram(CurrentOperation);
        }

        private void BuildOperationDiagram(IOperation operation)
        {
            var diagramWindow = new OperationFlowWindow(operation);
            diagramWindow.Show();
        }
    }
}