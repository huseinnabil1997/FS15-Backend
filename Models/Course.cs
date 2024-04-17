using System;
using otomobil.Data;

namespace otomobil.Models
{
	public class Course
	{
		public int course_id { get; set; }
		public string course_name { get; set; }
		public string category_name { get; set; }
        public string image_url { get; set; }
        public int price { get; set; }
    }
}
