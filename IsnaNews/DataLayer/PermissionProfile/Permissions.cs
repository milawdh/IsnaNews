using DataLayer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Types
{
    public static class AdminPermissions
    {
        public const string GroupName = "Admin";
        public static class NewsPermission
        {
            public const string Default = GroupName + ".NewsPermission";
            public const string Add = Default + ".Add";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
        public static class CategoryPermission
        {
            public const string Default = GroupName + ".CategoryPermission";
            public const string Add = Default + ".Add";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
        public static class AdvertisementPermission
        {
            public const string Default = GroupName + ".AdvertisementPermission";
            public const string Add = Default + ".Add";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
        public static class ConfigPermission
        {
            public const string Default = GroupName + ".ConfigPermission";
            public const string Add = Default + ".Add";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
        public static class CommentPermission
        {
            public const string Default = GroupName + ".CommentPermission";
            public const string Reply = Default + ".Reply";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
        public static class KeywordPermission
        {
            public const string Default = GroupName + ".KeywordPermission";
            public const string Add = Default + ".Add";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
        public static class UsersPermission
        {
            public const string Default = GroupName + ".UsersPermission";
            public const string Add = Default + ".Add";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
        public static class RolesPermission
        {
            public const string Default = GroupName + ".RolesPermission";
            public const string Add = Default + ".Add";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
    }
}
