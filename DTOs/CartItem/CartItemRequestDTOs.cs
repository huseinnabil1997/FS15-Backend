using System;
namespace otomobil.DTOs.CartItem
{
	public class CartItemRequestDTO
	{
		public int user_id { get; set; }
		public int course_id { get; set; }
		public int category_id { get; set; }
		public string course_name { get; set; }
        public string schedule { get; set; }
        public int price { get; set; }
		public int quantity { get; set; } = 1;
    }
}

