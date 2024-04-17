using System;
namespace otomobil.DTOs.User
{
	public class ResetPasswordDTO
	{
		public string email { get; set; } = string.Empty;
		public string password { get; set; } = string.Empty;
		public string confirmPassword { get; set; } = string.Empty;
    }
}

