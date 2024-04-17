using System;
namespace otomobil.Models
{
	public class User
	{
		
		public int user_id { get; set; }

		public string name { get; set; } = string.Empty;

		public string email { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;
        public bool isActivated { get; set; }
    }
}

