using System;
using otomobil.Data;

namespace otomobil.Models
{
	public class CartItem
	{
        public int user_id { get; set; }
        public int course_id { get; set; }
        public int category_id { get; set; }
        public string course_name { get; set; }
		public string category_name { get; set; }
        public string schedule { get; set; }
        public string image_url { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
    }
}
