using DataLayer.Dtos.Admin;
using DataLayer.Dtos.Admin.Advertisement;
using DataLayer.Dtos.Admin.Base;
using DataLayer.Dtos.Admin.Comment;
using DataLayer.Dtos.Admin.Config;
using DataLayer.Dtos.Admin.Keyword;
using DataLayer.Dtos.Admin.News;
using DataLayer.Dtos.Admin.User;
using DataLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Services.Api;
using ServiceLayer.Utils;
using Services.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceLayer.Services.Impl
{
    public class AdminService : IAdminService
    {

        private readonly Core _core;

        public AdminService(Core core)
        {
            _core = core;
        }



        //-----Users----
        /// <summary>
        /// Map Values Of TblUser to AdminUserDto to show on dataTable
        /// </summary>
        /// <param name="tblUser"></param>
        /// <returns></returns>
        private AdminUserDto MapUserDto(TblUser tblUser)
        {
            var result = new AdminUserDto()
            {
                Id = tblUser.Id,
                Name = tblUser.Name,
                RoleName = _core.Role.GetById(tblUser.RoleId).Name,
                Tell = tblUser.Tell,
                UserName = tblUser.UserName,
                ProfileImageUrl = _core.Image.GetById(tblUser.ProfileImageId).ImageUrl,
                LastLoginDate = tblUser.LastLoginDate.ToPersianDate(),
                DateSigned = tblUser.DateSigned.ToPersianDate(),
                permissions = GetUsersPermissions(tblUser.Id).Result.Permissions
            };
            return result;
        }
        public AdminReadAllQueryResult<AdminUserDto> GetUsersList()
        {
            return new AdminReadAllQueryResult<AdminUserDto>(_core.User.Get().Select(MapUserDto).ToList());
        }
        public AdminReadByIdQueryResult<AdminUserDto> GetUserById(int id)
        {
            if (!_core.User.Any(x => x.Id == id))
            {
                return new AdminReadByIdQueryResult<AdminUserDto>("کاربر یافت نشد");
            }
            return new AdminReadByIdQueryResult<AdminUserDto>(MapUserDto(_core.User.GetById(id)));
        }
        public AdminNoneQueryResult AddUser(AdminUserCreateUpdateDto user)
        {
            var modelValidateResult = Extentions.ValidateModel(user);
            if (!modelValidateResult.IsValid)
            {
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);
            }
            var errors = new List<string>();
            if (_core.User.Any(_ => _.Tell == user.Tell))
            {
                errors.Add("این شماره تلفن از قبل وجود دارد");
            }
            if (_core.User.Any(_ => _.UserName == user.UserName))
            {
                errors.Add("این نام کاربری از قبل وجود دارد");
            }
            if (errors.Count > 0)
            {
                return new AdminNoneQueryResult(errors);
            }
            try
            {
                TblUser tblUser = new()
                {
                    Name = user.Name,
                    UserName = user.UserName,
                    Tell = user.Tell,
                    Password = user.Password.HashData(),
                    RoleId = user.RoleId ?? 1,
                    ProfileImageId = 1
                };
                _core.User.Add(tblUser);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }

        public AdminNoneQueryResult UpdateUser(AdminUserCreateUpdateDto user, int userId)
        {
            var modelValidateResult = Extentions.ValidateModel(user);
            if (!modelValidateResult.IsValid)
            {
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);
            }
            if (!_core.User.Any(_ => _.Id == userId))
            {
                return new AdminNoneQueryResult("کاربر یافت نشد");
            }
            var errors = new List<string>();
            if (_core.User.Any(_ => _.Tell == user.Tell && _.Id != userId))
            {
                errors.Add("این شماره تلفن از قبل وجود دارد");
            }
            if (_core.User.Any(_ => _.UserName == user.UserName && _.Id != userId))
            {
                errors.Add("این نام کاربری از قبل وجود دارد");
            }
            if (errors.Count > 0)
            {
                return new AdminNoneQueryResult(errors);
            }
            try
            {
                TblUser tblUser = new()
                {
                    Id = userId,
                    Name = user.Name,
                    UserName = user.UserName,
                    Tell = user.Tell,
                    Password = user.Password.HashData(),
                    RoleId = user.RoleId ?? 1,
                    //TODO
                    ProfileImageId = 1
                };
                _core.User.Update(tblUser);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }

        public AdminNoneQueryResult DeleteUser(int userId)
        {
            if (!_core.User.Any(_ => _.Id == userId))
            {
                return new AdminNoneQueryResult("کاربر یافت نشد");
            }
            try
            {
                _core.User.DeleteById(userId);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }

        /// <summary>
        /// Gets User Permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public AdminReadByIdQueryResult<AdminUsersPermissionsDto> GetUsersPermissions(int userId)
        {
            var tblUser = _core.User.GetById(userId);
            var rolePermissions = _core.RolePermissionsRel.Get(i => i.RoleId == tblUser.RoleId).Select(i =>
            {
                var permission = _core.Permissions.GetById(i.PermissionId);
                return permission.Name;
            }).ToList();
            var userPermissions = new AdminUsersPermissionsDto(rolePermissions);
            return new AdminReadByIdQueryResult<AdminUsersPermissionsDto>(userPermissions);
        }

        //-----News----
        /// <summary>
        /// Maps TblNews To AdminNewsDto To Show on DataTable
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        private AdminNewsDto MapNewsDto(TblNews news)
        {
            var result = new AdminNewsDto()
            {
                Body = news.Body,
                Title = news.Title,
                ReporterId = news.ReporterId,
                DatePosted = news.DatePosted.ToPersianDate(),
                IsImportantNews = news.IsImportantNews,
                ViewCount = news.ViewCount,
            };
            if (news.Id != null)
            {
                result.MainImageUrl = _core.Image.GetById(news.MainImageId).ImageUrl;
                result.ImageUrls = _core.NewsImageRel.Get(includes: "Image", where: i => i.NewsId == news.Id).Select(_ => _.Image.ImageUrl).ToList();
                result.VideoUrls = _core.NewsVideoRel.Get(includes: "Video", where: i => i.NewsId == news.Id).Select(_ => _.Video.VideoUrl).ToList();
                result.Keyword = _core.NewsKeywordRel.Get(includes: "KeyWord", where: i => i.NewsId == news.Id).Select(_ => _.KeyWord.Body).ToList();
                result.Comments = _core.Comment.Get(includes: "User").Select(i =>
                {
                    (string userName, string UserProfile, long Id, string? Reply) r;
                    r.userName = i.User.UserName;
                    r.UserProfile = _core.Image.GetById(i.User.ProfileImageId).ImageUrl;
                    r.Id = i.Id;
                    r.Reply = null;
                    if (i.InverseParent.Count > 0)
                        r.Reply = i.InverseParent.First().Body;
                    return r;
                }).ToList();
            }
            return result;
        }

        public AdminReadAllQueryResult<AdminNewsDto> GetNewsList()
        {
            return new AdminReadAllQueryResult<AdminNewsDto>(_core.News.Get().Select(MapNewsDto).ToList());
        }

        public AdminReadByIdQueryResult<AdminNewsDto> GetNewsById(long id)
        {
            if (!_core.News.Any(i => i.Id == id))
            {
                return new AdminReadByIdQueryResult<AdminNewsDto>("خبر مورد نظر یافت نشد");
            }
            return new AdminReadByIdQueryResult<AdminNewsDto>(MapNewsDto(_core.News.GetById(id)));
        }
        //TODO Fix Images
        public async Task<AdminNoneQueryResult> AddNewsAsync(AdminNewsCreateUpdateDto dto)
        {
            var modelValidateResult = Extentions.ValidateModel(dto);
            if (!modelValidateResult.IsValid)
            {
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);
            }
            var errors = new List<string>();
            if (_core.News.Any(_ => _.Title == dto.Title))
                errors.Add("خبری با این تیتر وجود دارد");
            if (!_core.News.Any(_ => _.CategoryId == dto.CategoryId))
                errors.Add("دسته بندی مورد نظر پیدا نشد");
            if (!_core.User.Any(_ => _.Id == dto.ReporterId))
                errors.Add("خبر نگار مورد نظر پیدا نشد");
            foreach (var item in dto.Keyword)
            {
                if (!_core.Keyword.Any(_ => _.Body == item))
                {
                    errors.Add($"کلمه کلیدی {item} پیدا نشد");
                }
            }
            if (errors.Count > 0)
            {
                return new AdminNoneQueryResult(Errors: errors);
            }
            try
            {
                if (!dto.MainImageUrl.IsAllowedFormat(new string[] { "jpg", "png" }))
                {
                    return new AdminNoneQueryResult("خطا در پردازش عکس اصلی");
                }
                _core.Image.Add(new TblImage { ImageUrl = dto.MainImageUrl });
                _core.Save();
                var MainImageId = _core.Image.Get().OrderBy(x => x.Id).LastOrDefault().Id;
                TblNews tblNews = new()
                {
                    Body = dto.Body,
                    DatePosted = dto.DatePosted,
                    ReporterId = dto.ReporterId,
                    Title = dto.Title,
                    CategoryId = dto.CategoryId,
                    IsImportantNews = dto.IsImportantNews,
                    ViewCount = 0
                };
                _core.News.Add(tblNews);
                _core.Save();
                var newsId = _core.News.Get().OrderBy(x => x.Id).LastOrDefault().Id;
                //Media
                var addImageResult = AddImage(dto, newsId);
                if (!addImageResult.Success)
                {
                    return new AdminNoneQueryResult(addImageResult.Error);
                }
                var addVideoResult = AddVideo(dto, newsId);
                if (!addVideoResult.Success)
                {
                    return new AdminNoneQueryResult(addVideoResult.Error);
                }
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        //TODO Fix Images
        public AdminNoneQueryResult UpdateNews(AdminNewsCreateUpdateDto dto, long id)
        {
            var modelValidateResult = Extentions.ValidateModel(dto);
            if (!modelValidateResult.IsValid)
            {
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);
            }
            if (!_core.News.Any(x => x.Id == id))
                return new AdminNoneQueryResult("خبر مورد نظر یافت نشد");
            var errors = new List<string>();
            if (_core.News.Any(_ => _.Title == dto.Title))
                errors.Add("خبری با این تیتر وجود دارد");
            if (!_core.Category.Any(_ => _.Id == dto.CategoryId))
                errors.Add("دسته بندی مورد نظر پیدا نشد");
            if (!_core.User.Any(_ => _.Id == dto.ReporterId))
                errors.Add("خبر نگار مورد نظر پیدا نشد");
            foreach (var item in dto.Keyword)
            {
                if (!_core.Keyword.Any(_ => _.Body == item))
                {
                    errors.Add($"کلمه کلیدی {item} پیدا نشد");
                }
            }
            if (errors.Count > 0)
            {
                return new AdminNoneQueryResult(Errors: errors);
            }
            try
            {
                if (!dto.MainImageUrl.IsAllowedFormat(new string[] { "jpg", "png" }))
                {
                    return new AdminNoneQueryResult("خطا در پردازش عکس اصلی");
                }
                _core.Image.Add(new TblImage { ImageUrl = dto.MainImageUrl });
                _core.Save();
                var MainImageId = _core.Image.Get().OrderBy(x => x.Id).LastOrDefault().Id;
                TblNews tblNews = new()
                {
                    Id = id,
                    Body = dto.Body,
                    DatePosted = dto.DatePosted,
                    ReporterId = dto.ReporterId,
                    MainImageId = MainImageId,
                    Title = dto.Title,
                    CategoryId = dto.CategoryId,
                    IsImportantNews = dto.IsImportantNews,
                };
                _core.Image.DeleteById(_core.News.GetById(id).MainImageId);
                _core.Save();
                _core.News.Update(tblNews);
                _core.Save();
                var newsId = _core.News.Get().OrderBy(x => x.Id).LastOrDefault().Id;
                //Media
                var addImageResult = AddImage(dto, newsId);
                if (!addImageResult.Success)
                {
                    return new AdminNoneQueryResult(addImageResult.Error);
                }
                var addVideoResult = AddVideo(dto, newsId);
                if (!addVideoResult.Success)
                {
                    return new AdminNoneQueryResult(addVideoResult.Error);
                }
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminNoneQueryResult DeleteNews(long id)
        {
            if (!_core.News.Any(_ => _.Id == id))
            {
                return new AdminNoneQueryResult("خبر مورد نظر یافت نشد");
            }
            _core.News.DeleteById(id);
            _core.Save();
            return new AdminNoneQueryResult();
        }

        //TODO Fix Images
        /// <summary>
        /// Adds Non Main Images of News
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        private (bool Success, List<string>? Error) AddImage(AdminNewsCreateUpdateDto dto, long newsId)
        {
            //Images
            var errors = new List<string>();
            for (int i = 0; i < dto.ImageUrls.Count; i++)
            {
                var url = dto.ImageUrls[i];
                if (!url.IsAllowedFormat(new string[] { "png", "jpg" }))
                {
                    errors.Add($"عکس فرعی شماره {i} نا معتبر است");
                }
            }
            if (errors.Count > 0)
            {
                return (false, errors);
            }
            for (int i = 0; i < dto.ImageUrls.Count; i++)
            {
                var url = dto.ImageUrls[i];
                _core.Image.Add(new TblImage { ImageUrl = url });
                _core.Save();
                var imageId = _core.Image.Get().OrderBy(_ => _.Id).LastOrDefault().Id;
                _core.NewsImageRel.Add(new TblNewsImageRel { ImageId = imageId, NewsId = newsId });
                _core.Save();
            }
            return (true, null);
        }
        //TODO Fix Videos
        /// <summary>
        /// Adds Videos of News
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        private (bool Success, List<string>? Error) AddVideo(AdminNewsCreateUpdateDto dto, long newsId)
        {
            var errors = new List<string>();
            for (int i = 0; i < dto.VideoUrls.Count; i++)
            {
                var url = dto.VideoUrls[i];
                if (!url.IsAllowedFormat(new string[] { "png", "jpg" }))
                {
                    errors.Add($"ویدئو شماره {i} نا معتبر است");
                }
            }
            if (errors.Count > 0)
            {
                return (false, errors);
            }

            //Videos
            for (int i = 0; i < dto.VideoUrls.Count; i++)
            {
                var url = dto.VideoUrls[i];
                _core.Video.Add(new TblVideo { VideoUrl = url });
                _core.Save();
                var videoId = _core.Video.Get().OrderBy(_ => _.Id).LastOrDefault().Id;
                _core.NewsVideoRel.Add(new TblNewsVideoRel { VideoId = videoId, NewsId = newsId });
                _core.Save();
            }
            return (true, null);
        }
        //-----Comments----
        public AdminNoneQueryResult DeleteComment(long id)
        {
            if (!_core.Comment.Any(_ => _.Id == id))
            {
                return new AdminNoneQueryResult("کامنت مورد نظر یافت نشد");
            }

            try
            {
                TblNewsComment comment = _core.Comment.GetById(id);
                if (comment.InverseParent != null)
                {
                    _core.Comment.DeleteById(comment.InverseParent.First().Id);
                    _core.Save();
                }
                _core.Comment.Delete(comment);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminNoneQueryResult ReplyComment(AdminCommentReplyDto replyDto, int SupporterId)
        {
            if (!_core.Comment.Any(_ => _.Id == replyDto.ReplyingCommentId))
                return new AdminNoneQueryResult("کامنت مورد نظر یافت نشد");
            try
            {
                TblNewsComment comment = new TblNewsComment() { Body = replyDto.Body, ParentId = replyDto.ReplyingCommentId, UserId = SupporterId };
                _core.Comment.Add(comment);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        //-----Role----
        public AdminNoneQueryResult AddRole(AdminRoleCreateUpdateDto roleDto)
        {
            var modelValidateResult = Extentions.ValidateModel(roleDto);
            if (!modelValidateResult.IsValid)
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);

            List<string> errors = new();
            foreach (var i in roleDto.PermissionIds)
            {
                if (!_core.Permissions.Any(_ => _.Id == i))
                {
                    errors.Add($"دسترسی با کد {i} پیدا نشد!");
                }
            }
            if (errors.Count > 0)
            {
                return new AdminNoneQueryResult(Errors: errors);
            }

            try
            {
                TblRole role = new TblRole { Name = roleDto.Name };
                _core.Role.Add(role);
                _core.Save();
                int roleId = _core.Role.Get().OrderBy(x => x.Id).LastOrDefault().Id;
                foreach (var i in roleDto.PermissionIds)
                {
                    TblRoleRolePermissionsRel permissionsRel = new TblRoleRolePermissionsRel() { PermissionId = i, RoleId = roleId };
                    _core.RolePermissionsRel.Add(permissionsRel);
                    _core.Save();
                }
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }

        }
        public AdminNoneQueryResult UpdateRole(AdminRoleCreateUpdateDto roleDto, int roleId)
        {
            if (roleId == 4)
                return new AdminNoneQueryResult("نقش اصلی ادمین قابل تغییر نیست");


            var modelValidateResult = Extentions.ValidateModel(roleDto);
            if (!modelValidateResult.IsValid)
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);


            List<string> errors = new();
            if (!_core.Role.Any(_ => _.Id == roleId))
            {
                errors.Add("نقش مورد نظر پیدا نشد");
            }
            foreach (var i in roleDto.PermissionIds)
            {
                if (!_core.Permissions.Any(_ => _.Id == i))
                {
                    errors.Add($"دسترسی با کد {i} پیدا نشد!");
                }
            }
            if (errors.Count > 0)
            {
                return new AdminNoneQueryResult(Errors: errors);
            }

            try
            {
                TblRole role = new TblRole { Name = roleDto.Name, Id = roleId };
                _core.Role.Update(role);
                _core.Save();
                foreach (var i in roleDto.PermissionIds)
                {
                    TblRoleRolePermissionsRel permissionsRel = new TblRoleRolePermissionsRel() { PermissionId = i, RoleId = roleId };
                    _core.RolePermissionsRel.Add(permissionsRel);
                    _core.Save();
                }
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminNoneQueryResult DeleteRole(int roleId)
        {
            if (roleId == 4)
                return new AdminNoneQueryResult("نقش اصلی ادمین قابل تغییر نیست");

            if (!_core.Role.Any(_ => _.Id == roleId))
                return new AdminNoneQueryResult("نقش مورد نظر یافت نشد");

            try
            {
                _core.Role.DeleteById(roleId);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminReadByIdQueryResult<AdminRoleDto> GetRoleById(int roleId)
        {
            if (!_core.Role.Any(_ => _.Id == roleId))
                return new AdminReadByIdQueryResult<AdminRoleDto>(error: "نقش مورد نظر یافت نشد");
            try
            {
                TblRole tblRole = _core.Role.Get(_ => _.Id == roleId, includes: "TblRoleRolePermissionsRel").FirstOrDefault();
                AdminRoleDto result = new AdminRoleDto()
                {
                    Name = tblRole.Name,
                    Permissions = tblRole.TblRoleRolePermissionsRel.Select(i =>
                    {
                        (int Id, string Name) result;
                        result.Id = i.PermissionId;
                        result.Name = _core.Permissions.GetById(i.PermissionId).Name;
                        return result;
                    }).ToList()
                };
                return new AdminReadByIdQueryResult<AdminRoleDto>(result: result);
            }
            catch (Exception ex)
            {
                return new AdminReadByIdQueryResult<AdminRoleDto>(ex.Message);
            }
        }
        public AdminReadAllQueryResult<AdminRoleDto> GetRoleList()
        {
            List<TblRole> tblRoles = _core.Role.Get(includes: "TblRoleRolePermissionsRel").ToList();
            List<AdminRoleDto> result = new List<AdminRoleDto>();
            foreach (var i in tblRoles)
            {
                AdminRoleDto dto = new AdminRoleDto()
                {
                    Name = i.Name,
                    Permissions = i.TblRoleRolePermissionsRel.Select(j =>
                    {
                        (int Id, string Name) result;
                        result.Id = j.PermissionId;
                        result.Name = _core.Permissions.GetById(j.PermissionId).Name;
                        return result;
                    }).ToList()
                };
                result.Add(dto);
            }
            return new AdminReadAllQueryResult<AdminRoleDto>(result);
        }

        //---Advertisement
        public AdminNoneQueryResult AddAdvertisement(AdminAdvertisementCreateUpdateDto dto)
        {
            var modelValidateResult = Extentions.ValidateModel(dto);
            if (!modelValidateResult.IsValid)
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);

            try
            {
                //TODO Fix Images
                _core.Image.Add(new TblImage() { ImageUrl = dto.MainBaner });
                _core.Save();
                var imageId = _core.Image.Get().OrderBy(x => x.Id).LastOrDefault().Id;
                TblAdvertisement tblAdvertisement = new TblAdvertisement() { MainBanerId = imageId, Link = dto.Link };
                _core.Advertisement.Add(tblAdvertisement);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }

        }

        public AdminNoneQueryResult UpdateAdvertisement(AdminAdvertisementCreateUpdateDto dto, int Id)
        {
            var modelValidateResult = Extentions.ValidateModel(dto);
            if (!modelValidateResult.IsValid)
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);

            if (_core.Advertisement.Any(_ => _.Id == Id))
                return new AdminNoneQueryResult("تبلیغ مورد نظر یافت نشد");
            try
            {
                TblAdvertisement tblAdvertisement = _core.Advertisement.GetById(Id);
                //TODO Fix Images
                _core.Image.DeleteById(tblAdvertisement.MainBanerId);
                _core.Save();
                _core.Image.Add(new TblImage() { ImageUrl = dto.MainBaner });
                _core.Save();

                var imageId = _core.Image.Get().OrderBy(x => x.Id).LastOrDefault().Id;
                tblAdvertisement.Link = dto.Link;
                tblAdvertisement.MainBanerId = imageId;

                _core.Advertisement.Update(tblAdvertisement);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }

        public AdminNoneQueryResult DeleteAdvertisement(int Id)
        {
            if (!_core.Advertisement.Any(_ => _.Id == Id))
                return new AdminNoneQueryResult("تبلیغ مورد نظر یافت نشد");
            try
            {
                _core.Advertisement.DeleteById(Id);
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }

        public AdminReadByIdQueryResult<AdminAdvertisementDto> GetAdvertisementById(int Id)
        {
            if (!_core.Advertisement.Any(_ => _.Id == Id))
                return new AdminReadByIdQueryResult<AdminAdvertisementDto>(error: "تبلیغ مورد نظر یافت نشد");
            TblAdvertisement tblAdvertisement = _core.Advertisement.Get(_ => _.Id == Id, includes: "MainBaner").FirstOrDefault();
            AdminAdvertisementDto result = new AdminAdvertisementDto() { Link = tblAdvertisement.Link, MainBaner = tblAdvertisement.MainBaner.ImageUrl };
            return new AdminReadByIdQueryResult<AdminAdvertisementDto>(result);
        }

        public AdminReadAllQueryResult<AdminAdvertisementDto> GetAdminAdvertisementList()
        {
            List<TblAdvertisement> tblAdvertisements = _core.Advertisement.Get(includes: "MainBaner").ToList();
            List<AdminAdvertisementDto> result = tblAdvertisements.Select(i =>
            {
                AdminAdvertisementDto dto = new AdminAdvertisementDto() { Link = i.Link, MainBaner = i.MainBaner.ImageUrl };
                return dto;
            }).ToList();
            return new AdminReadAllQueryResult<AdminAdvertisementDto>(result);
        }

        //----AboutUs
        public AdminNoneQueryResult AddAboutUs(string body)
        {
            if (body.IsNullOrEmpty())
                return new AdminNoneQueryResult("متن نمیتواند خالی باشد");
            if (_core.AboutUs.Any(_ => _.Value == body))
                return new AdminNoneQueryResult("این متن از قبل وجود دارد");
            try
            {
                _core.AboutUs.Add(new TblAboutUs { Value = body });
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminNoneQueryResult UpdateAboutUs(string body, int Id)
        {
            if (body.IsNullOrEmpty())
                return new AdminNoneQueryResult("متن نمیتواند خالی باشد");

            List<string> errors = new List<string>();
            if (!_core.AboutUs.Any(_ => _.Id == Id))
                errors.Add("متن مورد نظر یافت نشد");
            if (_core.AboutUs.Any(_ => _.Value == body && _.Id != Id))
                errors.Add("این متن از قبل وجود دارد");
            if (errors.Count > 0)
                return new AdminNoneQueryResult(errors);
            try
            {
                _core.AboutUs.Update(new TblAboutUs { Id = Id, Value = body });
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminNoneQueryResult DeleteAboutUs(int Id)
        {
            if (!_core.AboutUs.Any(_ => _.Id == Id))
                return new AdminNoneQueryResult("متن مورد نظر یافت نشد");
            try
            {
                _core.AboutUs.DeleteById(Id);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }

        public AdminReadByIdQueryResult<AdminAboutUsDto> GetAboutUsById(int Id)
        {
            if (!_core.AboutUs.Any(_ => _.Id == Id))
                return new AdminReadByIdQueryResult<AdminAboutUsDto>("متن مورد نظر یافت نشد");
            return new AdminReadByIdQueryResult<AdminAboutUsDto>(new AdminAboutUsDto(_core.AboutUs.GetById(Id).Value));
        }
        public AdminReadAllQueryResult<AdminAboutUsDto> GetAboutUsList()
        {
            List<TblAboutUs> tbls = _core.AboutUs.Get().ToList();
            List<AdminAboutUsDto> result = tbls.Select(i => new AdminAboutUsDto(i.Value)).ToList();
            return new AdminReadAllQueryResult<AdminAboutUsDto>(result);
        }

        //----ContactUs
        public AdminNoneQueryResult AddContactUs(string body)
        {
            if (body.IsNullOrEmpty())
                return new AdminNoneQueryResult("متن نمیتواند خالی باشد");
            if (_core.ContactUs.Any(_ => _.Value == body))
                return new AdminNoneQueryResult("این متن از قبل وجود دارد");
            try
            {
                _core.ContactUs.Add(new TblContactUs { Value = body });
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminNoneQueryResult UpdateContactUs(string body, int Id)
        {
            if (body.IsNullOrEmpty())
                return new AdminNoneQueryResult("متن نمیتواند خالی باشد");

            List<string> errors = new List<string>();
            if (!_core.ContactUs.Any(_ => _.Id == Id))
                errors.Add("متن مورد نظر یافت نشد");
            if (_core.ContactUs.Any(_ => _.Value == body && _.Id != Id))
                errors.Add("این متن از قبل وجود دارد");
            if (errors.Count > 0)
                return new AdminNoneQueryResult(errors);
            try
            {
                _core.ContactUs.Update(new TblContactUs { Id = Id, Value = body });
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminNoneQueryResult DeleteContactUs(int Id)
        {
            if (!_core.ContactUs.Any(_ => _.Id == Id))
                return new AdminNoneQueryResult("متن مورد نظر یافت نشد");
            try
            {
                _core.ContactUs.DeleteById(Id);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }

        public AdminReadByIdQueryResult<AdminContactUsDto> GetContactUsById(int Id)
        {
            if (!_core.ContactUs.Any(_ => _.Id == Id))
                return new AdminReadByIdQueryResult<AdminContactUsDto>("متن مورد نظر یافت نشد");
            return new AdminReadByIdQueryResult<AdminContactUsDto>(new AdminContactUsDto(_core.ContactUs.GetById(Id).Value));
        }
        public AdminReadAllQueryResult<AdminContactUsDto> GetContactUsList()
        {
            List<TblContactUs> tbls = _core.ContactUs.Get().ToList();
            List<AdminContactUsDto> result = tbls.Select(i => new AdminContactUsDto(i.Value)).ToList();
            return new AdminReadAllQueryResult<AdminContactUsDto>(result);
        }

        //---Keywords
        public AdminNoneQueryResult AddKeyword(AdminKeyWordCreateUpdateDto keyword)
        {
            if (_core.Keyword.Any(_ => _.Body == keyword.Body))
                return new AdminNoneQueryResult("این کلمه کلیدی وجود دارد");
            try
            {
                _core.Keyword.Add(new TblKeyWord { Body = keyword.Body });
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminNoneQueryResult UpdateKeyword(AdminKeyWordCreateUpdateDto keyword, int Id)
        {
            List<string> errors = new List<string>();
            if (!_core.Keyword.Any(_ => _.Id == Id))
                errors.Add("کلمه کلیدی مورد نظر یافت نشد");
            if (_core.Keyword.Any(_ => _.Id != Id && _.Body == keyword.Body))
                errors.Add("کلمه کلیدی دیگری با این متن وجود دارد");
            if (errors.Count > 0)
                return new AdminNoneQueryResult(errors);
            try
            {
                _core.Keyword.Update(new TblKeyWord { Body = keyword.Body, Id = Id });
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminNoneQueryResult DeleteKeyword(int Id)
        {
            if (!_core.Keyword.Any(_ => _.Id == Id))
                return new AdminNoneQueryResult("این کلمه کلیدی یافت نشد");
            try
            {
                _core.Keyword.DeleteById(Id);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public AdminReadByIdQueryResult<AdminKeywordDto> GetKeywordById(int Id)
        {
            if (!_core.Keyword.Any(_ => _.Id == Id))
                return new AdminReadByIdQueryResult<AdminKeywordDto>("کلمه کلیدی مورد نظر یافت نشد");
            try
            {
                TblKeyWord tblKeyWord = _core.Keyword.GetById(Id);
                AdminKeywordDto dto = new AdminKeywordDto(tblKeyWord);
                return new AdminReadByIdQueryResult<AdminKeywordDto>(dto);
            }
            catch (Exception ex)
            {
                return new AdminReadByIdQueryResult<AdminKeywordDto>(ex.Message);
                throw;
            }

        }
        public AdminReadAllQueryResult<AdminKeywordDto> GetKewordList()
        {
            List<TblKeyWord> tblKeyWords = _core.Keyword.Get().ToList();
            List<AdminKeywordDto> result = tblKeyWords.Select(i =>
            {
                return new AdminKeywordDto(i);
            }).ToList();
            return new AdminReadAllQueryResult<AdminKeywordDto>(result);
        }

    }
}
