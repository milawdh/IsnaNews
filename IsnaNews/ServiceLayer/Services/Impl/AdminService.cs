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
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Services.Api;
using ServiceLayer.Utils;
using Services.Services;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceLayer.Services.Impl
{
    public class AdminService : IAdminService
    {
        private const string ResourcePath = @"../../IsnaNews/wwwroot/Resources/";

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
        public async Task<AdminNoneQueryResult> AddUserAsync(AdminUserCreateUpdateDto userDto)
        {

            var modelValidateResult = Extentions.ValidateModel(userDto);
            if (!modelValidateResult.IsValid)
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);
            var errors = new List<string>();
            if (_core.User.Any(_ => _.Tell == userDto.Tell))
            {
                errors.Add("این شماره تلفن از قبل وجود دارد");
            }
            if (_core.User.Any(_ => _.UserName == userDto.UserName))
            {
                errors.Add("این نام کاربری از قبل وجود دارد");
            }
            if (errors.Count > 0)
                return new AdminNoneQueryResult(errors);
            try
            {
                long profileImageId = 1;
                if (userDto.ProfileImage != null)
                {
                    var addImageResult = await AddImage(userDto.ProfileImage);
                    if (!addImageResult.Success)
                        return new AdminNoneQueryResult(addImageResult.Error);
                    profileImageId = addImageResult.ImageId.Value;
                }

                TblUser tblUser = new()
                {
                    Name = userDto.Name,
                    UserName = userDto.UserName,
                    Tell = userDto.Tell,
                    Password = userDto.Password.HashData(),
                    RoleId = userDto.RoleId,
                    ProfileImageId = profileImageId
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

        public async Task<AdminNoneQueryResult> UpdateUserAsync(AdminUserCreateUpdateDto userDto, int userId)
        {
            var modelValidateResult = Extentions.ValidateModel(userDto);
            if (!modelValidateResult.IsValid)
            {
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);
            }
            if (!_core.User.Any(_ => _.Id == userId))
            {
                return new AdminNoneQueryResult("کاربر یافت نشد");
            }
            var errors = new List<string>();
            if (_core.User.Any(_ => _.Tell == userDto.Tell && _.Id != userId))
            {
                errors.Add("این شماره تلفن از قبل وجود دارد");
            }
            if (_core.User.Any(_ => _.UserName == userDto.UserName && _.Id != userId))
            {
                errors.Add("این نام کاربری از قبل وجود دارد");
            }
            if (errors.Count > 0)
            {
                return new AdminNoneQueryResult(errors);
            }
            try
            {
                long profileImageId = 1;
                if (userDto.ProfileImage != null)
                {
                    var addImageResult = await AddImage(userDto.ProfileImage);
                    if (!addImageResult.Success)
                        return new AdminNoneQueryResult(addImageResult.Error);
                    profileImageId = addImageResult.ImageId.Value;

                    var deleteImageResult = DeleteImage(_core.User.GetById(userId).ProfileImageId);
                    if (!deleteImageResult.Success)
                        return new AdminNoneQueryResult(deleteImageResult.Error);

                }
                TblUser tblUser = new()
                {
                    Id = userId,
                    Name = userDto.Name,
                    UserName = userDto.UserName,
                    Tell = userDto.Tell,
                    Password = userDto.Password.HashData(),
                    RoleId = userDto.RoleId,
                    ProfileImageId = profileImageId
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
                result.ImageUrls = _core.NewsImageRel.Get(includes: "Image", where: i => i.NewsId == news.Id).Select(_ => (_.Image.ImageUrl, _.ImageId)).ToList();
                result.VideoUrls = _core.NewsVideoRel.Get(includes: "Video", where: i => i.NewsId == news.Id).Select(_ => (_.Video.VideoUrl, _.VideoId)).ToList();
                result.Keyword = _core.NewsKeywordRel.Get(includes: "KeyWord", where: i => i.NewsId == news.Id).Select(_ => _.KeyWord.Body).ToList();
                result.Comments = _core.Comment.Get(includes: "User").Select(i =>
                {
                    (string userName, string UserProfile, long Id, string? Reply) comment;
                    comment.userName = i.User.UserName;
                    comment.UserProfile = _core.Image.GetById(i.User.ProfileImageId).ImageUrl;
                    comment.Id = i.Id;
                    comment.Reply = null;
                    if (i.InverseParent.Count > 0)
                        comment.Reply = i.InverseParent.First().Body;
                    return comment;
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
        public async Task<AdminNoneQueryResult> AddNewsAsync(AdminNewsCreateUpdateDto dto)
        {
            var modelValidateResult = Extentions.ValidateModel(dto);
            if (!modelValidateResult.IsValid)
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);
            if (dto.MainImage == null || dto.PerviuosMainImage.IsNullOrEmpty())
                return new AdminNoneQueryResult("عکس اصلی خبر خالی است");

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
                var imageAddResult = await AddImage(dto.MainImage);
                if (!imageAddResult.Success)
                    return new AdminNoneQueryResult(imageAddResult.Error);

                TblNews tblNews = new()
                {
                    Body = dto.Body,
                    DatePosted = DateTime.Now,
                    ReporterId = dto.ReporterId,
                    Title = dto.Title,
                    CategoryId = dto.CategoryId,
                    IsImportantNews = dto.IsImportantNews,
                    ViewCount = 0,
                    MainImageId = imageAddResult.ImageId.Value
                };
                _core.News.Add(tblNews);
                _core.Save();
                var newsId = _core.News.Get().OrderBy(_ => _.Id).LastOrDefault().Id;

                //Media
                foreach (var image in dto.ImageUrls)
                {
                    try
                    {
                        var addImageResult = await AddImage(image);
                        if (!addImageResult.Success)
                            return new AdminNoneQueryResult(addImageResult.Error);
                        _core.NewsImageRel.Add(new TblNewsImageRel { ImageId = addImageResult.ImageId.Value, NewsId = newsId });
                        _core.Save();
                    }
                    catch (Exception ex)
                    {
                        return new AdminNoneQueryResult("خطا در پردازش عکس فرعی" + image.FileName);
                    }
                }
                foreach (var video in dto.VideoUrls)
                {
                    try
                    {
                        var addVideoResult = await AddVideo(video);
                        if (!addVideoResult.Success)
                            return new AdminNoneQueryResult(addVideoResult.Error);
                        _core.NewsVideoRel.Add(new TblNewsVideoRel { NewsId = newsId, VideoId = addVideoResult.VideoId.Value });
                    }
                    catch (Exception ex)
                    {
                        return new AdminNoneQueryResult("خطا در پردازش ویدئو" + video.FileName);
                    }
                }
                //KeyWord Add
                foreach (var item in dto.Keyword)
                {
                    TblKeyWord tblKeyWord = _core.Keyword.Get(_ => _.Body == item).FirstOrDefault();
                    _core.NewsKeywordRel.Add(new TblNewsKeyWordRel { KeyWordId = tblKeyWord.Id, NewsId = newsId });
                    _core.Save();
                }
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }
        }
        public async Task<AdminNoneQueryResult> UpdateNewsAsync(AdminNewsCreateUpdateDto dto, long newsId)
        {
            var modelValidateResult = Extentions.ValidateModel(dto);
            if (!modelValidateResult.IsValid)
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);

            if (dto.MainImage == null && dto.PerviuosMainImage.IsNullOrEmpty())
                return new AdminNoneQueryResult("عکس اصلی خبر خالی است");

            if (!_core.News.Any(x => x.Id == newsId))
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
                TblNews tblNews = _core.News.Get(_ => _.Id == newsId, includes: "MainImage").FirstOrDefault();
                if (dto.PerviuosMainImage.Trim().Split('/')[dto.PerviuosMainImage.Trim().Split('/').Length - 1] !=
                    tblNews.MainImage.ImageUrl
                    )
                {
                    if (dto.MainImage != null)
                        return new AdminNoneQueryResult("عکس اصلی خبر خالی است");
                    var imageAddResult = await AddImage(dto.MainImage);
                    if (!imageAddResult.Success)
                        return new AdminNoneQueryResult(imageAddResult.Error);
                    var ImageDeleteResult = DeleteImage(tblNews.MainImageId);
                    if (!ImageDeleteResult.Success)
                        return new AdminNoneQueryResult(ImageDeleteResult.Error);


                    tblNews = new TblNews()
                    {
                        Id = newsId,
                        Body = dto.Body,
                        DatePosted = DateTime.Now,
                        ReporterId = dto.ReporterId,
                        MainImageId = imageAddResult.ImageId.Value,
                        Title = dto.Title,
                        CategoryId = dto.CategoryId,
                        IsImportantNews = dto.IsImportantNews,
                    };
                    _core.News.Update(tblNews);
                    _core.Save();
                }


                //Media
                foreach (var image in dto.ImageUrls)
                {
                    try
                    {
                        var addImageResult = await AddImage(image);
                        if (!addImageResult.Success)
                            return new AdminNoneQueryResult(addImageResult.Error);
                        _core.NewsImageRel.Add(new TblNewsImageRel { ImageId = addImageResult.ImageId.Value, NewsId = newsId });
                        _core.Save();
                    }
                    catch (Exception ex)
                    {
                        return new AdminNoneQueryResult("خطا در پردازش عکس فرعی" + image.FileName);
                    }
                }
                foreach (var video in dto.VideoUrls)
                {
                    try
                    {
                        var addVideoResult = await AddVideo(video);
                        if (!addVideoResult.Success)
                            return new AdminNoneQueryResult(addVideoResult.Error);
                        _core.NewsVideoRel.Add(new TblNewsVideoRel { NewsId = newsId, VideoId = addVideoResult.VideoId.Value });
                    }
                    catch (Exception ex)
                    {
                        return new AdminNoneQueryResult("خطا در پردازش ویدئو" + video.FileName);
                    }
                }
                //KeyWords Refresh
                var perviousKeywords = _core.NewsKeywordRel.Get(_ => _.NewsId == newsId).ToList();
                foreach (var item in perviousKeywords)
                {
                    try
                    {
                        _core.NewsKeywordRel.Delete(item);
                        _core.Save();
                    }
                    catch (Exception ex)
                    {
                        return new AdminNoneQueryResult("خطا در رفرش کلمات کلیدی: " + ex.Message);
                    }
                }
                //KeyWord Add
                foreach (var item in dto.Keyword)
                {
                    TblKeyWord tblKeyWord = _core.Keyword.Get(_ => _.Body == item).FirstOrDefault();
                    _core.NewsKeywordRel.Add(new TblNewsKeyWordRel { KeyWordId = tblKeyWord.Id, NewsId = newsId });
                    _core.Save();
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

        //Images And Video
        /// <summary>
        /// Adds Non Main Images of News
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<(bool Success, string? Error, long? ImageId)> AddImage(IFormFile file)
        {
            if (file.Headers.Any(_ => _.Key == "Content-Type"))
                return (false, "هدر های فایل نامعتبر است", null);

            else if (file.Headers["Content-Type"].ToString().EndsWith("/jpg") ||
                file.Headers["Content-Type"].ToString().EndsWith("/jpeg") ||
                file.Headers["Content-Type"].ToString().EndsWith("/png") ||
                file.Headers["Content-Type"].ToString().EndsWith("/webp") ||
                file.Headers["Content-Type"].ToString().EndsWith("/gif")
                )
            {
                var url = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
                var DbVideoUrls = from image in _core.Image.Get() select image.ImageUrl.ToString();
                while (true)
                {
                    if (DbVideoUrls.Any(_ => _ == url))
                    {
                        url = Guid.NewGuid().ToString();
                    }
                    else { break; }
                }
                try
                {
                    _core.Image.Add(new TblImage { ImageUrl = url });
                    _core.Save();
                    var ImageId = _core.Video.Get().OrderBy(_ => _.Id).LastOrDefault().Id;

                    if (file.Length <= 0)
                    {
                        return (false, "فایل عکس نامعتبر است", null);
                    }
                    else
                    {
                        string filePath = ResourcePath + url;
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }
                    return (true, null, ImageId);
                }
                catch (Exception ex)
                {
                    return (false, ex.Message, null);
                }
            }
            return (false, "فرمت عکس نامعتبر است", null);
        }
        /// <summary>
        /// Adds Videos of News
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<(bool Success, string? Error, long? VideoId)> AddVideo(IFormFile file)
        {
            //TODO Add to resource
            if (file.Headers.Any(_ => _.Key == "Content-Type"))
            {
                return (false, "هدر های فایل نامعتبر است", null);
            }
            else if (!file.Headers["Content-Type"].ToString().EndsWith("/mp4"))
            {
                return (false, "فرمت فایل ویدئو فقط mp4 میتواند باشد", null);
            }

            var url = Guid.NewGuid().ToString() + "." + file.ContentType.Split('/')[1];
            var DbVideoUrls = from video in _core.Video.Get() select video.VideoUrl;
            while (true)
            {
                if (DbVideoUrls.Any(_ => _ == url))
                {
                    url = Guid.NewGuid().ToString();
                }
                else { break; }
            }
            try
            {
                _core.Video.Add(new TblVideo { VideoUrl = url });
                _core.Save();

                if (file.Length <= 0)
                {
                    return (false, "فایل عکس نامعتبر است", null);
                }
                else
                {
                    string filePath = ResourcePath + url;
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }


                var videoId = _core.Video.Get().OrderBy(_ => _.Id).LastOrDefault().Id;
                return (true, null, videoId);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }

        }

        /// <summary>
        /// Deletes Image By Id from DataBase and Resources
        /// </summary>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        private (bool Success, string? Error) DeleteImage(long ImageId)
        {
            try
            {
                TblImage tblImage = _core.Image.GetById(ImageId);
                var imageName = tblImage.ImageUrl;
                if (!Directory.EnumerateFiles(ResourcePath).Any(_ => _ == imageName))
                    return (false, "فایل عکس پیدا نشد");

                foreach (string file in Directory.EnumerateFiles(ResourcePath))
                {
                    if (file == imageName)
                    {
                        System.IO.File.Delete(ResourcePath + file);
                    }
                }
                _core.Image.Delete(tblImage);
                _core.Save();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes Video By Id from DataBase and Resources
        /// </summary>
        /// <param name="VideoId"></param>
        /// <returns></returns>
        private (bool Success, string? Error) DeleteVideo(long VideoId)
        {
            try
            {
                TblVideo tblVideo = _core.Video.GetById(VideoId);
                var videoName = tblVideo.VideoUrl;
                if (!Directory.EnumerateFiles(ResourcePath).Any(_ => _ == videoName))
                    return (false, "فایل عکس پیدا نشد");

                foreach (string file in Directory.EnumerateFiles(ResourcePath))
                {
                    if (file == videoName)
                    {
                        System.IO.File.Delete(ResourcePath + file);
                    }
                }
                _core.Video.Delete(tblVideo);
                _core.Save();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Deletes NewsImageRel with its file
        /// </summary>
        /// <param name="relId"></param>
        /// <returns></returns>
        public AdminNoneQueryResult DeleteImageRel(long relId)
        {
            try
            {
                if (_core.NewsImageRel.Any(_ => _.Id == relId))
                    return new AdminNoneQueryResult("خطا در پیدا کردن عکس فرعی");


                var imagaRel = _core.NewsImageRel.GetById(relId);
                var imageDeleteResult = DeleteImage(imagaRel.ImageId);
                if (!imageDeleteResult.Success)
                    return new AdminNoneQueryResult(imageDeleteResult.Error);
                _core.NewsImageRel.Delete(imagaRel);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }

        }
        /// <summary>
        /// Deletes NewsVideoRel with its file
        /// </summary>
        /// <param name="relId"></param>
        /// <returns></returns>
        public AdminNoneQueryResult DeleteVideoRel(long relId)
        {
            try
            {
                if (_core.NewsVideoRel.Any(_ => _.Id == relId))
                    return new AdminNoneQueryResult("خطا در پیدا کردن عکس فرعی");

                var videoRel = _core.NewsVideoRel.GetById(relId);
                var videoDeleteResult = DeleteVideo(videoRel.VideoId);
                if (!videoDeleteResult.Success)
                    return new AdminNoneQueryResult(videoDeleteResult.Error);
                _core.NewsVideoRel.Delete(videoRel);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }

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
                if (!ValidateRole(roleDto.PermissionIds, i))
                    return new AdminNoneQueryResult("لطفا برای دستسرسی ها دسترسی مادر آنها انتخاب شده باشد!");
                if (!_core.Permissions.Any(_ => _.Id == i))
                    errors.Add($"دسترسی با کد {i} پیدا نشد!");
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
                if (!ValidateRole(roleDto.PermissionIds, i))
                    return new AdminNoneQueryResult("لطفا برای دستسرسی ها دسترسی مادر آنها انتخاب شده باشد!");
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
                //Refresh Roles
                foreach (var item in _core.RolePermissionsRel.Get().Where(_ => _.RoleId == roleId).ToList())
                {
                    _core.RolePermissionsRel.Delete(item);
                    _core.Save();
                }
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

        /// <summary>
        /// Validates Roles Permission Sent By
        /// </summary>
        /// <param name="allPermissionIds">All PermissionIds Sent</param>
        /// <param name="permissionId">PermissionId we need Validate</param>
        /// <returns></returns>
        private bool ValidateRole(List<int> allPermissionIds, int permissionId)
        {
            TblRolePermissions permission = _core.Permissions.GetById(permissionId);
            if (permission.ParentId == null)
            {
                foreach (var i in permission.InverseParent)
                {
                    if (!allPermissionIds.Any(_ => _ == i.Id))
                        return false;
                }
                return true;
            }
            return allPermissionIds.Any(_ => _ == permission.ParentId);
        }

        //---Advertisement
        public async Task<AdminNoneQueryResult> AddAdvertisementAsync(AdminAdvertisementCreateUpdateDto dto)
        {
            var modelValidateResult = Extentions.ValidateModel(dto);
            if (!modelValidateResult.IsValid)
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);
            if (dto.MainBaner != null || dto.PerviuosMainBaner.Trim().IsNullOrEmpty())
                return new AdminNoneQueryResult("عکس اصلی خالی است");
            try
            {
                var ImageAddResult = await AddImage(dto.MainBaner);
                if (!ImageAddResult.Success)
                {
                    return new AdminNoneQueryResult(ImageAddResult.Error);
                }

                TblAdvertisement tblAdvertisement = new TblAdvertisement() { MainBanerId = ImageAddResult.ImageId.Value, Link = dto.Link };
                _core.Advertisement.Add(tblAdvertisement);
                _core.Save();
                return new AdminNoneQueryResult();
            }
            catch (Exception ex)
            {
                return new AdminNoneQueryResult(ex.Message);
            }

        }

        public async Task<AdminNoneQueryResult> UpdateAdvertisementAsync(AdminAdvertisementCreateUpdateDto dto, int Id)
        {
            var modelValidateResult = Extentions.ValidateModel(dto);
            if (!modelValidateResult.IsValid)
                return new AdminNoneQueryResult(modelValidateResult.ErrorMessages);

            if (dto.MainBaner != null && dto.PerviuosMainBaner.Trim().IsNullOrEmpty())
                return new AdminNoneQueryResult("عکس اصلی خالی است");
            if (_core.Advertisement.Any(_ => _.Id == Id))
                return new AdminNoneQueryResult("تبلیغ مورد نظر یافت نشد");
            try
            {
                TblAdvertisement tblAdvertisement = _core.Advertisement.Get(_ => _.Id == Id, includes: "MainBaner").FirstOrDefault();
                if (dto.PerviuosMainBaner.Split("/")[dto.PerviuosMainBaner.Split("/").Length - 1] !=
                    tblAdvertisement.MainBaner.ImageUrl.ToString())
                {
                    if (dto.MainBaner == null)
                        return new AdminNoneQueryResult("عکس اصلی خالی است");
                    var ImageAddResult = await AddImage(dto.MainBaner);
                    if (!ImageAddResult.Success)
                    {
                        return new AdminNoneQueryResult(ImageAddResult.Error);
                    }
                    tblAdvertisement.Link = dto.Link;
                    tblAdvertisement.MainBanerId = ImageAddResult.ImageId.Value;
                }

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
