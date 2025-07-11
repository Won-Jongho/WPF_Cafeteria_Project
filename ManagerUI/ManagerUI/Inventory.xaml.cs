using ManagerUI.ViewModels;
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

namespace ManagerUI
{
    /// <summary>
    /// Inventory.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Inventory : Page
    {
        private Frame _mainFrame;

        public Inventory(Frame frame, IngredientViewModel viewModel)
        {
            InitializeComponent();
            _mainFrame = frame;
            this.DataContext = viewModel;
        }
    }
}
