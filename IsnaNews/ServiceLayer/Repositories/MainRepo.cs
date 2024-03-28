using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Services.Repositories
{
    public class MainRepo<T> : IMainRepo<T> where T : class
    {
        private readonly IsnaNewsContext _context;
        private readonly DbSet<T> _dbSet;

        public MainRepo(IsnaNewsContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual EntityEntry Add(T entity)
        {
            return _dbSet.Add(entity);
        }

        public virtual async Task<EntityEntry> AddAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        public virtual bool Update(T entity)
        {
            dynamic model = entity;
            _dbSet.Local.FindEntry(model.Id)
            .State = EntityState.Detached;
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return true;
        }

        public virtual bool Delete(T entity)
        {
            try
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                    _dbSet.Attach(entity);
                _dbSet.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool DeleteById(object id)
        {
            try
            {
                T entity = _dbSet.Find(id);
                return entity != null && Delete(entity);
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> DeleteByIdAsync(object id)
        {
            try
            {
                T entity = await _dbSet.FindAsync(id);
                return entity != null && Delete(entity);
            }
            catch
            {
                return false;
            }
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includes = null)
        {
            IQueryable<T> query = _dbSet;
            if (where != null)
                query = query.Where(where);
            if (orderBy != null)
                query = orderBy(query);
            if (includes != null)
                foreach (string i in includes.Split(','))
                    query = query.Include(i);


            return query.ToList();
        }

        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includes = null)
        {
            IQueryable<T> query = _dbSet;
            if (where != null)
                query = query.Where(where);
            if (orderBy != null)
                query = orderBy(query);
            if (includes != null)
                foreach (string i in includes.Split(','))
                    query = query.Include(i);
            return await query.ToListAsync();
        }

        public virtual bool Any(Expression<Func<T, bool>> where = null)
        {
            if (where != null)
                return _dbSet.Any(where);
            return false;
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> where = null)
        {
            if (where != null)
                return await _dbSet.AnyAsync(where);
            return false;
        }

        public virtual T GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> where = null)
        {
            if (where != null)
                return _dbSet.FirstOrDefault(where);
            return null;
        }

        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where = null)
        {
            if (where != null)
                return await _dbSet.FirstOrDefaultAsync(where);
            return null;
        }
    }
}
