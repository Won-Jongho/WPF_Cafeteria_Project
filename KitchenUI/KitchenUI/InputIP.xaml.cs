using KitchenApp.Tcp;
using System;
using System.Windows;

namespace KitchenUI
{
    /// <summary>
    /// InputIP.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputIP : Window
    {
        public InputIP()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = IpTextBox.Text.Trim();
            int port = 3333; 
            var connector = new KitchenClientConnector();

            try
            {
                connector.Connect(ip, port);

                var viewModel = new MainViewModel(connector);
                var mainWindow = new MainWindow(viewModel);
                mainWindow.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 연결 실패: {ex.Message}\n프로그램을 종료합니다.");
                Application.Current.Shutdown();
            }
        }
    }
}