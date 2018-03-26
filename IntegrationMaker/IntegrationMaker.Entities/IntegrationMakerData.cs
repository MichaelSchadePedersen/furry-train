using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using static IntegrationMaker.Entities.Data;

namespace IntegrationMaker.Entities
{
    public enum IdNamingConventions
    {
        Id,
        ClassNamePlusId
    }

    public enum JobTypes
    {
        PartialIntegrationImport = 10,
        FullIntegrationImport = 20,
    }

    public enum JobStates
    {
        Created = 10,
        InProcess = 20,
        Failed = 30,
        Completed = 40,
        Deleted = 50,
    }

    public enum ErpIntegrationErrorCodes
    {
        InvalidCredentials = 1,
        InvalidToken = 2,
        UserNotFound = 3,
        UserDeletedOrNotFound = 4,
        AccountDeletedOrNotFound = 5,
        NoApiAccess = 6,
        DemoAccountIsExpired = 7,
        NoAdministratorRightToCompany = 8,
        AuthenticationUnspecified = 9,
        CouldNotDetermineCompany = 10,
        VersionIncompatibility = 11,
        InsufficientPrivileges = 12,
        ServiceTemporarilyUnavailable = 13,
        TheGivenTokenHasBeenRevoked = 14,
        WrongServiceURL = 15,
        Unknown = 99
    }
    #region Generic class
    /// <summary>
    /// A helper class that allows you to work with Entity Framework entities and data without having to work directly with the DbContext class and its peculiarities.
    /// The exposed methods abstract away the DbContext and provides CRUD functionality in a more intuitive manner.
    /// To use, call the Configure method once with the type of your DbContext class and optionally some other configuration. Then just call the exposed CRUD methods.
    /// </summary>
    public static class Data<T> where T : class
    {
        /// <summary>
        /// Set this to false to indicate that your identity fields are always of type long (aka. Int64 or BIGINT). This reduces some overhead in determining the type of the field.
        /// </summary>
        public static bool SupportNonLongKeys = false;

        //private static List<T> cache;

        public static void ConfigureCaching(bool enableCaching, bool preLoad = false)
        {
            throw new NotImplementedException();

            /*EnableCaching = enableCaching;

			if (!enableCaching)
				cache.Clear();
			else if (preLoad)
				cache = Get();*/
        }

        #region Save
        public static T Save(T entity)
        {
            if (entity != null)
                try
                {
                    using (var db = GetDbContext())
                    {
                        var table = db.Set<T>();
                        long id = GetId(entity);
                        db.Entry(entity).State = id > 0 ? System.Data.Entity.EntityState.Modified : System.Data.Entity.EntityState.Added;
                        db.SaveChanges();
                    }
                }
                catch (DbEntityValidationException e)
                {
                    RethrowValidationException(e);
                }
                catch (Exception ex)
                {
                    throw;
                }

            return entity;
        }

