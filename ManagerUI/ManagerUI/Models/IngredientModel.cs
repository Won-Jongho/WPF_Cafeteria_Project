using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerUI.Models
{
    // 재고 재료 모델
    public class IngredientModel : INotifyPropertyChanged
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

        public IngredientModel()
        {
            ReceiptDate = DateTime.Now;  // 기본값으로 현재 날짜 설정
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void onPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}