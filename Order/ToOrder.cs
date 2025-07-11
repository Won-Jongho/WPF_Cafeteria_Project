using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order
{
    public class Order2 : INotifyPropertyChanged
    {
        private int orderId;
        private ObservableCollection<MenuItem> items;

        public int OrderId
        {
            get => orderId;
            set { orderId = value; OnPropertyChanged(nameof(OrderId)); }
        }

        public ObservableCollection<MenuItem> Items
        {
            get => items;
            set { items = value; OnPropertyChanged(nameof(Items)); OnPropertyChanged(nameof(Summary)); OnPropertyChanged(nameof(GroupedItems)); }
        }

        // 요약 문자열 (예: [3] 아메리카노 x1, 바닐라라떼 x1)
        public string Summary
        {
            get
            {
                if (Items == null || Items.Count == 0)
                    return $"[{OrderId}] (비어있음)";

                var grouped = Items.GroupBy(i => i.Name);
                return $"[{OrderId}] " + string.Join(", ", grouped.Select(g => $"{g.Key} x{g.Count()}"));
            }
        }

        // 오른쪽 상세 주문에 표시할 메뉴별 수량 리스트
        public ObservableCollection<string> GroupedItems
        {
            get
            {
                if (Items == null)
                    return new ObservableCollection<string>();

                var grouped = Items
                    .GroupBy(i => i.Name)
                    .Select(g => $"{g.Key} x{g.Count()}");
                return new ObservableCollection<string>(grouped);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
