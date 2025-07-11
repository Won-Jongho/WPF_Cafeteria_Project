using ManagerUI.Models;
using Sharedlib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ManagerUI.ViewModels
{
    public class IngredientViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Ingredient> inglist;
        public ObservableCollection<Ingredient> IngList { get { return inglist; } }

        public int SelectedIdx { get; set; }

        public Command CmdSave { get; set; }     //항목추가
        public Command CmdEdit { get; set; }    //수정
        public Command CmdDel { get; set; }     //삭제
        public Command CmdSel { get; set; }     //항목선택

        private IngredientModel ingmodel;
        public IngredientModel IngModel
        {
            get { return ingmodel; }
            set { ingmodel = value; onPropertyChanged(nameof(IngModel)); }
        }

        public IngredientViewModel()
        {
            ingmodel = new IngredientModel();
            inglist = new ObservableCollection<Ingredient>();
            CmdSave = new Command(exe_save, canexe);
            CmdEdit = new Command(exe_edit, canexe);
            CmdDel = new Command(exe_del, canexe);
            CmdSel = new Command(exe_sel, canexe);

            LoadIngredientsFromCsv();
            GetNextId();
        }

        private void exe_save(object o)
        {
            IngList.Add(new Ingredient(IngModel.Name, IngModel.Price, IngModel.Quantity, IngModel.ReceiptDate));
            
            // CSV 파일에 저장
            SaveIngredientToCsv();
        }
        private void exe_edit(object o)
        {
            IngList[SelectedIdx].Name = IngModel.Name;
            IngList[SelectedIdx].Price = IngModel.Price;
            IngList[SelectedIdx].Quantity = IngModel.Quantity;
            IngList[SelectedIdx].ReceiptDate = IngModel.ReceiptDate;

            // CSV 파일에 저장
            SaveIngredientToCsv();
        }
        private void exe_del(object o)
        {
            IngList.RemoveAt(SelectedIdx);

            // CSV 파일에 저장
            SaveIngredientToCsv();
        }
        private void exe_sel(object o)
        {
            if (SelectedIdx < 0)
            {
                return;
            }
            IngModel.Id = IngList[SelectedIdx].Id;
            IngModel.Name = IngList[SelectedIdx].Name;
            IngModel.Price = IngList[SelectedIdx].Price;
            IngModel.Quantity = IngList[SelectedIdx].Quantity;
            IngModel.ReceiptDate = IngList[SelectedIdx].ReceiptDate;
        }
        private bool canexe(object o)
        {
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void onPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        // IngredientViewModel에 CSV 파일에서 재고 ID 갱신 및 저장 기능
        private string inventoryCsvPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventory.csv");

        // CSV가 있을 시 가장 높은 ID를 찾아서 갱신
        private void GetNextId()
        {
            if (File.Exists(inventoryCsvPath))
            {
                var lines = File.ReadAllLines(inventoryCsvPath);

                // 파일이 비어있지 않은지 확인
                if (lines.Length > 1)
                {
                    var ids = lines.Skip(1) // 첫 번째 줄은 헤더
                                    .Select(line =>
                                    {
                                        // 빈 줄이나 잘못된 형식이 있으면 제외하고 처리
                                        var columns = line.Split(',');

                                        // ID가 정상적으로 파싱되는지 확인
                                        if (columns.Length > 0 && int.TryParse(columns[0], out int id))
                                        {
                                            return id;
                                        }

                                        // 문제가 있으면 0 반환 (혹은 다른 적절한 값)
                                        return -1;
                                    })
                                    .Where(id => id > 0) // 0 이하인 ID는 제외
                                    .ToList();

                    // ID가 존재하면 최대값을 찾고, 없으면 1로 설정
                    int nextId = ids.Any() ? ids.Max() : 1;

                    Ingredient.SetNextId(nextId); // 기존 최대 ID + 1
                }
                else
                {
                    // 파일에 데이터가 없으면 ID를 1로 설정
                    Ingredient.SetNextId(1);
                }
            }
            else
            {
                // 파일이 없으면 ID를 1로 설정
                Ingredient.SetNextId(1);
            }
        }

        public void LoadIngredientsFromCsv()
        {
            if (!File.Exists(inventoryCsvPath)) return;

            var csvLines = File.ReadAllLines(inventoryCsvPath, Encoding.UTF8);
            var ingredients = new ObservableCollection<Ingredient>();

            // 첫 번째 라인은 헤더이므로 건너뛰기
            for (int i = 1; i < csvLines.Length; i++)
            {
                // 빈 줄 건너뛰기
                if (string.IsNullOrWhiteSpace(csvLines[i])) continue;

                var values = csvLines[i].Split(',');


                // 각 값들을 Ingredient 객체로 변환
                int id = int.Parse(values[0]);
                string name = values[1]; // 두 번째 값은 Name
                int price = int.Parse(values[2]); // 세 번째 값은 Price
                int quantity = int.Parse(values[3]); // 네 번째 값은 Quantity
                DateTime receiptDate = DateTime.Parse(values[4]); // 다섯 번째 값은 ReceiptDate

                // 생성자에 맞게 값 전달
                var ingredient = new Ingredient(id, name, price, quantity, receiptDate);

                ingredients.Add(ingredient);
            }

            // inglist에 로드된 데이터를 할당
            inglist = ingredients;
        }

        private void SaveIngredientToCsv()
        {
            StringBuilder csvContent = new StringBuilder();

            // CSV 헤더 작성 (첫 번째 줄에만 작성)
            csvContent.AppendLine("Id,Name,Price,Quantity,ReceiptDate");

            // 새로운 데이터 추가
            foreach (var ingredient in inglist)
            {
                // CSV 포맷에 맞게 각 항목을 추가
                csvContent.AppendLine($"{ingredient.Id},{ingredient.Name},{ingredient.Price},{ingredient.Quantity},{ingredient.ReceiptDate:yyyy-MM-dd}");
            }

            // 파일 덮어쓰기
            if (File.Exists(inventoryCsvPath))
            {
                // 기존 파일에서 첫 번째 줄(헤더)만 남기고 나머지 내용 덮어쓰기
                var lines = File.ReadAllLines(inventoryCsvPath, Encoding.UTF8);

                // 첫 번째 줄(헤더)을 그대로 두고, 그 이후의 내용을 덮어쓸 내용으로 갱신
                var newContent = new List<string> { lines[0] }; // 첫 번째 줄(헤더)은 그대로 추가
                newContent.AddRange(csvContent.ToString().Split(Environment.NewLine).Skip(1)); // 첫 번째 줄을 제외한 나머지 덮어쓰는 내용 추가

                // 5. 덮어쓸 데이터를 기존 파일에 새로 작성
                File.WriteAllLines(inventoryCsvPath, newContent, Encoding.UTF8);
            }
            else
            {
                // 파일이 없으면 새로 만든다
                File.WriteAllText(inventoryCsvPath, csvContent.ToString(), Encoding.UTF8);
            }
        }
    }
}