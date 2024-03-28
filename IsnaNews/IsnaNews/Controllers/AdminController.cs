using DataLayer.Dtos.Admin;
using DataLayer.Dtos.Admin.Advertisement;
using DataLayer.Dtos.Admin.Base;
using DataLayer.Dtos.Admin.Category;
using DataLayer.Dtos.Admin.Comment;
using DataLayer.Dtos.Admin.Config;
using DataLayer.Dtos.Admin.Keyword;
using DataLayer.Dtos.Admin.News;
using DataLayer.Dtos.Admin.User;
using DataLayer.Models;
using DataLayer.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Filters;
using ServiceLayer.Services.Api;
using ServiceLayer.Utils;
using Services.Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using TestMultipart.ModelBinding;
using static System.Formats.Asn1.AsnWriter;

namespace IsnaNews.Controllers
{
    [SetUserInfo]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly Core _core;
        private readonly IConfiguration _configuration;

        public AdminController(IAdminService adminService, Core core, IConfiguration configuration)
        {
            _adminService = adminService;
            _core = core;
            _configuration = configuration;
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.NewsPermission.Default })]
        public IActionResult NewsList(int? p)
        {
            p = p ?? 1;
            ViewBag.pageCount = p <= 1 ? 1 : p;
            var newsList = _adminService.GetNewsList().Result;
            var model = newsList.Skip((p.Value - 1) * 10).Take(10).ToList();
            foreach (var item in model)
            {
                item.Body = item.Body.ToString().ToShortBody(15);
            }
            ViewBag.AllPageCount = newsList.Count == 0 || newsList.Count == 10 ? 1 :
                Convert.ToInt32(Math.Ceiling(a: newsList.Count / 10)) + 1;
            var dataTable = Extentions.BuildDataTable<AdminNewsDto>(model, "لیست کاربران");

            return View("NewsDataTable", dataTable);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.UsersPermission.Default })]
        public IActionResult UsersList(int? p)
        {
            p = p ?? 1;
            ViewBag.pageCount = p <= 1 ? 1 : p;

            var usersList = _adminService.GetUsersList().Result;
            var model = usersList.OrderBy(_ => _.Id)
                .Skip((p.Value - 1) * 10).Take(10).ToList();
            ViewBag.AllPageCount = usersList.Count == 0 || usersList.Count == 10 ? 1 :
                Convert.ToInt32(Math.Ceiling(a: usersList.Count / 10)) + 1;

            var dataTable = Extentions.BuildDataTable<AdminUserDto>(model, "لیست کاربران");
            return View("UserDataTable", dataTable);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Default })]
        public IActionResult AboutUs(int? p)
        {
            p = p ?? 1;
            ViewBag.pageCount = p <= 1 ? 1 : p;

            var aboutUsList = _adminService.GetAboutUsList().Result;
            var aboutUsModel = aboutUsList.Skip((p.Value - 1) * 10).Take(10).ToList();
            ViewBag.AllAboutUsPageCount = aboutUsList.Count == 0 || aboutUsList.Count == 10 ? 1 :
                Convert.ToInt32(Math.Ceiling(a: aboutUsList.Count / 10)) + 1;
            DataTable data = Extentions.BuildDataTable<AdminAboutUsDto>(aboutUsModel, "لیست درباره ما");
            return View("AboutUsDataTable", data);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Default })]

        public IActionResult ContactUs(int? p)
        {
            p = p ?? 1;
            ViewBag.pageCount = p <= 1 ? 1 : p;

            var contactUsList = _adminService.GetContactUsList().Result;
            var contactUsModel = contactUsList.Skip((p.Value - 1) * 10).Take(10).ToList();
            ViewBag.AllContactUsPageCount = contactUsList.Count == 0 || contactUsList.Count == 10 ? 1 :
                Convert.ToInt32(Math.Ceiling(a: contactUsList.Count / 10)) + 1;
            DataTable data = Extentions.BuildDataTable<AdminContactUsDto>(contactUsModel, "لیست ارتباط با ما");
            return View("ContactUsDataTable", data);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.KeywordPermission.Default })]
        public IActionResult Keywords(int? p)
        {

            p = p ?? 1;
            ViewBag.pageCount = p <= 1 ? 1 : p;

            var keywordList = _adminService.GetKewordList().Result;
            var model = keywordList.OrderBy(_ => _.Id).Skip((p.Value - 1) * 10).Take(10).ToList();

            ViewBag.AllPageCount = keywordList.Count == 0 || keywordList.Count == 10 ? 1 :
                Convert.ToInt32(Math.Ceiling(a: keywordList.Count / 10)) + 1;
            var dataTable = Extentions.BuildDataTable<AdminKeywordDto>(model, "لیست کلمات کلیدی");
            return View("KeywordsDataTable", dataTable);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.RolesPermission.Default })]
        public IActionResult Roles(int? p)
        {
            p = p ?? 1;
            ViewBag.pageCount = p <= 1 ? 1 : p;

            var Roles = _adminService.GetRoleList().Result;
            var model = Roles
                .Skip((p.Value - 1) * 10).Take(10).ToList();
            ViewBag.AllPageCount = Roles.Count == 0 || Roles.Count == 10 ? 1 :
                Convert.ToInt32(Math.Ceiling(a: Roles.Count / 10)) + 1;
            var dataTable = Extentions.BuildDataTable<AdminRoleDto>(model, "لیست نقش ها");
            return View("RolesDataTable", dataTable);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.AdvertisementPermission.Default })]

        public IActionResult AdvertisemntsList(int? p)
        {
            p = p ?? 1;
            ViewBag.pageCount = p <= 1 ? 1 : p;

            var advertisemntsList = _adminService.GetAdminAdvertisementList().Result;
            var model = advertisemntsList.Skip((p.Value - 1) * 10).Take(10).ToList();

            ViewBag.AllPageCount = advertisemntsList.Count == 0 || advertisemntsList.Count == 10 ? 1 :
                Convert.ToInt32(Math.Ceiling(a: advertisemntsList.Count / 10)) + 1;
            var dataTable = Extentions.BuildDataTable<AdminAdvertisementDto>(model, "لیست تبلیغات");
            return View("AdvertisemntsDataTable", dataTable);
        }

        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.CategoryPermission.Default })]
        public IActionResult CategoryList(int? p)
        {
            p = p ?? 1;
            ViewBag.pageCount = p <= 1 ? 1 : p;

            var categoryList = _adminService.GetCategoryList().Result;
            var model = categoryList.OrderBy(_ => _.Id)
                .Skip((p.Value - 1) * 10).Take(10).ToList();
            ViewBag.AllPageCount = categoryList.Count == 0 || categoryList.Count == 10 ? 1 :
                Convert.ToInt32(Math.Ceiling(a: categoryList.Count / 10)) + 1;

            var dataTable = Extentions.BuildDataTable<AdminCategoryDto>(model, "لیست دسته بندی ها");
            return View("CategoryDataTable", dataTable);
        }


        ///////////////////////////
        //--Actions--

        //--Advertisemnt
        [HttpGet]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.AdvertisementPermission.Add })]
        public IActionResult GetAddModalAdvertisemnt()
        {
            return PartialView("AdminAdvertisementModal", null);
        }


        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.AdvertisementPermission.Update })]

        [HttpGet]
        public IActionResult GetEditModalAdvertisemnt([FromQuery] int id)
        {
            var result = _adminService.GetAdvertisementById(id);
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }
            return PartialView("AdminAdvertisementModal", result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.AdvertisementPermission.Add })]

        [ValidateAntiForgeryToken]
        public IActionResult AddAdvertisemnt(AdminAdvertisementCreateUpdateDto advertisement)
        {
            var result = _adminService.AddAdvertisementAsync(advertisement).Result;
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.AdvertisementPermission.Update })]

        [ValidateAntiForgeryToken]
        public IActionResult UpdateAdvertisemnt(AdminAdvertisementCreateUpdateDto advertisement, int Id)
        {
            var result = _adminService.UpdateAdvertisementAsync(advertisement, Id).Result;
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.AdvertisementPermission.Delete })]

        public IActionResult DeleteAdvertisemnt([FromQuery] int id)
        {
            var result = _adminService.DeleteAdvertisement(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }

        //--Roles
        [HttpGet]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.RolesPermission.Add })]

        public IActionResult GetAddModalRole()
        {
            var permissions = _core.Permissions.Get().ToList();
            (List<(int, string)> RolePermission, List<TblRolePermissions> AllPermissions) model = (null, permissions);
            return PartialView("PermissionSelectModal", model);
        }
        [HttpGet]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.RolesPermission.Update })]

        public IActionResult GetEditModalRole([FromQuery] int id)
        {
            var result = _adminService.GetRoleById(id);
            var permissions = _core.Permissions.Get().ToList();
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }
            ViewBag.RoleId = id;
            ViewBag.RoleName = result.Result.Name;
            (List<(int, string)> RolePermission, List<TblRolePermissions> AllPermissions) model = (result.Result.Permissions, permissions);
            return PartialView("PermissionSelectModal", model);
        }
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.RolesPermission.Add })]

        public IActionResult AddRole(AdminRoleCreateUpdateDto role)
        {
            var result = _adminService.AddRole(role);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.RolesPermission.Update })]

        [ValidateAntiForgeryToken]
        public IActionResult UpdateRole(AdminRoleCreateUpdateDto role, int Id)
        {
            var result = _adminService.UpdateRole(role, Id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.RolesPermission.Delete })]

        public IActionResult DeleteRole([FromQuery] int id)
        {
            var result = _adminService.DeleteRole(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        //--KeyWords
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.KeywordPermission.Add })]
        [HttpGet]
        public IActionResult GetAddModalKeyword()
        {
            ViewBag.Title = "افزودن کلمه کلیدی جدید";
            return PartialView("OneBodyModal", null);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.KeywordPermission.Update })]
        [HttpGet]
        public IActionResult GetEditModalKeyword([FromQuery] int id)
        {
            var result = _adminService.GetKeywordById(id);
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }

            ViewBag.Title = "ویرایش کلمه کلیدی";
            return PartialView("OneBodyModal", (result.Result.Body, result.Result.Id));
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.KeywordPermission.Add })]
        [ValidateAntiForgeryToken]
        public IActionResult AddKeyword(AdminKeyWordCreateUpdateDto keyword)
        {
            var result = _adminService.AddKeyword(keyword);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.KeywordPermission.Update })]

        [ValidateAntiForgeryToken]
        public IActionResult UpdateKeyword(AdminKeyWordCreateUpdateDto keyword, int Id)
        {
            var result = _adminService.UpdateKeyword(keyword, Id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.KeywordPermission.Delete })]

        public IActionResult DeleteKeyword([FromQuery] int id)
        {
            var result = _adminService.DeleteKeyword(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        //--Users
        [HttpGet]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.UsersPermission.Add })]

        public IActionResult GetAddModalUser()
        {
            return PartialView("UserModal", null);
        }
        [HttpGet]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.UsersPermission.Update })]

        public IActionResult GetEditModalUser([FromQuery] int id)
        {
            var result = _adminService.GetUserById(id);
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }

            return PartialView("UserModal", result.Result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.UsersPermission.Add })]

        [ValidateAntiForgeryToken]
        public IActionResult AddUser(AdminUserCreateUpdateDto user)
        {
            var result = _adminService.AddUserAsync(user).Result;
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.UsersPermission.Update })]

        public IActionResult UpdateUser(AdminUserCreateUpdateDto user, int Id)
        {
            string currentUserName = (string)TempData["UserName"];
            int currentUserId = (int)TempData["UserId"];
            var result = _adminService.UpdateUserAsync(user, Id).Result;
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            if (Id == currentUserId)
            {
                var token = GenerateJwtToken(user.UserName);
                HttpContext.Response.Cookies.Append("Token", token, new CookieOptions { Expires = DateTime.Now.AddMinutes(20) });
            }
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.UsersPermission.Delete })]

        [HttpDelete]
        public IActionResult DeleteUser([FromQuery] int id)
        {
            int currentUserId = (int)TempData["UserId"];
            if (currentUserId == id)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(new AdminNoneQueryResult("نمیتوانید خود را حذف کنید"));
            }
            var result = _adminService.DeleteUser(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        //--AboutUs
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Add })]

        [HttpGet]
        public IActionResult GetAddModalAboutUs()
        {
            ViewBag.Title = "افزودن متن درباره ما جدید";
            return PartialView("OneBodyModal", null);
        }
        [HttpGet]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Update })]

        public IActionResult GetEditModalAboutUs([FromQuery] int id)
        {
            var result = _adminService.GetAboutUsById(id);
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }

            ViewBag.Title = "افزودن متن ارتباط با ما جدید";
            return PartialView("OneBodyModal", (result.Result.Body, result.Result.Id));
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Add })]

        [ValidateAntiForgeryToken]
        public IActionResult AddAboutUs(string body)
        {
            var result = _adminService.AddAboutUs(body);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Update })]

        [ValidateAntiForgeryToken]
        public IActionResult UpdateAboutUs(string body, int Id)
        {
            var result = _adminService.UpdateAboutUs(body, Id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Delete })]

        public IActionResult DeleteAboutUs([FromQuery] int id)
        {
            var result = _adminService.DeleteAboutUs(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        //--ContactUs
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Add })]
        [HttpGet]
        public IActionResult GetAddModalContactUs()
        {
            ViewBag.Title = "افزودن کلمه کلیدی جدید";
            return PartialView("OneBodyModal", null);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Update })]
        [HttpGet]
        public IActionResult GetEditModalContactUs([FromQuery] int id)
        {
            var result = _adminService.GetContactUsById(id);
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }

            ViewBag.Title = "ویرایش کلمه کلیدی";
            return PartialView("OneBodyModal", (result.Result.Body, result.Result.Id));
        }
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Add })]

        public IActionResult AddContactUs(string body)
        {
            var result = _adminService.AddContactUs(body);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Update })]

        public IActionResult UpdateContactUs(string body, int Id)
        {
            var result = _adminService.UpdateContactUs(body, Id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.ConfigPermission.Delete })]
        public IActionResult DeleteContactUs([FromQuery] int id)
        {
            var result = _adminService.DeleteContactUs(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        //--News
        [HttpGet]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.NewsPermission.Add })]
        public IActionResult GetAddModalNews()
        {
            return PartialView("AddNewsModal", null);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.NewsPermission.Update })]
        [HttpGet]
        public IActionResult GetEditModalNews([FromQuery] int Id)
        {
            var result = _adminService.GetNewsById(Id);
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }
            return PartialView("EditNewsModal", result.Result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.NewsPermission.Add })]
        [HttpPost]
        [RequestSizeLimit(1073741824)]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]

        public IActionResult AddNews(AdminNewsCreateUpdateDto news)
        {
            var result = _adminService.AddNewsAsync(news).Result;
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.NewsPermission.Update })]
        [HttpPost]
        [RequestSizeLimit(1073741824)]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        public IActionResult UpdateNews(AdminNewsCreateUpdateDto news, int Id)
        {
            var result = _adminService.UpdateNewsAsync(news, Id).Result;
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.NewsPermission.Delete })]
        [HttpDelete]
        public IActionResult DeleteNews(int Id)
        {
            var result = _adminService.DeleteNews(Id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.CommentPermission.Delete })]
        [HttpDelete]
        public IActionResult DeleteComment([FromQuery] int Id)
        {
            var result = _adminService.DeleteComment(Id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.CommentPermission.Reply })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReplyComment(AdminCommentReplyDto comment)
        {
            string userName = User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
            if (!_core.User.Any(_ => _.UserName == userName))
            {
                HttpContext.Response.StatusCode = 400;
                return Json("کاربر پاسخ دهنده نامعتبر است");
            }
            int userId = _core.User.Get(_ => _.UserName == userName).FirstOrDefault().Id;
            string HostName = Dns.GetHostName();
            IPAddress[] IPAddress = Dns.GetHostEntry(HostName).AddressList;
            string Ip = IPAddress[1].ToString();
            comment.IP = Ip;
            var result = _adminService.ReplyComment(comment, userId);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.NewsPermission.Update })]
        [HttpDelete]
        public IActionResult DeleteNewsImageRel([FromQuery] int id)
        {
            var result = _adminService.DeleteImageRel(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.NewsPermission.Update })]
        [HttpDelete]
        public IActionResult DeleteNewsVideoRel([FromQuery] int id)
        {
            var result = _adminService.DeleteVideoRel(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }


        //--Categories
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.CategoryPermission.Add })]
        [HttpGet]
        public IActionResult GetAddModalCategory()
        {
            return PartialView("CategoryModal", null);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.CategoryPermission.Update })]
        [HttpGet]
        public IActionResult GetEditModalCategory([FromQuery] int id)
        {
            var result = _adminService.GetCategoryById(id);
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }

            return PartialView("CategoryModal", result.Result);
        }
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.CategoryPermission.Add })]

        public IActionResult AddCategory(AdminCategoryCreateUpdateDto dto)
        {
            var result = _adminService.AddCategory(dto);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [ValidateAntiForgeryToken]
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.CategoryPermission.Update })]

        public IActionResult UpdateCategory(AdminCategoryCreateUpdateDto dto, int Id)
        {
            var result = _adminService.UpdateCategory(dto, Id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { AdminPermissions.CategoryPermission.Delete })]
        [HttpDelete]
        public IActionResult DeleteCategory([FromQuery] int id)
        {
            var result = _adminService.DeleteCategory(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
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
