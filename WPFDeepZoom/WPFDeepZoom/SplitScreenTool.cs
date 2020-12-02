using MultilayerZoom.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDeepZoom
{
    internal class SplitScreenTool
    {
        public static List<Border> SplitScreen(Point pStartRowColumn, Point pOverRowColumn, Grid gAllSlideBox, ResourceDictionary rdDirectory)
        {
            List<Border> list = new List<Border>();
            pStartRowColumn.X *= 2.0;
            pStartRowColumn.Y *= 2.0;
            pOverRowColumn.X *= 2.0;
            pOverRowColumn.Y *= 2.0;
            int num;
            int num2;
            if (pStartRowColumn.X < pOverRowColumn.X)
            {
                num = (int)pStartRowColumn.X;
                num2 = (int)pOverRowColumn.X;
            }
            else
            {
                num = (int)pOverRowColumn.X;
                num2 = (int)pStartRowColumn.X;
            }
            int num3;
            int num4;
            if (pStartRowColumn.Y < (double)(int)pOverRowColumn.Y)
            {
                num3 = (int)pStartRowColumn.Y;
                num4 = (int)pOverRowColumn.Y;
            }
            else
            {
                num3 = (int)pOverRowColumn.Y;
                num4 = (int)pStartRowColumn.Y;
            }
            Dictionary<Point, Border> dictionary = new Dictionary<Point, Border>();
            foreach (FrameworkElement child in gAllSlideBox.Children)
            {
                if (child is Border)
                {
                    int row = Grid.GetRow(child);
                    int column = Grid.GetColumn(child);
                    if (num <= row && row <= num2 && num3 <= column && column <= num4)
                    {
                        dictionary.Add(new Point(row - num, column - num3), (Border)child);
                    }
                }
            }
            ClearSplitScreen(gAllSlideBox);
            int num5 = Math.Abs((int)(pStartRowColumn.X - pOverRowColumn.X));
            int num6 = Math.Abs((int)(pStartRowColumn.Y - pOverRowColumn.Y));
            for (int i = 0; i <= num5; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                gAllSlideBox.RowDefinitions.Add(rowDefinition);
                if (i % 2 == 1)
                {
                    GridSplitter gridSplitter = new GridSplitter();
                    gridSplitter.SetValue(FrameworkElement.StyleProperty, rdDirectory["HorGridSplitter"]);
                    Grid.SetRow(gridSplitter, i);
                    Grid.SetColumnSpan(gridSplitter, num6 + 1);
                    gAllSlideBox.Children.Add(gridSplitter);
                    rowDefinition.SetValue(FrameworkElement.StyleProperty, rdDirectory["SplitterRow"]);
                }
                else
                {
                    rowDefinition.SetValue(FrameworkElement.StyleProperty, rdDirectory["SlideRow"]);
                }
            }
            for (int j = 0; j <= num6; j++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                gAllSlideBox.ColumnDefinitions.Add(columnDefinition);
                if (j % 2 == 1)
                {
                    GridSplitter gridSplitter2 = new GridSplitter();
                    gridSplitter2.SetValue(FrameworkElement.StyleProperty, rdDirectory["VerGridSplitter"]);
                    Grid.SetColumn(gridSplitter2, j);
                    Grid.SetRowSpan(gridSplitter2, num5 + 1);
                    gAllSlideBox.Children.Add(gridSplitter2);
                    columnDefinition.SetValue(FrameworkElement.StyleProperty, rdDirectory["SplitterColumn"]);
                }
                else
                {
                    columnDefinition.SetValue(FrameworkElement.StyleProperty, rdDirectory["SlideColumn"]);
                }
            }
            for (int k = 0; k <= num5; k++)
            {
                if (k % 2 == 1)
                {
                    continue;
                }
                for (int l = 0; l <= num6; l++)
                {
                    if (l % 2 == 1)
                    {
                        continue;
                    }
                    Point key = new Point(k, l);
                    Border border;
                    if (dictionary.ContainsKey(key))
                    {
                        border = dictionary[key];
                        if (num5 == 0 && num6 == 0)
                        {
                            RemoveActiveStyle(border.Child as Grid);
                        }
                    }
                    else
                    {
                        border = CreateSlideBox(rdDirectory);
                        list.Add(border);
                    }
                    Grid.SetRow(border, k);
                    Grid.SetColumn(border, l);
                    gAllSlideBox.Children.Add(border);
                }
            }
            dictionary.Clear();
            return list;
        }

        public static Border CreateSlideBox(ResourceDictionary rdDirectory)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.SetValue(FrameworkElement.StyleProperty, rdDirectory["rectLeft"]);
            Rectangle rectangle2 = new Rectangle();
            rectangle2.SetValue(FrameworkElement.StyleProperty, rdDirectory["rectTop"]);
            Rectangle rectangle3 = new Rectangle();
            rectangle3.SetValue(FrameworkElement.StyleProperty, rdDirectory["rectRight"]);
            Rectangle rectangle4 = new Rectangle();
            rectangle4.SetValue(FrameworkElement.StyleProperty, rdDirectory["rectBottom"]);
            Grid grid = new Grid();
            grid.SetValue(FrameworkElement.StyleProperty, rdDirectory["MultiScaleImageBox"]);
            Border border = new Border();
            border.SetValue(FrameworkElement.StyleProperty, rdDirectory["SlideNameBorder"]);
            Label label = new Label();
            label.SetValue(FrameworkElement.StyleProperty, rdDirectory["SlideName"]);
            border.Child = label;
            StackPanel stackPanel = CreatRulerBox(rdDirectory);
            MultiScaleImage multiScaleImage = new MultiScaleImage();
            multiScaleImage.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            multiScaleImage.VerticalContentAlignment = VerticalAlignment.Stretch;
            Border border2 = new Border();
            border2.SetValue(FrameworkElement.StyleProperty, rdDirectory["bGridBox"]);
            ZoomableCanvas scopedElement = (ZoomableCanvas)(border2.Child = new ZoomableCanvas());
            Grid grid2 = new Grid();
            grid2.SetValue(FrameworkElement.StyleProperty, rdDirectory["gMarkBox"]);
            Grid grid3 = new Grid();
            grid3.Children.Add(multiScaleImage);
            grid3.Children.Add(border2);
            grid3.Children.Add(grid2);
            grid.Children.Add(border);
            grid.Children.Add(stackPanel);
            grid.Children.Add(grid3);
            grid.Children.Add(rectangle);
            grid.Children.Add(rectangle2);
            grid.Children.Add(rectangle4);
            grid.Children.Add(rectangle3);
            Border result = new Border
            {
                Child = grid
            };
            NameScope.SetNameScope(grid, new NameScope());
            grid.RegisterName("bSlideNameBorder", border);
            grid.RegisterName("lSlideName", label);
            grid.RegisterName("sRulerToolBox", stackPanel);
            grid.RegisterName("gFlipBox", grid3);
            grid.RegisterName("mMultiScaleImage", multiScaleImage);
            grid.RegisterName("bGridBox", border2);
            grid.RegisterName("gMarkBox", grid2);
            grid.RegisterName("zcGrid", scopedElement);
            grid.RegisterName("rectLeft", rectangle);
            grid.RegisterName("rectTop", rectangle2);
            grid.RegisterName("rectBottom", rectangle4);
            grid.RegisterName("rectRight", rectangle3);
            return result;
        }

        private static StackPanel CreatRulerBox(ResourceDictionary rdDirectory)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.SetValue(FrameworkElement.StyleProperty, rdDirectory["sRulerToolBox"]);
            Border border = new Border();
            border.SetValue(FrameworkElement.StyleProperty, rdDirectory["sRulerToolBorder"]);
            StackPanel stackPanel2 = new StackPanel();
            Label label = new Label();
            label.SetValue(FrameworkElement.StyleProperty, rdDirectory["lRulerValue"]);
            StackPanel stackPanel3 = new StackPanel();
            stackPanel3.SetValue(FrameworkElement.StyleProperty, rdDirectory["sRulerItem"]);
            for (int i = 0; i < 5; i++)
            {
                Label label2 = new Label();
                label2.SetValue(FrameworkElement.StyleProperty, rdDirectory["lItem"]);
                if (i == 0)
                {
                    label2.BorderThickness = new Thickness(1.0, 1.0, 1.0, 0.0);
                }
                else
                {
                    label2.BorderThickness = new Thickness(0.0, 1.0, 1.0, 0.0);
                }
                stackPanel3.Children.Add(label2);
            }
            stackPanel2.Children.Add(label);
            stackPanel2.Children.Add(stackPanel3);
            border.Child = stackPanel2;
            StackPanel stackPanel4 = new StackPanel();
            stackPanel4.SetValue(FrameworkElement.StyleProperty, rdDirectory["sFlatTool"]);
            stackPanel.Children.Add(border);
            stackPanel.Children.Add(stackPanel4);
            NameScope.SetNameScope(stackPanel, new NameScope());
            stackPanel.RegisterName("lRulerValue", label);
            stackPanel.RegisterName("sRulerItem", stackPanel3);
            stackPanel.RegisterName("sFlatTool", stackPanel4);
            return stackPanel;
        }

        public static void FocusArea(Point pStartRowColumn, Point pOverRowColumn, Grid gAllRect)
        {
            int num = Math.Abs((int)(pStartRowColumn.X - pOverRowColumn.X)) + 1;
            int num2 = Math.Abs((int)(pStartRowColumn.Y - pOverRowColumn.Y)) + 1;
            foreach (Border child in gAllRect.Children)
            {
                int row = Grid.GetRow(child);
                int column = Grid.GetColumn(child);
                if (row < num && column < num2)
                {
                    child.Background = new SolidColorBrush(Color.FromRgb(57, 176, 181));
                }
                else
                {
                    child.Background = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                }
            }
        }

        public static void RemoveActiveStyle(Grid gSlideBox)
        {
            (gSlideBox.FindName("rectLeft") as Rectangle).Visibility = Visibility.Collapsed;
            (gSlideBox.FindName("rectTop") as Rectangle).Visibility = Visibility.Collapsed;
            (gSlideBox.FindName("rectBottom") as Rectangle).Visibility = Visibility.Collapsed;
            (gSlideBox.FindName("rectRight") as Rectangle).Visibility = Visibility.Collapsed;
        }

        public static void AddActiveStyle(Grid gSlideBox)
        {
            (gSlideBox.FindName("rectLeft") as Rectangle).Visibility = Visibility.Visible;
            (gSlideBox.FindName("rectTop") as Rectangle).Visibility = Visibility.Visible;
            (gSlideBox.FindName("rectBottom") as Rectangle).Visibility = Visibility.Visible;
            (gSlideBox.FindName("rectRight") as Rectangle).Visibility = Visibility.Visible;
        }

        public static bool CurrentSlideEmpty(Grid gAllSlideBox, Grid gSlideBox)
        {
            bool result = true;
            Border element = gSlideBox.Parent as Border;
            if (gAllSlideBox.Children.Contains(element))
            {
                result = false;
            }
            return result;
        }

        public static void ClearSplitScreen(Grid gAllSlideBox)
        {
            gAllSlideBox.Children.Clear();
            gAllSlideBox.RowDefinitions.Clear();
            gAllSlideBox.ColumnDefinitions.Clear();
        }

        public static void GetSelectedArea(Grid gAllRect, ref Point pStartRowColumn, ref Point pOverRowColumn)
        {
            pStartRowColumn = new Point(0.0, 0.0);
            pOverRowColumn = new Point(0.0, 0.0);
            foreach (Border child in gAllRect.Children)
            {
                if (child.Background.ToString() == "#FF7FFFAA")
                {
                    int row = Grid.GetRow(child);
                    int column = Grid.GetColumn(child);
                    if (pStartRowColumn == new Point(-1.0, -1.0))
                    {
                        pStartRowColumn = new Point(row, column);
                    }
                    if (pOverRowColumn == new Point(-1.0, -1.0))
                    {
                        pOverRowColumn = new Point(row, column);
                    }
                    else
                    {
                        pOverRowColumn.X = ((pOverRowColumn.X > (double)row) ? pOverRowColumn.X : ((double)row));
                        pOverRowColumn.Y = ((pOverRowColumn.Y > (double)column) ? pOverRowColumn.Y : ((double)column));
                    }
                }
            }
        }
    }

}
