using ManagerUI.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManagerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IngredientViewModel _ingViewModel = new IngredientViewModel();
        private SalesViewModel _salesViewModel = new SalesViewModel();
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Inventory(MainFrame, _ingViewModel)); // 시작화면
        }

        private void GoToInventory(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Inventory(MainFrame, _ingViewModel));
        }

        private void GoToSales(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Sales(MainFrame, _salesViewModel));
        }
    }
}