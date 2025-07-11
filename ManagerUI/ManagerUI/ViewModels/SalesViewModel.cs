using CommunityToolkit.Mvvm.Input;
using ManagerUI.Models;
using Sharedlib.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ManagerUI.ViewModels
{
    public class SalesViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SalesRecord> saleslist;
        public ObservableCollection<SalesRecord> SalesList { get { return saleslist; } } // 전체

        private ObservableCollection<SalesRecord> filteredsales;
        public ObservableCollection<SalesRecord> FilteredSales
        {
            get { return filteredsales; }
            set { filteredsales = value; OnPropertyChanged(nameof(FilteredSales)); }
        } // 필터된 것

        private DateTime startDate { get; set; } = DateTime.Today.AddDays(-7);
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; OnPropertyChanged(nameof(StartDate)); }
        }
        private DateTime endDate { get; set; } = DateTime.Today;
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; OnPropertyChanged(nameof(EndDate)); }
        }

        public Command FilterCmd { get; set; } // 필터 커맨드

        public Command RefreshCmd { get; set; } // 새로 고침 버튼 커맨드

        public int TotalFilteredSales => FilteredSales.Sum(s => s.Total);

        // 생성자에서 새로 고침 버튼 커맨드 연결
        public SalesViewModel()
        {
            saleslist = new ObservableCollection<SalesRecord>();
            filteredsales = new ObservableCollection<SalesRecord>();

            FilterCmd = new Command(exeFilter, canexe);
            RefreshCmd = new Command(exeRefresh, canexe);

            LoadSalesFromCsv(); // 초기 매출 CSV 불러오기
            exeFilter(null); // 초기 필터링
        }

        private void exeFilter(object o)
        {
            var filtered = SalesList
                .Where(s => s.Date >= StartDate && s.Date <= EndDate)
                .ToList();

            FilteredSales = new ObservableCollection<SalesRecord>(filtered);
            OnPropertyChanged(nameof(TotalFilteredSales));
        }

        private void exeRefresh(object o)
        {
            LoadSalesFromCsv();  // 파일에서 다시 읽어오고
            exeFilter(null);     // 날짜 필터링도 다시 적용
        }

        private bool canexe(object o)
        {
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // SalesViewModel에 매출 조회 CSV 불러오기 기능 추가

        private string salesCsvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sales.csv");

        // 매출 CSV에서 데이터 읽기
        private void LoadSalesFromCsv()
        {
            if (File.Exists(salesCsvPath))
            {
                var lines = File.ReadAllLines(salesCsvPath).Skip(1); // 첫 번째 줄은 헤더

                saleslist.Clear();
                foreach (var line in lines)
                {
                    // 빈 줄 건너뛰기
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var columns = line.Split(',');

                    saleslist.Add(new SalesRecord
                    {
                        Date = DateTime.Parse(columns[3]),
                        ItemName = columns[0],
                        Quantity = int.Parse(columns[1]),
                        UnitPrice = int.Parse(columns[2]),
                    });
                }
                exeFilter(null); // 새로 불러온 데이터를 필터링
            }
        }
    }
}