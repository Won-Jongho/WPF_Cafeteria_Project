using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharedlib.Models
{
    public class SalesRecord : INotifyPropertyChanged
    {
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; onPropertyChanged(nameof(Date)); }
        }

        private string _itemName;
        public string ItemName
        {
            get { return _itemName; }
            set {  _itemName = value; onPropertyChanged(nameof(ItemName)); }
        }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            // Quantity나 UnitPrice가 변경되면 Total도 업데이트
            set { _quantity = value; onPropertyChanged(nameof(Quantity)); onPropertyChanged(nameof(Total)); }
        }

        private int _unitPrice;
        public int UnitPrice
        {
            get { return _unitPrice; }
            // Quantity나 UnitPrice가 변경되면 Total도 업데이트
            set { _unitPrice = value; onPropertyChanged(nameof(UnitPrice)); onPropertyChanged(nameof(Total)); }
        }

        public int Total => Quantity * UnitPrice;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void onPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}