using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.ActiveDirectory; // 바이트 배열 변환을 위해 추가
using System.IO;
using System.Linq;
using System.Net.Sockets; // TCP 통신을 위해 추가
using System.Printing.IndexedProperties;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Order
{

    // 범용적인 ICommand 구현 (이전에 있던 Command 클래스입니다. MainViewModel로 옮깁니다.)
    public class RelayCommand : ICommand // 이름 충돌을 피하기 위해 Command -> RelayCommand로 변경
    {
        
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null) // canExecute를 선택적 매개변수로
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }

    // MainViewModel 클래스: 모든 UI 관련 로직 및 데이터가 여기에 옵니다.
    public class ViewModel : INotifyPropertyChanged
    {
        // 장바구니 항목 컬렉션 (UI에 바인딩될 것)
        private string Ip;
        private int order_cnt = 1;
        public ObservableCollection<MenuItem> CartItems { get; set; } = new ObservableCollection<MenuItem>();

        // 총 주문 금액 (UI에 바인딩될 것)
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

        // 명령 (Command): View의 사용자 액션(클릭)을 처리할 것
        public ICommand AddItemCommand { get; private set; } // 메뉴 추가 명령
        public ICommand RemoveItemCommand { get; private set; } // 장바구니 항목 제거 명령 (X 버튼)
        public ICommand PlaceOrderCommand { get; private set; } // 주문 명령 (주문 버튼)
        public ICommand ClearCartCommand { get; private set; } // 삭제 명령 (삭제 버튼)

        public ViewModel(string ip)
        {
            Ip = ip;
            // ObservableCollection의 변화를 감지하여 총 가격 업데이트
            CartItems.CollectionChanged += CartItems_CollectionChanged;

            // 명령 초기화
            AddItemCommand = new RelayCommand(ExecuteAddItem);
            RemoveItemCommand = new RelayCommand(ExecuteRemoveItem); 
            PlaceOrderCommand = new RelayCommand(ExecutePlaceOrder, CanPlaceOrder);
            ClearCartCommand = new RelayCommand(ExecuteClearCart, CanClearCart);

            UpdateOverallTotalPrice(); // 초기 총 금액 설정
        }

        // 장바구니 컬렉션 변경 시 호출
        private void CartItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // 항목 추가 시 PropertyChanged 이벤트 구독
            if (e.NewItems != null)
            {
                foreach (MenuItem item in e.NewItems)
                {
                    item.PropertyChanged += MenuItem_PropertyChanged;
                }
            }
            // 항목 제거 시 PropertyChanged 이벤트 구독 해제
            if (e.OldItems != null)
            {
                foreach (MenuItem item in e.OldItems)
                {
                    item.PropertyChanged -= MenuItem_PropertyChanged;
                }
            }
            UpdateOverallTotalPrice(); // 컬렉션 변경 시 총 가격 업데이트
        }

        // 개별 MenuItem의 속성 (수량, 가격) 변경 시 호출
        private void MenuItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MenuItem.Quantity) || e.PropertyName == nameof(MenuItem.Price))
            {
                UpdateOverallTotalPrice();
            }
        }

        // 총 주문 금액 업데이트 로직
        private void UpdateOverallTotalPrice()
        {
            OverallTotalPrice = CartItems.Sum(item => (int)item.TotalPrice);
            // CanExecute 상태를 다시 확인하도록 명령 관리자에게 요청 (버튼 활성화/비활성화 등)
            CommandManager.InvalidateRequerySuggested();
        }

        // AddItemCommand가 실행될 때 호출될 메서드
        private void ExecuteAddItem(object parameter)
        {
            // parameter는 XAML에서 CommandParameter로 넘겨줄 Tag 데이터 (예: "에스프레소,1000")
            if (parameter is string tagData)
            {
                string[] parts = tagData.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[1], out int price))
                {
                    string name = parts[0];
                    MenuItem existingItem = CartItems.FirstOrDefault(item => item.Name == name);

                    if (existingItem != null)
                    {
                        existingItem.Quantity++;
                    }
                    else
                    {
                        CartItems.Add(new MenuItem(name, price));
                    }
                }
            }
        }

        // RemoveItemCommand가 실행될 때 호출될 메서드 (X 버튼)
        private void ExecuteRemoveItem(object parameter)
        {
            // parameter는 XAML에서 CommandParameter로 넘겨줄 CartItems의 MenuItem 객체

            if (parameter is MenuItem itemToRemove)
            {
                if (itemToRemove.Quantity > 1)
                {
                    itemToRemove.Quantity--;
                }
                else
                {
                    CartItems.Remove(itemToRemove);
                }
            }
        }

        // PlaceOrderCommand가 실행될 때 호출될 메서드 (주문 버튼)
        private async void ExecutePlaceOrder(object parameter)
        {
            // 실제 주문 처리 로직 (예: MessageBox로 주문 완료 메시지 표시)
            if (CartItems.Any())
            {
                System.Windows.MessageBox.Show($"총 {OverallTotalPrice:N0}원에 대한 주문이 완료되었습니다!", "주문 완료", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                


                TcpClient client = new TcpClient(Ip, 3333);
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };


                ////
                if (order_cnt == 1) {
                    JObject registerMsg1 = new JObject
                    {
                        ["type"] = "register",
                        ["role"] = "order"
                    };

                    writer.WriteLine(registerMsg1.ToString(Formatting.None));
                    //Console.WriteLine("[클라이언트 등록] " + registerMsg1);
                }

                JArray itemsArray = new JArray();
                foreach (var carItem in CartItems)
                {
                    Console.WriteLine(" menu : " + carItem.Name + " quantity : " + carItem.Quantity);
                    JObject item = new JObject {
                        ["menu"] = carItem.Name, ["quantity"] = carItem.Quantity
                    };
                    itemsArray.Add(item);
                }

                JObject registerMsg = new JObject {
                    ["type"] = "order",
                    ["order_id"] = order_cnt++,
                    ["items"] = itemsArray
                };


                writer.WriteLine(registerMsg.ToString(Formatting.None));
                Console.WriteLine("[주문 전송] " + registerMsg);

                CartItems.Clear(); // 주문 후 장바구니 비우기
            }
            else
            {
                System.Windows.MessageBox.Show("장바구니가 비어있습니다.", "알림", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        // PlaceOrderCommand의 CanExecute 메서드 (주문 버튼 활성화/비활성화 로직)
        private bool CanPlaceOrder(object parameter)
        {
            return CartItems.Any(); // 장바구니에 항목이 하나라도 있을 때만 주문 가능
        }

        // ClearCartCommand가 실행될 때 호출될 메서드 (삭제 버튼)
        private void ExecuteClearCart(object parameter)
        {
            if (CartItems.Any())
            {
                var result = System.Windows.MessageBox.Show("장바구니를 비우시겠습니까?", "확인", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    CartItems.Clear(); // 장바구니 모든 항목 제거
                }
            }
        }

        // ClearCartCommand의 CanExecute 메서드 (삭제 버튼 활성화/비활성화 로직)
        private bool CanClearCart(object parameter)
        {
            return CartItems.Any(); // 장바구니에 항목이 하나라도 있을 때만 삭제 가능
        }

        // INotifyPropertyChanged 구현
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}