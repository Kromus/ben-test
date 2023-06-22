using System;

namespace Ensek_Rest_API_Tests.Models
{
    public class Order
    {
        public string Id { get; set; }  
        public int Quantity { get; set; }  
        public string EnergyName { get; set; }
        public int EnergyId { get; set; }

    }
}
