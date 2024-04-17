using System;
using otomobil.Models;
using otomobil.DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using otomobil.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using otomobil.DTOs.Email;
using Microsoft.AspNetCore.WebUtilities;

namespace otomobil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

	public class UserController : ControllerBase
	{
        private readonly UserData _userData;
        private readonly IConfiguration _configuration;
        private readonly EmailService _mail;

        public UserController(UserData userData, IConfiguration configuration, EmailService mail)
        {
            _userData = userData;
            _configuration = configuration;
            _mail = mail;
        }

        [HttpPost("CreateUser")]

        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDto)
        {
            try
            {
                User user = new User
                {
                    //Id = Guid.NewGuid(),
                    email = userDto.email,
                    name = userDto.name,
                    password = BCrypt.Net.BCrypt.HashPassword(userDto.password),
                    isActivated = false
                };

                //UserRole userRole = new UserRole
                //{
                //    UserId = user.Id,
                //    Role = userDto.Role
                //};

                bool result = _userData.CreateUserAccount(user);

                if (result)
                {
                    bool mailResult = await SendEmailActivation(user);
                    return StatusCode(201, userDto);
                }
                else
                {
                    return StatusCode(500, "Data not inserted");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequestDTO credential)
        {
            if (credential is null) return BadRequest("Invalid client request");

            if (string.IsNullOrEmpty(credential.email) || string.IsNullOrEmpty(credential.password)) return BadRequest("Invalid client request");

            User? user = _userData.CheckUserAuth(credential.email);

            if (user == null) return Unauthorized("You do not authorized");

            // Pengecekan isActivated
            if (!user.isActivated)
            {
                return BadRequest("Your account is not activated. Please check your email for activation link.");
            }

            bool isVerified = BCrypt.Net.BCrypt.Verify(credential.password, user?.password);

            if (user != null && !isVerified)
            {
                return BadRequest("Incorrect Password! Please check your password");
            }
            else
            {
                var key = _configuration.GetSection("JwtConfig:Key").Value;
                var JwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                var claims = new Claim[]
                {
            new Claim(ClaimTypes.Name, user.email),
                    //new Claim(ClaimTypes.Role, userRole.Role)
                };

                var signingCredential = new SigningCredentials(
                    JwtKey, SecurityAlgorithms.HmacSha256Signature
                );

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = signingCredential
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var securityToken = tokenHandler.CreateToken(tokenDescriptor);

                string token = tokenHandler.WriteToken(securityToken);

                return Ok(new LoginResponseDTO { Token = token });
            }
        }


        [HttpGet("ActivateUser")]
        public IActionResult ActivateUser(int user_id, string email)
        {
            try
            {
                User? user = _userData.CheckUserAuth(email);

                if (user == null)
                    return BadRequest("Activation Failed");

                if (user.isActivated == true)
                    return BadRequest("User has been activated");

                bool result = _userData.ActivateUser(user_id);

                if (result)
                    return Ok("User activated");
                else
                    return StatusCode(500, "Activation Failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Email is empty");

                bool sendMail = await SendEmailForgetPassword(email);

                if (sendMail)
                {
                    return Ok("Mail sent");
                }
                else
                {
                    return StatusCode(500, "Error");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        private async Task<bool> SendEmailForgetPassword(string email)
        {
            // send email
            List<string> to = new List<string>();
            to.Add(email);

            string subject = "Forget Password";

            var param = new Dictionary<string, string?>
                    {
                        {"email", email }
                    };

            string callbackUrl = QueryHelpers.AddQueryString("http://localhost:5173/new-password", param);

            string body = "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>";

            EmailModel mailModel = new EmailModel(to, subject, body);

            bool mailResult = await _mail.SendAsync(mailModel, new CancellationToken());

            return mailResult;
        }

        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            try
            {
                if (resetPassword == null)
                    return BadRequest("No Data");

                if (resetPassword.password != resetPassword.confirmPassword)
                {
                    return BadRequest("Password doesn't match");
                }

                bool reset = _userData.ResetPassword(resetPassword.email, BCrypt.Net.BCrypt.HashPassword(resetPassword.password));

                if (reset)
                {
                    return Ok("Reset password OK");
                }
                else
                {
                    return StatusCode(500, "Error");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<bool> SendEmailActivation(User user)
        {
            if (user == null)
                return false;

            if (string.IsNullOrEmpty(user.email))
                return false;
            // send email
            List<string> to = new List<string>();
            to.Add(user.email);

            string subject = "Account Activation";

            var param = new Dictionary<string, string?>
                    {
                        {"user_id", user.user_id.ToString() },
                        {"email", user.email }
                    };

            string callbackUrl = QueryHelpers.AddQueryString("http://localhost:5173/confirm-success", param);

            //string body = "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>";

            EmailActivationModel model = new EmailActivationModel()
            {
                Email = user.email,
                Link = callbackUrl
            };

            string body = _mail.GetEmailTemplate(model);


            EmailModel mailModel = new EmailModel(to, subject, body);
            bool mailResult = await _mail.SendAsync(mailModel, new CancellationToken());
            return mailResult;
        }
    }
}

