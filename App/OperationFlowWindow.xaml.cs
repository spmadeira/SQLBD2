using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Querying.Data;
using Querying.Query;
using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.UI.Xaml.Diagram.Controls;

namespace App
{
    public partial class OperationFlowWindow : Window
    {
        private List<NodeViewModel>[] NodeInfo { get; } 
            = new List<NodeViewModel>[8];
        private NodeCollection NodeCollection { get; }
            = new NodeCollection();
        private ConnectorCollection ConnectorCollection { get; }
            = new ConnectorCollection();

        public OperationFlowWindow()
        {
            InitializeComponent();
            
            Diagram.Nodes = NodeCollection;
            Diagram.Connectors = ConnectorCollection;
        }

        public OperationFlowWindow(IOperation operation) : this()
        {
            DrawOperation(operation);
        }

        private void DrawOperation(IOperation operation)
        {
            NodeCollection.Clear();
            ConnectorCollection.Clear();
            for (int i = 0; i < NodeInfo.Length; i++) NodeInfo[i] = new List<NodeViewModel>();
            
            AddNodeCascading(operation, 0);
            PositionNodes();
        }

        private NodeViewModel AddNodeCascading(IOperation operation, int level)
        {
            var node = new NodeViewModel
            {
                ID = Guid.NewGuid(),
                UnitHeight = 90,
                UnitWidth = 180,
                Shape = new RectangleGeometry {Rect = new Rect(0, 0, 10, 10)},
                ShapeStyle = Resources["ShapeStyle"] as Style,
                Annotations = new ObservableCollection<IAnnotation>
                {
                    new TextAnnotationViewModel()
                    {
                        Text = operation.OperationDescription,
                        FontStyle = FontStyles.Normal,
                        FontSize = 14,
                        FontFamily = Resources["MaterialDesignFont"] as FontFamily,
                        FontWeight = FontWeights.Normal,
                        Foreground = new SolidColorBrush(Colors.White),
                    }
                }
            };
            NodeInfo[level].Add(node);
            NodeCollection.Add(node);

            foreach (var childOperation in operation.ChildOperations)
            {
                var childNode = AddNodeCascading(childOperation, level + 1);
                ConnectorCollection.Add(new ConnectorViewModel
                {
                    SourceNode = node,
                    TargetNode = childNode
                });
            }

            return node;
        }

        private void PositionNodes()
        {
            var levels = 
                NodeInfo.TakeWhile(t => t.Count != 0).Count();

            if (levels == 0) return;
            
            var levelWidth = 900 / levels;

            for (int i = 0; i < levels; i++)
            {
                var col = NodeInfo[i];
                var levelSize = 600 / col.Count;

                for (int j = 0; j < col.Count; j++)
                {
                    col[j].OffsetX = (levelSize * (j + 1)) - levelSize / 2f;
                    col[j].OffsetY = (levelWidth * (i + 1)) - levelWidth / 2f;
                }
            }
        }
    }
}