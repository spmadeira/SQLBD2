using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public MainWindow()
        {
            InitializeComponent();
            var db = new Database();
            MockData.Seed(db);
            var result = Utils.BuildOperation("select * from usuario;", db).RunOperation();
            // foreach (var key in result.EntryCollection.Keys)
            // {
            //     DataGrid.Columns.Add(new DataGridTextColumn
            //     {
            //         Binding = new Binding(key.ToString().ToLower())
            //     });
            // }
            
            

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
            
            DataGrid.AutoGenerateColumns = false;
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
    }
}