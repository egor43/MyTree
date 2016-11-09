using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Xml.Serialization;
using System.Windows.Threading;
using MyTree;

namespace TreeViewerWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MyTree<string> Tree = new MyTree<string>();
        public byte Zoom = 1;
        public byte time = 3;
        public DispatcherTimer timer = new DispatcherTimer();
        public TreeNode<string> trenode;



        public MainWindow()
        {
            InitializeComponent();
            menu.IsEnabled = false;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(Logo_Show);
            timer.Start();
            Tree.Add(null, "0");
        }

        public void NodeDialog(object sender, MouseButtonEventArgs e)
        {
            Button btn = sender as Button;
            trenode = btn.Tag as TreeNode<string>;
            NodeDialog DialogWindow = new NodeDialog();
            DialogWindow.Owner = this;
            DialogWindow.ShowDialog();
            grid.Children.Clear();
            PrintTree<string>(Tree.Head, 0, 0, Zoom);
        }

        public void PrintTree<T>(TreeNode<T> tree, double StartX, double StartY, byte zoom = 1)
        {
            int number_d = 0;
            Button tb = new Button();
            tb.Content = Convert.ToString(tree.Value);
            tb.Height = 30 * zoom;
            tb.Width = 50 * zoom;
            tb.Tag = tree;
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.Margin = new Thickness(StartX * zoom, StartY * zoom, 0, 0);
            tb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.NodeDialog);
            grid.Children.Add(tb);
            for (int i = 0; i < tree.Children.Count; i++)
            {

                if (i != 0)
                {
                    number_d += CountNodes(tree.Children[i - 1]);
                    LineGeometry line = new LineGeometry();
                    line.StartPoint = new Point((StartX + 25) * zoom, (StartY + 30) * zoom);
                    line.EndPoint = new Point((StartX + 60) * zoom, (StartY + 50 * (i + 1) + number_d * 50 + 15) * zoom);
                    System.Windows.Shapes.Path ptline = new System.Windows.Shapes.Path();
                    ptline.Stroke = Brushes.Black;
                    ptline.Data = line;
                    grid.Children.Add(ptline);
                    PrintTree<T>(tree.Children[i], StartX + 60, StartY + 50 * (i + 1) + number_d * 50, zoom);
                }
                else
                {
                    LineGeometry line = new LineGeometry();
                    line.StartPoint = new Point((StartX + 25) * zoom, (StartY + 30) * zoom);
                    line.EndPoint = new Point((StartX + 60) * zoom, (StartY + 75) * zoom);
                    System.Windows.Shapes.Path ptline = new System.Windows.Shapes.Path();
                    ptline.Stroke = Brushes.Black;
                    ptline.Data = line;
                    grid.Children.Add(ptline);
                    PrintTree<T>(tree.Children[i], StartX + 60, StartY + 50, zoom);
                }
            }
        }

        private int CountNodes<T>(TreeNode<T> node)
        {
            int sum = 0;
            foreach (var v in node.Children)
            {
                sum += CountNodes(v);
            }
            return sum += node.Children.Count;
        }

        public int WidthTree<T>(TreeNode<T> tree)
        {
            int count = tree.Children.Count;
            List<int> list = new List<int>();
            foreach (var v in tree.Children)
            {
                SearchWidthTree<T>(v, list, 1);
            }
            foreach (var v in list)
            {
                if (count < v) count = v;
            }
            return count;
        }

        private void SearchWidthTree<T>(TreeNode<T> tree, List<int> list, int level)
        {
            if (list.Count < level) list.Add(0);
            list[level - 1] += tree.Children.Count;
            foreach (var v in tree.Children)
            {
                SearchWidthTree<T>(v, list, level + 1);
            }
        }

        private void ZoomUp_Click(object sender, RoutedEventArgs e)
        {
            if (Zoom < 10) Zoom++;
            grid.Children.Clear();
            PrintTree<string>(Tree.Head, 0, 0, Zoom);
        }

        private void ZoomDown_Click(object sender, RoutedEventArgs e)
        {
            if (Zoom > 1) Zoom--;
            grid.Children.Clear();
            PrintTree<string>(Tree.Head, 0, 0, Zoom);
        }

        private void Logo_Show(object sender, EventArgs e)
        {
            if (time < 1)
            {
                timer.Stop();
                menu.IsEnabled = true;
                Logo.IsEnabled = false;
                Logo.Visibility = Visibility.Collapsed;
            }
            time--;
            Logo.Text += ".";
        }
    }
}
