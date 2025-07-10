using System.Configuration;
using System.Data;
using System.Windows;

namespace KitchenUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var inputWindow = new InputIP();
            inputWindow.Show();
            //new InputIP();
        }

    }

}