        public static async Task<T> SaveAsync(T entity)
        {
            if (entity != null)
                try
                {
                    using (var db = GetDbContext())
                    {
                        var table = db.Set<T>();
                        long id = GetId(entity);
                        db.Entry(entity).State = id > 0 ? System.Data.Entity.EntityState.Modified : System.Data.Entity.EntityState.Added;
                        await db.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
                catch (DbEntityValidationException e)
                {
                    RethrowValidationException(e);
                }
                catch (DbUpdateException e)
                {
                    throw;
                }

            return entity;
        }

        public static T Save<M>(T entity, Expression<Func<T, ICollection<M>>> manyToManyProperty) where M : class
        {
            if (entity != null)
                try
                {
                    //Set the navigation properties to null, save the main entity, then repopulate the nav.props and handle them in a separate context.
                    var mappingList = new List<M>();

                    using (var db = GetDbContext())
                    {
                        mappingList = db.Entry(entity).Collection(manyToManyProperty).CurrentValue.ToList();
                        db.Entry(entity).Collection(manyToManyProperty).CurrentValue = null;

                        var table = db.Set<T>();
                        long id = GetId(entity);
                        db.Entry(entity).State = id > 0 ? System.Data.Entity.EntityState.Modified : System.Data.Entity.EntityState.Added;
                        db.SaveChanges();

                        db.Entry(entity).Collection(manyToManyProperty).CurrentValue = mappingList;
                    }

                    HandlemanyToManyPropertyAsync<M>(entity, manyToManyProperty).Wait();
                }
                catch (DbEntityValidationException e)
                {
                    RethrowValidationException(e);
                }

            return entity;
        }

        public static async Task<T> SaveAsync<M>(T entity, Expression<Func<T, ICollection<M>>> manyToManyProperty) where M : class
        {
            if (entity != null)
                try
                {
                    var mappingList = new List<M>();

                    using (var db = GetDbContext())
                    {
                        mappingList = db.Entry(entity).Collection(manyToManyProperty).CurrentValue.ToList();
                        db.Entry(entity).Collection(manyToManyProperty).CurrentValue = null;

                        var table = db.Set<T>();
                        long id = GetId(entity);
                        db.Entry(entity).State = id > 0 ? System.Data.Entity.EntityState.Modified : System.Data.Entity.EntityState.Added;

                        await db.SaveChangesAsync().ConfigureAwait(false);

                        db.Entry(entity).Collection(manyToManyProperty).CurrentValue = mappingList;
                    }

                    await HandlemanyToManyPropertyAsync<M>(entity, manyToManyProperty).ConfigureAwait(false);
                }
                catch (DbEntityValidationException e)
                {
                    RethrowValidationException(e);
                }

            return entity;
        }

        public static List<T> Save(IEnumerable<T> entities)
        {
            if (entities == null)
                return new List<T>();

            var list = entities as List<T> ?? entities.ToList();

            if (!list.Any())
                return list;

            try
            {
                using (var db = GetDbContext())
                {
                    SetEntityState(list, db);
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }

            return entities as List<T>;
        }

        public static async Task<List<T>> SaveAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return new List<T>();

            var list = entities as List<T> ?? entities.ToList();

            if (!list.Any())
                return list;

            try
            {
                using (var db = GetDbContext())
                {
                    SetEntityState(list, db);
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }

            return list;
        }

        //Inspired by http://stackoverflow.com/questions/5940225/fastest-way-of-inserting-in-entity-framework
        public static List<T> BulkSave(IEnumerable<T> entities)
        {
            if (entities == null)
                return new List<T>();

            var list = entities as List<T> ?? entities.ToList();

            if (!list.Any())
                return list;

            int bulkCount = 1000, counter = 0, rounds = 0;
            DbContext db = null;

            try
            {
                foreach (var entity in list)
                {
                    if ((counter == 0 && rounds == 0) || counter == bulkCount)
                    {
                        if (db != null)
                        {
                            db.SaveChanges();
                            db.Dispose();
                        }

                        db = GetDbContext();
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        counter = 0;
                        rounds++;
                    }

                    long id = GetId(entity);
                    db.Entry(entity).State = id > 0 ? System.Data.Entity.EntityState.Modified : System.Data.Entity.EntityState.Added;

                    //db.Set<T>().Add(item);
                    counter++;
                }
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }

            return list as List<T>;
        }

        //Inspired by http://stackoverflow.com/questions/5940225/fastest-way-of-inserting-in-entity-framework
        public static async Task<List<T>> BulkSaveAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return new List<T>();

            var list = entities as List<T> ?? entities.ToList();

            if (!list.Any())
                return list;

            Int32 bulkCount = 1000, counter = 0, rounds = 0;
            DbContext db = null;

            try
            {
                foreach (var item in list)
                {
                    if ((counter == 0 && rounds == 0) || counter == bulkCount)
                    {
                        if (db != null)
                        {
                            await db.SaveChangesAsync().ConfigureAwait(false);
                            db.Dispose();
                        }

                        db = GetDbContext();
                        db.Configuration.AutoDetectChangesEnabled = false;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        counter = 0;
                        rounds++;
                    }

                    var id = (Int64)item.GetType().GetProperty("Id").GetValue(item, null);
                    db.Entry(item).State = id > 0 ? System.Data.Entity.EntityState.Modified : System.Data.Entity.EntityState.Added;

                    //db.Set<T>().Add(item);
                    counter++;
                }
                await db.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }

            return entities as List<T>;
        }
        #endregion

        #region Delete
        public static void Delete(T entity)
        {
            try
            {
                using (var db = GetDbContext())
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static async Task DeleteAsync(T entity)
        {
            try
            {
                using (var db = GetDbContext())
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static void Delete(long id, bool throwIfNotFound = false)
        {
            var predicate = GetIdPredicate(id);

            try
            {
                using (var db = GetDbContext())
                {
                    var entity = db.Set<T>().SingleOrDefault(predicate);

                    if (entity == null)
                    {
                        if (throwIfNotFound)
                            throw new ArgumentException("Entity of type " + typeof(T).Name + " with Id " + id.ToString() + " was not found.");
                        else
                            return;
                    }

                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static async Task DeleteAsync(long id, bool throwIfNotFound = false)
        {
            var predicate = GetIdPredicate(id);

            try
            {
                using (var db = GetDbContext())
                {
                    var entity = db.Set<T>().SingleOrDefault(predicate);

                    if (entity == null)
                    {
                        if (throwIfNotFound)
                            throw new ArgumentException("Entity of type " + typeof(T).Name + " with Id " + id.ToString() + " was not found.");
                        else
                            return;
                    }


                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static void Delete<M>(T entity, Expression<Func<T, ICollection<M>>> manyToManyProperty) where M : class
        {
            try
            {
                HandlemanyToManyPropertyAsync<M>(entity, manyToManyProperty, true).Wait();

                using (var db = GetDbContext())
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static async Task DeleteAsync<M>(T entity, Expression<Func<T, ICollection<M>>> manyToManyProperty) where M : class
        {
            try
            {
                await HandlemanyToManyPropertyAsync<M>(entity, manyToManyProperty, true).ConfigureAwait(false);

                using (var db = GetDbContext())
                {
                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static void Delete<M>(long id, Expression<Func<T, ICollection<M>>> manyToManyProperty) where M : class
        {
            Delete<M>(id, false, manyToManyProperty);
        }

        public static void Delete<M>(long id, bool throwIfNotFound = false, Expression<Func<T, ICollection<M>>> manyToManyProperty = null) where M : class
        {
            var predicate = GetIdPredicate(id);

            try
            {
                HandlemanyToManyPropertyAsync<M>(id, manyToManyProperty, true).Wait();

                using (var db = GetDbContext())
                {
                    var entity = db.Set<T>().SingleOrDefault(predicate);

                    if (entity == null)
                    {
                        if (throwIfNotFound)
                            throw new ArgumentException("Entity of type " + typeof(T).Name + " with Id " + id.ToString() + " was not found.");
                        else
                            return;
                    }

                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static async Task DeleteAsync<M>(long id, Expression<Func<T, ICollection<M>>> manyToManyProperty) where M : class
        {
            await DeleteAsync<M>(id, false, manyToManyProperty).ConfigureAwait(false);
        }

        public static async Task DeleteAsync<M>(long id, bool throwIfNotFound, Expression<Func<T, ICollection<M>>> manyToManyProperty) where M : class
        {
            var predicate = GetIdPredicate(id);

            try
            {
                await HandlemanyToManyPropertyAsync<M>(id, manyToManyProperty, true).ConfigureAwait(false);

                using (var db = GetDbContext())
                {
                    var entity = db.Set<T>().SingleOrDefault(predicate);

                    if (entity == null)
                    {
                        if (throwIfNotFound)
                            throw new ArgumentException("Entity of type " + typeof(T).Name + " with Id " + id.ToString() + " was not found.");
                        else
                            return;
                    }

                    db.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static void Delete(IEnumerable<T> entities)
        {
            if (entities == null)
                return;

            var list = entities as List<T> ?? entities.ToList();

            if (!list.Any())
                return;

            try
            {
                using (var db = GetDbContext())
                {
                    foreach (var entity in list)
                        db.Entry(entities).State = EntityState.Deleted;

                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static async Task DeleteAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                return;

            var list = entities as List<T> ?? entities.ToList();

            if (!list.Any())
                return;

            try
            {
                using (var db = GetDbContext())
                {
                    foreach (var entity in list)
                        db.Entry(entity).State = EntityState.Deleted;

                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static void Delete(Expression<Func<T, bool>> where = null)
        {
            try
            {
                using (var db = GetDbContext())
                {
                    var set = db.Set<T>();
                    set.RemoveRange(set.Where(where));
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }

        public static async Task DeleteAsync(Expression<Func<T, bool>> where = null)
        {
            try
            {
                using (var db = GetDbContext())
                {
                    var set = db.Set<T>();
                    set.RemoveRange(set.Where(where));
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (DbEntityValidationException e)
            {
                RethrowValidationException(e);
            }
        }
        #endregion

        #region Get single
        public static T Get(long id, params Expression<Func<T, object>>[] include)
        {
            var predicate = GetIdPredicate(id);

            using (var db = GetDbContext())
            {
                IQueryable<T> query = db.Set<T>().AsNoTracking();

                foreach (var includeProperty in include)
                    query = query.Include(includeProperty);

                return query.SingleOrDefault(predicate);
            }
        }

        public static async Task<T> GetAsync(long id, params Expression<Func<T, object>>[] include)
        {
            var predicate = GetIdPredicate(id);

            using (var db = GetDbContext())
            {
                IQueryable<T> query = db.Set<T>().AsNoTracking();

                foreach (var includeProperty in include)
                    query = query.Include(includeProperty);

                return await query.SingleOrDefaultAsync(predicate).ConfigureAwait(false);
            }
        }

        public static T GetByKey(params object[] keyValues)
        {
            using (var db = GetDbContext())
            {
                return db.Set<T>().Find(keyValues);
            }
        }

        public static async Task<T> GetByKeyAsync(params object[] keyValues)
        {
            using (var db = GetDbContext())
            {
                return await db.Set<T>().FindAsync(keyValues).ConfigureAwait(false);
            }
        }
        #endregion

        #region Get multiple
        public static List<T> GetSorted<TOrderBy>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrderBy>> orderBy,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return GetSorted(where, orderBy, false, 0, include);
        }

        public static List<T> GetSorted<TOrderBy>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrderBy>> orderBy, bool orderDescending,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return GetSorted(where, orderBy, orderDescending, 0, include);
        }

        public static List<T> GetSorted<TOrderBy>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrderBy>> orderBy, bool orderDescending = false,
            int take = 0,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(db, where, orderBy, orderDescending, take, include);
                return query.ToList();
            }
        }

        public static async Task<List<T>> GetSortedAsync<TOrderBy>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrderBy>> orderBy,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return await GetSortedAsync(where, orderBy, false, 0, include).ConfigureAwait(false);
        }

        public static async Task<List<T>> GetSortedAsync<TOrderBy>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrderBy>> orderBy, bool orderDescending,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return await GetSortedAsync(where, orderBy, orderDescending, 0, include).ConfigureAwait(false);
        }

        public static async Task<List<T>> GetSortedAsync<TOrderBy>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrderBy>> orderBy, bool orderDescending = false,
             int take = 0, params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(db, where, orderBy, orderDescending, take, include);
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public static List<T> GetSorted<TOrderBy>(IEnumerable<long> ids,
            Expression<Func<T, TOrderBy>> orderBy,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return GetSorted(GetIdPredicate(ids), orderBy, false, 0, include);
        }

        public static List<T> GetSorted<TOrderBy>(IEnumerable<long> ids,
            Expression<Func<T, TOrderBy>> orderBy, bool orderDescending,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return GetSorted(GetIdPredicate(ids), orderBy, orderDescending, 0, include);
        }

        public static async Task<List<T>> GetSortedAsync<TOrderBy>(IEnumerable<long> ids,
            Expression<Func<T, TOrderBy>> orderBy,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return await GetSortedAsync(GetIdPredicate(ids), orderBy, false, 0, include).ConfigureAwait(false);
        }

        public static async Task<List<T>> GetSortedAsync<TOrderBy>(IEnumerable<long> ids,
            Expression<Func<T, TOrderBy>> orderBy, bool orderDescending,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return await GetSortedAsync(GetIdPredicate(ids), orderBy, orderDescending, 0, include).ConfigureAwait(false);
        }

        public static List<T> GetAllSorted<TOrderBy>(
            Expression<Func<T, TOrderBy>> orderBy, bool orderDescending = false,
            int take = 0,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(db, null, orderBy, orderDescending, take, include);
                return query.ToList();
            }
        }

        public static async Task<List<T>> GetAllSortedAsync<TOrderBy>(
            Expression<Func<T, TOrderBy>> orderBy, bool orderDescending = false,
             int take = 0, params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(db, null, orderBy, orderDescending, take, include);
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public static List<T> GetAllSorted<TOrderBy>(
            Expression<Func<T, TOrderBy>> orderBy,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(db, null, orderBy, false, 0, include);
                return query.ToList();
            }
        }

        public static async Task<List<T>> GetAllSortedAsync<TOrderBy>(
            Expression<Func<T, TOrderBy>> orderBy,
             params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(db, null, orderBy, false, 0, include);
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public static List<T> GetAllSorted<TOrderBy>(
            Expression<Func<T, TOrderBy>> orderBy = null, bool orderDescending = false,
            params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(db, null, orderBy, orderDescending, 0, include);
                return query.ToList();
            }
        }

        public static async Task<List<T>> GetAllSortedAsync<TOrderBy>(
            Expression<Func<T, TOrderBy>> orderBy = null, bool orderDescending = false,
             params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(db, null, orderBy, orderDescending, 0, include);
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public static List<T> GetAll(params Expression<Func<T, object>>[] include)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<int>(db, null, null, false, 0, include); //<int> is used as dummy.
                return query.ToList();
            }
        }

        public static async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] include)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<int>(db, null, null, false, 0, include); //<int> is used as dummy.
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public static List<T> Get(params Expression<Func<T, object>>[] include)
        {
            return GetSorted<int>(include: include, where: null, orderBy: null);
        }

        public static async Task<List<T>> GetAsync(params Expression<Func<T, object>>[] include)
        {
            //Note: This could be held shorter by just calling an overload to GetAsync, but that would mean extra overhead because of nested awaits. We do it for the sync methods however.			
            using (var db = GetDbContext())
            {
                var query = GetQuery<int>(include: include, db: db);
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public static List<T> Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] include)
        {
            return GetSorted<int>(where: where, include: include, orderBy: null);
        }

        public static async Task<List<T>> GetAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] include)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<int>(where: where, include: include, db: db);
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public static List<T> Get(IEnumerable<long> ids, params Expression<Func<T, object>>[] include)
        {
            return GetSorted<int>(where: GetIdPredicate(ids), include: include, orderBy: null);
        }

        public static async Task<List<T>> GetAsync(IEnumerable<long> ids, params Expression<Func<T, object>>[] include)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<int>(where: GetIdPredicate(ids), include: include, db: db);
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }
        #endregion

        #region FirstOrDefault
        public static T FirstOrDefault(params Expression<Func<T, object>>[] include)
        {
            return GetSorted<int>(take: 1, include: include, where: null, orderBy: null).SingleOrDefault();
        }

        public static async Task<T> FirstOrDefaultAsync(params Expression<Func<T, object>>[] include)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<int>(take: 1, include: include, db: db);
                return await query.SingleOrDefaultAsync().ConfigureAwait(false);
            }
        }

        public static T FirstOrDefault(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] include)
        {
            return GetSorted<int>(where: where, orderBy: null, take: 1, include: include).SingleOrDefault();
        }

        public static async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] include)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<int>(where: where, take: 1, include: include, db: db);
                return await query.SingleOrDefaultAsync().ConfigureAwait(false);
            }
        }

        public static T FirstOrDefaultSorted<TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderBy, bool orderDescending = false, params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return GetSorted<TOrderBy>(where, orderBy, orderDescending, 1, include).SingleOrDefault();
        }

        public static async Task<T> FirstOrDefaultSortedAsync<TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderBy, bool orderDescending = false, params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(where: where, take: 1,
                    orderBy: orderBy, orderDescending: orderDescending,
                    include: include, db: db);
                return await query.SingleOrDefaultAsync().ConfigureAwait(false);
            }
        }

        public static T FirstOrDefaultSorted<TOrderBy>(Expression<Func<T, TOrderBy>> orderBy, bool orderDescending = false, params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            return GetSorted<TOrderBy>(null, orderBy, orderDescending, 1, include).SingleOrDefault();
        }

        public static async Task<T> FirstOrDefaultSortedAsync<TOrderBy>(Expression<Func<T, TOrderBy>> orderBy, bool orderDescending = false, params Expression<Func<T, object>>[] include) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TOrderBy>(where: null, take: 1,
                    orderBy: orderBy, orderDescending: orderDescending,
                    include: include, db: db);
                return await query.SingleOrDefaultAsync().ConfigureAwait(false);
            }
        }
        #endregion

        #region Projection
        public static List<TSelect> Select<TSelect>(Expression<Func<T, TSelect>> select, Expression<Func<T, bool>> where = null, int take = 0)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TSelect, int>(db: db, select: select, where: where, take: take);
                return query.ToList();
            }
        }

        public static async Task<List<TSelect>> SelectAsync<TSelect>(Expression<Func<T, TSelect>> select, Expression<Func<T, bool>> where = null, int take = 0)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TSelect, int>(db: db, select: select, where: where, take: take);
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public static TSelect SelectFirstOrDefault<TSelect>(Expression<Func<T, TSelect>> select, Expression<Func<T, bool>> where = null, int take = 0)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TSelect, int>(db: db, select: select, where: where, take: 1);
                return query.ToList().FirstOrDefault();
            }
        }

        public static async Task<TSelect> SelectFirstOrDefaultAsync<TSelect>(Expression<Func<T, TSelect>> select, Expression<Func<T, bool>> where = null, int take = 0)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TSelect, int>(db: db, select: select, where: where, take: 1);
                return (await query.ToListAsync().ConfigureAwait(false)).FirstOrDefault();
            }
        }

        public static List<TSelect> SelectSorted<TSelect, TOrderBy>(Expression<Func<T, TSelect>> select,
            Expression<Func<T, bool>> where = null,
            Expression<Func<T, TOrderBy>> orderBy = null, bool orderDescending = false,
            int take = 0) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TSelect, TOrderBy>(db, select, where, orderBy, orderDescending, take);
                return query.ToList();
            }
        }

