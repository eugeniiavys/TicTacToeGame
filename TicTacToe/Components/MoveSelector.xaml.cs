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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToe.Components
{
    /// <summary>
    /// Interaction logic for MoveSelector.xaml
    /// </summary>
    public partial class MoveSelector : UserControl
    {
        public static readonly DependencyProperty RowPositionProperty =
            DependencyProperty.Register("RowPosition", typeof(string), typeof(MoveSelector), new PropertyMetadata(string.Empty));

        public string RowPosition
        {
            get { return (string)GetValue(RowPositionProperty); }
            set { SetValue(RowPositionProperty, value); }
        }

        public static readonly RoutedEvent MoveClickEvent = EventManager.RegisterRoutedEvent(
            "MoveClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MainWindow));

        public event RoutedEventHandler MoveClick
        {
            add { AddHandler(MoveClickEvent, value); }
            remove { RemoveHandler(MoveClickEvent, value); }
        }

        public Button ClickedButton { get; set; }

        public MoveSelector()
        {
            InitializeComponent();
        }

        private void OnMoveClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            clickedButton.IsEnabled = false;

            ClickedButton = (Button)sender;

            RaiseEvent(new RoutedEventArgs(MoveClickEvent));
        }

        public void ResetButtons()
        {
            var mainGrid = (Grid)Content;
            var buttonGrid = (Grid)mainGrid.Children[0];

            foreach (UIElement child in buttonGrid.Children)
            {
                if (child is Button button)
                {
                    button.Content = "";
                    button.ClearValue(Button.BackgroundProperty);
                    button.IsEnabled = true;
                    button.Visibility = Visibility.Visible;
                }
            }

            ClickedButton = null;
        }

    }
}