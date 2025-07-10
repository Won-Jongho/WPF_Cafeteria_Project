using CommunityToolkit.Mvvm.Input;
using KitchenApp;
using KitchenApp.Tcp;
using KitchenUI;
using Sharedlib.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;


public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Order> Orders { get; set; }
    private Order selectedOrder;
    public Order SelectedOrder
    {
        get => selectedOrder;
        set
        {
            selectedOrder = value;
            OnPropertyChanged(nameof(SelectedOrder));
        }
    }

    public ICommand AcceptOrderCommand { get; }
    public ICommand RejectOrderCommand { get; }

    private KitchenClientConnector _client;

    public MainViewModel(KitchenClientConnector connector)
    {
        Orders = new ObservableCollection<Order>();
        // 명령 초기화 - 커스텀 Command 사용
        AcceptOrderCommand = new Command(exe_AcceptOrder, CanExecuteOrderCommand);
        RejectOrderCommand = new Command(exe_RejectOrder, CanExecuteOrderCommand);

        // TCP 연결
        _client = connector;
        _client.OrderReceived += OnOrderReceived;
    }




    private void OnOrderReceived(Order order)
    { // UI 스레드에서 실행
      App.Current.Dispatcher.Invoke(() => { Orders.Add(order);
          Debug.WriteLine(order.Order_Id);
          // 첫 주문이 들어오면 선택된 주문으로 설정
           if (SelectedOrder == null)
          { SelectedOrder = order; } }); 
    }


    private void exe_AcceptOrder(object parameter)
    {
        AcceptOrder();
    }

    private void exe_RejectOrder(object parameter)
    {
        RejectOrder();
    }
    private bool CanExecuteOrderCommand(object parameter)
    {
        return SelectedOrder != null;
    }

    private void AcceptOrder()
    {
        if (SelectedOrder != null)
        {
            try
            {
                // 서버로 수락 메시지 전송
                _client.SendAcceptOrder(SelectedOrder);

                // UI에서 주문 제거
                var acceptedOrder = SelectedOrder;
                Orders.Remove(acceptedOrder);
                SelectedOrder = Orders.FirstOrDefault();

                // 성공 메시지 (선택사항)
                MessageBox.Show($"주문 {acceptedOrder.Order_Id}이 수락되었습니다.", "주문 수락", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"주문 수락 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void RejectOrder()
    {
        if (SelectedOrder != null)
        {
            try
            {
                // UI에서 주문 제거
                var rejectedOrder = SelectedOrder;
                Orders.Remove(rejectedOrder);
                SelectedOrder = Orders.FirstOrDefault();

                // 성공 메시지 (선택사항)
                MessageBox.Show($"주문 {rejectedOrder.Order_Id}이 거절되었습니다.", "주문 거절", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"주문 거절 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}