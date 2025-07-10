using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Sharedlib.Models
{
    public class Order
    {
        public string Type { get; set; }
        
        public int Order_Id { get; set; }
        public List<OrderItem> Items { get; set; }

        // 주문 요약 표시용 속성 (ListBox용)

        public string SummaryText
        {
            get
            {
                string itemSummary = string.Join(" ", Items.Select(i => $"{i.Menu} x {i.Quantity}"));
                return $"주문번호: {Order_Id} {itemSummary}";
            }
        }




        // 오른쪽 상세 표시용
        public List<string> GroupedItems
        {
            get
            {
                var result = new List<string> {$"주문번호: {Order_Id}"};

                result.AddRange(Items.Select(i => $"{i.Menu} x {i.Quantity}"));

                return result;
            }
        }


    }
}
