using System;
namespace otomobil.DTOs.User
{
	public class LoginResponseDTO
	{
		public string Token { get; set; } = string.Empty;
        public int user_id { get; set; }
    }
}

