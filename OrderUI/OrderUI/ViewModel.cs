using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyMenuItem = Sharedlib.Models.MenuItem;

namespace OrderUI
{
    internal class ViewModel : INotifyPropertyChanged
    {
        //이건 MenuItem 으로 빼는게 맞지 않나?
        private int order_cnt = 1;
        //public 으로 하는게 맞나?
        public ObservableCollection<MyMenuItem> CartItems { get; set; } = new ObservableCollection<MyMenuItem>();

        private int _overallTotalPrice;
        public int OverallTotalPrice
        {
            get { return _overallTotalPrice; }
            set
            {
                if (_overallTotalPrice != value)
                {
                    _overallTotalPrice = value;
                    OnPropertyChanged(nameof(OverallTotalPrice));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AddItemCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }
        public ICommand PlaceOrderCommand { get; private set; }
        public ICommand ClearCartCommand {  get; private set; }

        TcpClient client;
        NetworkStream stream;
        StreamReader reader;
        StreamWriter writer;

        public ViewModel(string ip)
        {
            //ViewModel 객체가 생성되면 바로 통신접속
            try
            {
                client = new TcpClient(ip, 3333);
                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("서버에 연결할 수 없습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("예기치 못한 오류가 발생하였습니다.");
            }

            JObject registerMsg = new JObject
            {
                ["type"] = "register",
                ["role"] = "order"
            };

            writer.WriteLine(registerMsg.ToString(Formatting.None));

            CartItems.CollectionChanged += CartItem_CollectionChanged;

            AddItemCommand = new Command(ExecuteAddItem);
            RemoveItemCommand = new Command(ExecuteRemoveItem);
            PlaceOrderCommand = new Command(ExecutePlaceOrder, CanPlaceOrder);
            ClearCartCommand = new Command(ExecuteClearCart, CanClearCart);

        }

        private void CartItem_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null) {
                foreach (MyMenuItem item in e.NewItems) 
                {
                    item.PropertyChanged += MenuItem_PropertyChanged;
                }
            }

            if (e.OldItems != null) 
            {
                foreach (MyMenuItem item in e.OldItems) 
                { 
                    item.PropertyChanged -= MenuItem_PropertyChanged;
                }
            }

            UpdateOverallTotalPrice();
        }

        private void MenuItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MyMenuItem.Quantity) || e.PropertyName == nameof(MyMenuItem.Price)) 
            {
                UpdateOverallTotalPrice();
            }
        }

        private void UpdateOverallTotalPrice()
        {
            //ObservableCollection 의 methid Sum 을 사용한다.
            OverallTotalPrice = CartItems.Sum(item => (int)item.TotalPrice);
            CommandManager.InvalidateRequerySuggested();
        }

        private void ExecuteAddItem(object parameter)
        {
            //parameter == tagData 인건가??
            if(parameter is string tagData)
            {
                string[] parts = tagData.Split(',');
                if(parts.Length == 2 && int.TryParse(parts[1], out int price))
                {
                    string name = parts[0];
                    MyMenuItem existingItem = CartItems.FirstOrDefault(Item => Item.Name == name);

                    if (existingItem != null) 
                    {
                        existingItem.Quantity++;    
                    }
                    else
                    {
                        CartItems.Add(new MyMenuItem(name, price));
                    }
                }
            }
        }

        private void ExecuteRemoveItem(object parameter)
        {
            //이건 무슨 조건이야 그냥 선언? 대입 아님?
            if(parameter is MyMenuItem itemToRemove)
            {
                if (itemToRemove.Quantity > 1)
                {
                    itemToRemove.Quantity--;
                }
                else {
                    CartItems.Remove(itemToRemove);
                }
            }
        }

        private void ExecutePlaceOrder(object parameter)
        {
            //ObservableCollection method Any 뭐라도 존재한다면
            if (CartItems.Any())
            {
                System.Windows.MessageBox.Show($"총 {OverallTotalPrice:N0}원에 대한 주문이 완료되었습니다!", "주문 완료", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                //통신을 위한 json array 생성
                JArray itemArray = new JArray();
                foreach (var cartItem in CartItems) {
                    JObject item = new JObject
                    {
                        ["menu"] = cartItem.Name,
                        ["quantity"] = cartItem.Quantity
                    };
                    itemArray.Add(item);
                }

                JObject msg = new JObject
                {
                    ["type"] = "order",
                    ["order_id"] = order_cnt++,
                    ["items"] = itemArray
                };

                //string 으로 변환하여 보낸다. Formatting.None 은 뭐지?
                writer.WriteLine(msg.ToString(Formatting.None));
                Console.WriteLine("[주문 전송]" + msg);

                CartItems.Clear(); //주문 후 장바구니 비우기
            }
            else
            {
                System.Windows.MessageBox.Show("장바구니가 비어있습니다.", "알림", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private void ExecuteClearCart(object parameter)
        {
            if (CartItems.Any())
            {
                var result = System.Windows.MessageBox.Show("장바구니를 비우시겠습니까?", "확인", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                if(result == System.Windows.MessageBoxResult.Yes){
                    CartItems.Clear();
                }
            }
        }

        private bool CanClearCart(object arg)
        {
            return CartItems.Any();
        }

        private bool CanPlaceOrder(object arg)
        {
            return CartItems.Any();
        }
    }
}
