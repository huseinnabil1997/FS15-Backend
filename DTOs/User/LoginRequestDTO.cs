using System;
namespace otomobil.DTOs.User
{
	public class LoginRequestDTO
	{
		public string email { get; set; } = string.Empty;
		public string password { get; set; } = string.Empty;
	}
}

