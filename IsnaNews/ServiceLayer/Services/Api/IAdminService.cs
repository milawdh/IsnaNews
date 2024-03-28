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
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Api
{
    public interface IAdminService
    {

        AdminReadAllQueryResult<AdminUserDto> GetUsersList();
        AdminReadByIdQueryResult<AdminUserDto> GetUserById(int id);
        Task<AdminNoneQueryResult> AddUserAsync(AdminUserCreateUpdateDto userDto);
        Task<AdminNoneQueryResult> UpdateUserAsync(AdminUserCreateUpdateDto userDto, int userId);
        AdminNoneQueryResult DeleteUser(int userId);
        AdminReadByIdQueryResult<AdminUsersPermissionsDto> GetUsersPermissions(int userId);
        AdminReadAllQueryResult<AdminNewsDto> GetNewsList();
        AdminReadByIdQueryResult<AdminNewsDto> GetNewsById(long id);
        Task<AdminNoneQueryResult> AddNewsAsync(AdminNewsCreateUpdateDto dto);
        Task<AdminNoneQueryResult> UpdateNewsAsync(AdminNewsCreateUpdateDto dto, long newsId);
        AdminNoneQueryResult DeleteNews(long id);
        AdminNoneQueryResult DeleteComment(long id);
        AdminNoneQueryResult ReplyComment(AdminCommentReplyDto replyDto, int SupporterId);
        AdminNoneQueryResult AddRole(AdminRoleCreateUpdateDto roleDto);
        AdminNoneQueryResult UpdateRole(AdminRoleCreateUpdateDto roleDto, int roleId);
        AdminNoneQueryResult DeleteRole(int roleId);
        AdminReadByIdQueryResult<AdminRoleDto> GetRoleById(int roleId);
        AdminReadAllQueryResult<AdminRoleDto> GetRoleList();
        Task<AdminNoneQueryResult> AddAdvertisementAsync(AdminAdvertisementCreateUpdateDto dto);
        Task<AdminNoneQueryResult> UpdateAdvertisementAsync(AdminAdvertisementCreateUpdateDto dto, int Id);
        AdminNoneQueryResult DeleteAdvertisement(int Id);
        AdminReadByIdQueryResult<AdminAdvertisementDto> GetAdvertisementById(int Id);
        AdminReadAllQueryResult<AdminAdvertisementDto> GetAdminAdvertisementList();
        AdminNoneQueryResult AddAboutUs(string body);
        AdminNoneQueryResult UpdateAboutUs(string body, int Id);
        AdminNoneQueryResult DeleteAboutUs(int Id);
        AdminReadByIdQueryResult<AdminAboutUsDto> GetAboutUsById(int Id);
        AdminReadAllQueryResult<AdminAboutUsDto> GetAboutUsList();
        AdminNoneQueryResult AddContactUs(string body);
        AdminNoneQueryResult UpdateContactUs(string body, int Id);
        AdminNoneQueryResult DeleteContactUs(int Id);
        AdminReadByIdQueryResult<AdminContactUsDto> GetContactUsById(int Id);
        AdminReadAllQueryResult<AdminContactUsDto> GetContactUsList();
        AdminNoneQueryResult AddKeyword(AdminKeyWordCreateUpdateDto keyword);
        AdminNoneQueryResult UpdateKeyword(AdminKeyWordCreateUpdateDto keyword, int Id);
        AdminNoneQueryResult DeleteKeyword(int Id);
        AdminReadByIdQueryResult<AdminKeywordDto> GetKeywordById(int Id);
        AdminReadAllQueryResult<AdminKeywordDto> GetKewordList();
        Task<(bool Success, string? Error, long? VideoId)> AddVideo(IFormFile file);
        Task<(bool Success, string? Error, long? ImageId)> AddImage(IFormFile file);
        AdminNoneQueryResult DeleteImageRel(long relId);
        AdminNoneQueryResult DeleteVideoRel(long relId);
        AdminReadAllQueryResult<AdminCategoryDto> GetCategoryList();
        AdminReadByIdQueryResult<AdminCategoryDto> GetCategoryById(int id);
        AdminNoneQueryResult AddCategory(AdminCategoryCreateUpdateDto dto);
        AdminNoneQueryResult UpdateCategory(AdminCategoryCreateUpdateDto dto, int id);
        AdminNoneQueryResult DeleteCategory(int id);



    }
}
