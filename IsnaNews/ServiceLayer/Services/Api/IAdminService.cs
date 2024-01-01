using DataLayer.Dtos.Admin;
using DataLayer.Dtos.Admin.Advertisement;
using DataLayer.Dtos.Admin.Base;
using DataLayer.Dtos.Admin.Comment;
using DataLayer.Dtos.Admin.Config;
using DataLayer.Dtos.Admin.Keyword;
using DataLayer.Dtos.Admin.News;
using DataLayer.Dtos.Admin.User;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Api
{
    internal interface IAdminService
    {
        AdminReadAllQueryResult<AdminUserDto> GetUsersList();
        AdminReadByIdQueryResult<AdminUserDto> GetUserById(int id);
        AdminNoneQueryResult AddUser(AdminUserCreateUpdateDto user);
        AdminNoneQueryResult UpdateUser(AdminUserCreateUpdateDto user, int userId);
        AdminNoneQueryResult DeleteUser(int userId);
        AdminReadByIdQueryResult<AdminUsersPermissionsDto> GetUsersPermissions(int userId);
        AdminReadAllQueryResult<AdminNewsDto> GetNewsList();
        AdminReadByIdQueryResult<AdminNewsDto> GetNewsById(long id);
        Task<AdminNoneQueryResult> AddNewsAsync(AdminNewsCreateUpdateDto dto);
        AdminNoneQueryResult UpdateNews(AdminNewsCreateUpdateDto dto, long id);
        AdminNoneQueryResult DeleteNews(long id);
        AdminNoneQueryResult DeleteComment(long id);
        AdminNoneQueryResult ReplyComment(AdminCommentReplyDto replyDto, int SupporterId);
        AdminNoneQueryResult AddRole(AdminRoleCreateUpdateDto roleDto);
        AdminNoneQueryResult UpdateRole(AdminRoleCreateUpdateDto roleDto, int roleId);
        AdminNoneQueryResult DeleteRole(int roleId);
        AdminReadByIdQueryResult<AdminRoleDto> GetRoleById(int roleId);
        AdminReadAllQueryResult<AdminRoleDto> GetRoleList();
        AdminNoneQueryResult AddAdvertisement(AdminAdvertisementCreateUpdateDto dto);
        AdminNoneQueryResult UpdateAdvertisement(AdminAdvertisementCreateUpdateDto dto, int Id);
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












    }
}
