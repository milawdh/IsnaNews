using DataLayer.Dtos.Shared;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using ServiceLayer.Filters;
using ServiceLayer.Utils;
using Services.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace IsnaNews.Controllers
{
    public class LoginController : Controller
    {
        private readonly Core _core;
        private readonly IConfiguration _configuration;

        public LoginController(Core core, IConfiguration configuration)
        {
            _core = core;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View("Login");
        }
        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        [AuthorizeAttempts("LoginAttempt","Register")]
        [ValidateModel("Register")]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(UserCreateUpdateDto user)
        {
            TempData["Errors"] = null;
            List<string> errors = new List<string>();
            var regex = new Regex("^\\d[-0-9]+\\d$");

            if (_core.User.Any(_ => _.UserName == user.UserName))
                errors.Add($"این نام کاربری {user.UserName} از قبل وجود دارد");
            if (user.Tell.Length != 11 || !regex.IsMatch(user.Tell))
            {
                errors.Add("شماره تلفن نا معتبر است");
            }
            else if (_core.User.Any(_ => _.Tell == user.Tell))
                errors.Add($"شماره تلفن {user.Tell} از قبل وجود دارد");
            if (errors.Count > 0)
            {
                TempData["ErrorTitle"] = "دروارد کردن فیلد ها دقت کنید";
                TempData["Errors"] = errors;
                return RedirectToAction("Register", "Login");
            }

            TblUser tblUser = new TblUser()
            {
                Name = user.UserName,
                Tell = user.Tell,
                UserName = user.UserName,
                Password = Extentions.HashData(user.Password),
            };
            _core.User.Add(tblUser);
            _core.Save();

            var token = GenerateJwtToken(user.UserName);
            HttpContext.Response.Cookies.Append("Token", token, new CookieOptions { Expires = DateTime.Now.AddMinutes(20) });

            return RedirectToAction("Index", "UserProfile");
        }
        [HttpPost]
        [AuthorizeAttempts("LoginAttempt","Index")]
        [ValidateModel("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult UserLogin(UserLoginDto userLogin)
        {
            TempData["Errors"] = null;
            userLogin.Password = userLogin.Password.HashData();
            if (_core.User.Any(_ => _.UserName == userLogin.UserName && _.Password == userLogin.Password))
            {
                var token = GenerateJwtToken(userLogin.UserName);
                HttpContext.Response.Cookies.Append("Token", token, new CookieOptions { Expires = DateTime.Now.AddMinutes(20) });
                return RedirectToAction("Index", "UserProfile");
            }
            TempData["ErrorTitle"] = "دروارد کردن فیلد ها دقت کنید";
            TempData["Errors"] = new List<string> { "نام کاربری یا رمز اشتباه است" };
            return RedirectToAction("Index","Login");
        }


        private string GenerateJwtToken(string userName)
        {
            //header
            string Issuer = _configuration["Jwt:Issuer"];
            string Audience = _configuration["Jwt:Audience"];
            var expires = DateTime.Now.AddMinutes(20);
            //signiture
            byte[] key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            //payload
            var subject = new ClaimsIdentity
            (new[]
                {
                new Claim(ClaimTypes.NameIdentifier,userName),
                }
            );
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Headers
                Issuer = Issuer,
                Audience = Audience,
                Expires = expires,
                //payload
                Subject = subject,
                //signiture
                SigningCredentials = signingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string JwtToken = tokenHandler.WriteToken(token);
            return JwtToken;
        }
    }
}
