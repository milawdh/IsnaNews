using System;
using System.Collections.Generic;
using System.Text;
using DataLayer.Models;
using Services.Repositories;

namespace Services.Services
{
    public class Core : IDisposable
    {
        private readonly IsnaNewsContext _context = new IsnaNewsContext();

        private MainRepo<TblAdvertisement> _advertisement;
        private MainRepo<TblCategory> _category;
        private MainRepo<TblImage> _image;
        private MainRepo<TblKeyWord> _keyword;
        private MainRepo<TblNews> _news;
        private MainRepo<TblNewsComment> _comment;
        private MainRepo<TblNewsImageRel> _newsImageRel;
        private MainRepo<TblNewsKeyWordRel> _newsKeywordRel;
        private MainRepo<TblNewsVideoRel> _newsVideoRel;
        private MainRepo<TblRole> _role;
        private MainRepo<TblRolePermissions> _rolePermissions;
        private MainRepo<TblRoleRolePermissionsRel> _rolePermissionsRel;
        private MainRepo<TblUser> _user;
        private MainRepo<TblVideo> _video;
        private MainRepo<TblAboutUs> _aboutUs;
        private MainRepo<TblContactUs> _contactUs;
        public MainRepo<TblAdvertisement> Advertisement => _advertisement ?? new MainRepo<TblAdvertisement>(_context);
        public MainRepo<TblCategory> Category => _category ??= new MainRepo<TblCategory>(_context);
        public MainRepo<TblImage> Image => _image ??= new MainRepo<TblImage>(_context);
        public MainRepo<TblNewsImageRel> NewsImageRel => _newsImageRel ??= new MainRepo<TblNewsImageRel>(_context);
        public MainRepo<TblVideo> Video => _video ??= new MainRepo<TblVideo>(_context);
        public MainRepo<TblRole> Role => _role ??= new MainRepo<TblRole>(_context);
        public MainRepo<TblNews> News => _news ??= new MainRepo<TblNews>(_context);
        public MainRepo<TblRolePermissions> Permissions => _rolePermissions ??= new MainRepo<TblRolePermissions>(_context);
        public MainRepo<TblRoleRolePermissionsRel> RolePermissionsRel => _rolePermissionsRel ??= new MainRepo<TblRoleRolePermissionsRel>(_context);
        public MainRepo<TblNewsComment> Comment => _comment ??= new MainRepo<TblNewsComment>(_context);
        public MainRepo<TblNewsVideoRel> NewsVideoRel => _newsVideoRel ??= new MainRepo<TblNewsVideoRel>(_context);
        public MainRepo<TblKeyWord> Keyword => _keyword ??= new MainRepo<TblKeyWord>(_context);
        public MainRepo<TblNewsKeyWordRel> NewsKeywordRel => _newsKeywordRel ??= new MainRepo<TblNewsKeyWordRel>(_context);
        public MainRepo<TblUser> User => _user ??= new MainRepo<TblUser>(_context);
        public MainRepo<TblAboutUs> AboutUs => _aboutUs ??= new MainRepo<TblAboutUs>(_context);
        public MainRepo<TblContactUs> ContactUs => _contactUs ??= new MainRepo<TblContactUs>(_context);
        public void Save() => _context.SaveChanges();
        public void Dispose() => _context.Dispose();

    }
}
