
using System.ComponentModel;
namespace Sharedlib.Models;

public class MenuItem : INotifyPropertyChanged
{
    private string name;
    public string Name
    {
        get { return name; }
        set 
        { 
            name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private int price;
    public int Price { 
        get { return price; }
        set
        {
            price = value;
            OnPropertyChanged(nameof(Price));
        }
    }

    private int quantity;
    public int Quantity { 
        get { return quantity; }
        set
        {
            quantity = value;
            OnPropertyChanged(nameof(Quantity));
        }
    }

    public int TotalPrice {
        get { return Quantity * Price;  }
    }

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
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

