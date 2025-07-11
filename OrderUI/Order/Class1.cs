using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace Order
{
    public class MenuItem : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }

        private int price;
        public int Price
        {
            get { return price; }
            set { price = value; OnPropertyChanged(nameof(Price)); }
        } // 가격

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(TotalPrice)); // 수량이 바뀌면 총 가격도 바뀔 수 있으므로 알림
                }
            }
        }
        public int TotalPrice
        {
            get { return Quantity * Price; }
        }  // 계산된 속성 추가

        public List<RecipeItem> Recipe { get; set; } // 레시피 재료 목록

        public MenuItem(string name, int price)
        {
            Name = name;
            Price = price;
            Quantity = 1;
        }

        public MenuItem(string name, int price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RecipeItem
    {
        public string IngredientName { get; set; } // 사용할 재료 이름
        public int Quantity { get; set; } // 필요한 수량
    }
}