        public static async Task<List<TSelect>> SelectSortedAsync<TSelect, TOrderBy>(Expression<Func<T, TSelect>> select = null,
            Expression<Func<T, bool>> where = null,
            Expression<Func<T, TOrderBy>> orderBy = null, bool orderDescending = false,
            int take = 0) where TOrderBy : IComparable
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TSelect, TOrderBy>(db, select, where, orderBy, orderDescending, take);
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }

        public static List<TSelect> SelectDistinct<TSelect>(Expression<Func<T, TSelect>> select, Expression<Func<T, bool>> where = null, int take = 0)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TSelect, int>(db: db, select: select, where: where, take: take).Distinct();
                return query.ToList();
            }
        }

        public static async Task<List<TSelect>> SelectDistinctAsync<TSelect>(Expression<Func<T, TSelect>> select, Expression<Func<T, bool>> where = null, int take = 0)
        {
            using (var db = GetDbContext())
            {
                var query = GetQuery<TSelect, int>(db: db, select: select, where: where, take: take).Distinct();
                return await query.ToListAsync().ConfigureAwait(false);
            }
        }
        #endregion

        #region Aggregates
        public static long Count(Expression<Func<T, bool>> where = null)
        {
            using (var db = GetDbContext())
            {
                return where == null ? db.Set<T>().LongCount() : db.Set<T>().LongCount(where);
            }
        }

        public static async Task<long> CountAsync(Expression<Func<T, bool>> where = null)
        {
            using (var db = GetDbContext())
            {
                return await (where == null ? db.Set<T>().LongCountAsync() : db.Set<T>().LongCountAsync(where)).ConfigureAwait(false);
            }
        }

        public static decimal? Sum(Expression<Func<T, decimal?>> select, Expression<Func<T, bool>> where = null)
        {
            using (var db = GetDbContext())
            {
                return where == null ? db.Set<T>().Sum(select) : db.Set<T>().Where(where).Sum(select);
            }
        }

        public static async Task<decimal?> SumAsync(Expression<Func<T, decimal?>> select, Expression<Func<T, bool>> where = null)
        {
            using (var db = GetDbContext())
            {
                if (where == null)
                    return await db.Set<T>().SumAsync(select).ConfigureAwait(false);

                return await db.Set<T>().Where(where).SumAsync(select).ConfigureAwait(false);
            }
        }

        public static decimal? Average(Expression<Func<T, decimal?>> select, Expression<Func<T, bool>> where = null)
        {
            using (var db = GetDbContext())
            {
                return where == null ? db.Set<T>().Average(select) : db.Set<T>().Where(where).Average(select);
            }
        }

        public static async Task<decimal?> AverageAsync(Expression<Func<T, decimal?>> select, Expression<Func<T, bool>> where = null)
        {
            using (var db = GetDbContext())
            {
                if (where == null)
                    return await db.Set<T>().AverageAsync(select).ConfigureAwait(false);

                return await db.Set<T>().Where(where).AverageAsync(select).ConfigureAwait(false);
            }
        }

        public static void RethrowValidationException(DbEntityValidationException e)
        {
            string message = "There was a problem validating the submitted data. The following error(s) were reported:";

            foreach (var error in e.EntityValidationErrors)
                foreach (var errorEntry in error.ValidationErrors)
                    message += " " + errorEntry.ErrorMessage;

            throw new ApplicationException(message);
        }
        #endregion

        #region Private methods
        /*//Fire-and-forget
		private static async Task UpdateCacheAsync(T entity, bool deleted = false) //false = update
		{
			//if (!EnableCaching)
				//return;

			throw new NotImplementedException();
		}*/

        private static IQueryable<T> GetQuery<TOrderBy>(DbContext db,
            Expression<Func<T, bool>> where = null,
            Expression<Func<T, TOrderBy>> orderBy = null, bool orderDescending = false,
            int take = 0,
            Expression<Func<T, object>>[] include = null) where TOrderBy : IComparable
        {
            IQueryable<T> query = db.Set<T>().AsNoTracking();

            foreach (var includeProperty in include)
                query = query.Include(includeProperty);

            if (where != null)
                query = query.Where(where);

            if (orderBy != null)
                query = orderDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            if (take > 0)
                query = query.Take(take);
            return query;
        }

        private static IQueryable<TSelect> GetQuery<TSelect, TOrderBy>(DbContext db,
            Expression<Func<T, TSelect>> select,
            Expression<Func<T, bool>> where = null,
            Expression<Func<T, TOrderBy>> orderBy = null, bool orderDescending = false,
            int take = 0) where TOrderBy : IComparable
        {
            IQueryable<T> query = db.Set<T>().AsNoTracking();

            if (where != null)
                query = query.Where(where);

            if (orderBy != null)
                query = orderDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            if (take > 0)
                query = query.Take(take);

            IQueryable<TSelect> finalQuery;

            if (select != null)
                finalQuery = query.Select(select);
            else
            {
                if (typeof(T) != typeof(TSelect))
                    throw new ArgumentException("When a select expression is not specified, the types T and TSelect must be identical.");

                finalQuery = query as IQueryable<TSelect>;
            }

            return finalQuery;
        }

        private static Expression<Func<T, bool>> GetIdPredicate(long id)
        {
            var param = Expression.Parameter(typeof(T), "e");
            string keyColumnName = GetKeyColumnName(typeof(T));

            var expressionProperty = Expression.Property(param, keyColumnName);

            if (!SupportNonLongKeys)
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(expressionProperty, Expression.Constant(id)), param);

            var property = typeof(T).GetProperty(keyColumnName);
            ConstantExpression expressionConstant = null;

            if (property.PropertyType == typeof(long))
                expressionConstant = Expression.Constant(id);
            else if (property.PropertyType == typeof(int))
                expressionConstant = Expression.Constant((int)id);
            else if (property.PropertyType == typeof(short))
                expressionConstant = Expression.Constant((short)id);
            else if (property.PropertyType == typeof(byte))
                expressionConstant = Expression.Constant((byte)id);
            else
                throw new InvalidOperationException("Id field \"" + property.Name + " of type \"" + typeof(T).FullName + " is not of a supported type.");

            return Expression.Lambda<Func<T, bool>>(Expression.Equal(expressionProperty, expressionConstant), param);
        }

        private static Expression<Func<T, bool>> GetIdPredicate(IEnumerable<long> ids)
        {
            Expression<Func<T, bool>> expression = null;

            foreach (long id in ids)
            {
                if (expression == null)
                    expression = GetIdPredicate(id);
                else
                    expression = expression.Or(GetIdPredicate(id));
            }

            return expression;
        }

        private static long GetId(T entity)
        {
            Type entityType = typeof(T);
            string idColumnName = GetKeyColumnName(entityType);
            var property = entityType.GetProperty(idColumnName);

            if (property == null)
                throw new Exception("Entities of type " + entityType.FullName + " does not contain the expected Id field. Expected name was: " + idColumnName);

            return Convert.ToInt64(property.GetValue(entity));
        }

        private static void SetEntityState(IEnumerable<T> entities, DbContext db)
        {
            Type entityType = typeof(T);
            string idColumnName = GetKeyColumnName(entityType);
            var property = entityType.GetProperty(idColumnName);

            if (property == null)
                throw new Exception("Entities of type " + entityType.FullName + " does not contain the expected Id field. Expected name was: " + idColumnName);

            foreach (T entity in entities)
                db.Entry(entity).State = Convert.ToInt64(property.GetValue(entity)) > 0 ? System.Data.Entity.EntityState.Modified : System.Data.Entity.EntityState.Added;

        }

        private static async Task HandlemanyToManyPropertyAsync<M>(long id, Expression<Func<T, ICollection<M>>> manyToManyProperty, bool forDeletion = false) where M : class
        {
            using (var db = GetDbContext())
            {
                var predicate = GetIdPredicate(id);
                var entity = db.Set<T>().SingleOrDefault(predicate);
                await HandlemanyToManyPropertyAsync<M>(entity, manyToManyProperty, forDeletion, db).ConfigureAwait(false);
            }
        }

        private static async Task HandlemanyToManyPropertyAsync<M>(T entity, Expression<Func<T, ICollection<M>>> manyToManyProperty, bool forDeletion = false) where M : class
        {
            using (var db = GetDbContext())
            {
                await HandlemanyToManyPropertyAsync<M>(entity, manyToManyProperty, forDeletion, db).ConfigureAwait(false);
            }
        }

        private static async Task HandlemanyToManyPropertyAsync<M>(T entity, Expression<Func<T, ICollection<M>>> manyToManyProperty, bool forDeletion, DbContext db)
            where M : class
        {
            var property = db.Entry(entity).Collection(manyToManyProperty);

            var id = GetId(entity);
            var predicate = GetIdPredicate(id);
            T dbEntity = await db.Set<T>().Include(property.Name).SingleOrDefaultAsync(predicate).ConfigureAwait(false);

            if (dbEntity == null)
                return;

            var dbMappings = db.Entry(dbEntity).Collection(manyToManyProperty).CurrentValue;
            var newMappings = forDeletion ? new List<M>() : db.Entry(entity).Collection(manyToManyProperty).CurrentValue;

            /*foreach (M value in newMappings)
				db.Entry(value).State = EntityState.Unchanged;*/

            dbMappings.Clear();

            foreach (var newMapping in newMappings)
            {
                id = Data<M>.GetId(newMapping);
                var mPredicate = Data<M>.GetIdPredicate(id);
                var dbMappingEntity = await db.Set<M>().SingleOrDefaultAsync(mPredicate).ConfigureAwait(false);

                if (dbMappingEntity == null)
                    continue;

                dbMappings.Add(dbMappingEntity);
            }

            await db.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion
    }
    #endregion

    // NOTE: The following non-generic class is mainly provided to allow simpler syntax like Data.Save(myEntity) rather than Data<MyEntityType>.Save(myEntity).
    // It also serves as home to truly global variables that do not need to be instantiated once per class.
    // However a few methods are included to provide backwards compatibility with previous versions of the Data class. 

    public static class Data
    {
        #region Configuration and fundamentals

        private static Type DbContextType { get; set; }

        private static int timeoutSeconds;
        //private static bool EnableCaching { get; set; } //Not implemented.
        internal static Func<Type, string> GetKeyColumnName;

        public static string ConnectionString { get; private set; }
        public static string DatabaseName { get; private set; }

        static Data()
        {
            //Customize this method as needed.
            Configure(typeof(IntegrationMakerEntities));

            //Get default connection info.
            using (var db = GetDbContext())
            {
                ConnectionString = db.Database.Connection.ConnectionString;
                DatabaseName = db.Database.Connection.Database;
            }

            //Optionally, configure caching for specific types.
            //EnableCaching = true; //Enables caching for all types - do not use blindly.
            //Data<MyType>.ConfigureCaching(true); //Enable for one or more specific types. Cache won't be loaded until a data retrieval call is made. Note that navigation properties will not be cached.
            //Data<Server>.ConfigureCaching(true, true); //Enable for one or more specific types. Cache will be loaded immediately. Note that navigation properties will not be cached.
        }

        public static void ForceConnectionString(string connectionString)
        {
            ConnectionString = connectionString;

            using (var db = GetDbContext())
                DatabaseName = db.Database.Connection.Database;
        }

        /// <summary>
        /// Configures the class so it knows the type of your EF context class. Optionally specify a standard naming convention.
        /// </summary>
        /// <param name="dbContextType">A type that inherits from DbContext. Must have a parameterless constructor.</param>
        /// <param name="namingConvention">A value specifying the naming convention used for your entities' primary key columns.</param>
        internal static void Configure(Type dbContextType, int timeoutSeconds = 180, IdNamingConventions namingConvention = IdNamingConventions.Id)
        {
            Data.timeoutSeconds = timeoutSeconds;

            if (!dbContextType.IsAssignableFrom(dbContextType) || dbContextType.GetConstructor(System.Type.EmptyTypes) == null)
                throw new ArgumentException("dbContextType must inherit from DbContext and have a public parameterless constructor.");

            DbContextType = dbContextType;

            if (namingConvention == IdNamingConventions.Id)
                GetKeyColumnName = GetKeyColumnNameUsingId;
            else if (namingConvention == IdNamingConventions.ClassNamePlusId)
                GetKeyColumnName = GetKeyColumnNameUsingClassNamePlusId;
        }

        /// <summary>
        /// Configures the static Data class so it knows the type of your EF context class. This overload allows you to specify a custom naming convention.
        /// </summary>
        /// <param name="dbContextType">A type that inherits from DbContext. Must have a parameterless constructor.</param>
        /// <param name="KeyColumnNameResolver">A function to resolve the name of an entity's primary key column based on its type.</param>
        internal static void Configure(Type dbContextType, Func<Type, string> KeyColumnNameResolver)
        {
            if (!dbContextType.IsAssignableFrom(dbContextType) || dbContextType.GetConstructor(System.Type.EmptyTypes) == null)
                throw new ArgumentException("dbContextType must inherit from DbContext and have a public parameterless constructor.");

            DbContextType = dbContextType;
            GetKeyColumnName = KeyColumnNameResolver;
        }

        private static string GetKeyColumnNameUsingId(Type t)
        {
            return "Id";
        }

        private static string GetKeyColumnNameUsingClassNamePlusId(Type t)
        {
            return t.Name + "Id";
        }
        #endregion

        #region Standard methods
        public static T Save<T>(T entity) where T : class
        {
            return Data<T>.Save(entity);
        }

        public static async Task<T> SaveAsync<T>(T entity) where T : class
        {
            return await Data<T>.SaveAsync(entity).ConfigureAwait(false);
        }

        public static T Save<T, M>(T entity, Expression<Func<T, ICollection<M>>> manyToManyProperty)
            where T : class
            where M : class
        {
            return Data<T>.Save<M>(entity, manyToManyProperty);
        }

        public static async Task<T> SaveAsync<T, M>(T entity, Expression<Func<T, ICollection<M>>> manyToManyProperty)
            where T : class
            where M : class
        {
            return await Data<T>.SaveAsync<M>(entity, manyToManyProperty).ConfigureAwait(false);
        }

        public static List<T> SaveList<T>(IEnumerable<T> entities) where T : class
        {
            return Data<T>.Save(entities);
        }

        public static async Task<List<T>> SaveListAsync<T>(IEnumerable<T> entities) where T : class
        {
            return await Data<T>.SaveAsync(entities).ConfigureAwait(false);
        }

        public static List<T> BulkSave<T>(IEnumerable<T> entities) where T : class
        {
            return Data<T>.BulkSave(entities);
        }

        public static async Task<List<T>> BulkSaveAsync<T>(IEnumerable<T> entities) where T : class
        {
            return await Data<T>.BulkSaveAsync(entities).ConfigureAwait(false);
        }

        public static void Delete<T>(T entity) where T : class
        {
            Data<T>.Delete(entity);
        }

        public static async Task DeleteAsync<T>(T entity) where T : class
        {
            await Data<T>.DeleteAsync(entity).ConfigureAwait(false);
        }

        public static void Delete<T>(Expression<Func<T, bool>> where = null) where T : class
        {
            Data<T>.Delete(where);
        }

        public static async Task DeleteAsync<T>(Expression<Func<T, bool>> where = null) where T : class
        {
            await Data<T>.DeleteAsync(where).ConfigureAwait(false);
        }

        public static void DeleteList<T>(IEnumerable<T> entities) where T : class
        {
            Data<T>.Delete(entities);
        }

        public static async Task DeleteListAsync<T>(IEnumerable<T> entities) where T : class
        {
            await Data<T>.DeleteAsync(entities).ConfigureAwait(false);
        }

        public static void Delete<T>(IEnumerable<T> entities) where T : class
        {
            Data<T>.Delete(entities);
        }

        public static async Task DeleteAsync<T>(IEnumerable<T> entities) where T : class
        {
            await Data<T>.DeleteAsync(entities).ConfigureAwait(false);
        }

        //Backwards compatibility starts here.

        public static T Get<T>(long id) where T : class
        {
            return Data<T>.Get(id);
        }
        public static List<T> GetList<T>(Func<T, bool> e = null, Boolean readOnly = true) where T : class
        {
            var result = new List<T>();

            try
            {
                using (var db = DbContextType.GetConstructor(System.Type.EmptyTypes).Invoke(new object[] { }) as DbContext)
                {
                    if (readOnly)
                    {
                        //Disable lazy loading and allow stuff to be serialized
                        db.Configuration.ProxyCreationEnabled = false;

                        if (e == null)
                        {
                            result = db.Set<T>().AsNoTracking().ToList();
                        }
                        else
                        {
                            result = db.Set<T>().AsNoTracking().Where(e).ToList();
                        }
                    }
                    else
                    {
                        if (e == null)
                        {
                            result = db.Set<T>().ToList();
                        }
                        else
                        {
                            result = db.Set<T>().Where(e).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                String stop = "";
                throw;
            }

            return result;
        }
        public static List<T> Get<T>(Expression<Func<T, bool>> where = null) where T : class
        {
            return Data<T>.Get(where);
        }

        public static long Count<T>(Expression<Func<T, bool>> where = null) where T : class
        {
            return Data<T>.Count(where);
        }

        public static List<T> SaveMany<T>(IEnumerable<T> entities) where T : class
        {
            var list = entities as List<T> ?? entities.ToList();

            if (list.Count > 100)
                return Data<T>.BulkSave(list);

            return Data<T>.Save(list);
        }

        public static DbContext GetDbContext(bool proxyCreationEnabled = false)
        {
            var db = DbContextType.GetConstructor(System.Type.EmptyTypes).Invoke(new object[] { }) as DbContext;

            if (!String.IsNullOrEmpty(ConnectionString))
                db.Database.Connection.ConnectionString = ConnectionString;

            db.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
            db.Database.CommandTimeout = timeoutSeconds;

            return db;
        }
        #endregion        
    }
}