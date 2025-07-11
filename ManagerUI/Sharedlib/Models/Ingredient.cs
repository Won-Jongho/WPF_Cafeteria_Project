using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharedlib.Models
{
    // SharedLibrary/Models/Ingredient.cs
    // 재고 재료 하나를 나타냄

    public class Ingredient : INotifyPropertyChanged
    {
        private int _id; // 재료 ID
        public int Id
        {
            get { return _id; }
            set { _id = value; onPropertyChanged(nameof(Id)); }
        }

        private string _name; // 재료 이름
        public string Name
        {
            get { return _name; }
            set { _name = value; onPropertyChanged(nameof(Name)); }
        }

        private int _price; // 단가
        public int Price
        {
            get { return _price; }
            set { _price = value; onPropertyChanged(nameof(Price)); }
        }
        private int _quantity; // 현재 수량
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; onPropertyChanged(nameof(Quantity)); }
        }
        private DateTime _receiptDate; // 입고일 (yyyyMMdd)
        public DateTime ReceiptDate
        {
            get { return _receiptDate; }
            set { _receiptDate = value; onPropertyChanged(nameof(ReceiptDate)); }
        }
        private int _consumption; // 예상 소비량
        public int Consumption
        {
            get { return _consumption; }
            set { _consumption = value; onPropertyChanged(nameof(Consumption)); }
        }

        private static int cnt;
        public static void SetNextId(int startId)
        {
            cnt = startId; // 외부에서 초기 ID 값을 설정
        }

        // 패러미터 생성자 (생성 시 반드시 패러미터 필요)
        public Ingredient(int id, string name, int price, int quantity, DateTime receiptDate)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            ReceiptDate = receiptDate;
            Consumption = 0;
        }
        public Ingredient(string name, int price, int quantity, DateTime receiptDate)
        {
            Id = ++cnt;
            Name = name;
            Price = price;
            Quantity = quantity;
            ReceiptDate = receiptDate;
            Consumption = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void onPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}