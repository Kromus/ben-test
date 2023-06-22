using System;

namespace Ensek_Rest_API_Tests.Models
{
    public class GetOrdersResponse
    {
        public string Id { get; set; }  
        public string Fuel { get; set; }
        public int Quantity { get; set; }
        public DateTime time { get; set; }
    }
}
