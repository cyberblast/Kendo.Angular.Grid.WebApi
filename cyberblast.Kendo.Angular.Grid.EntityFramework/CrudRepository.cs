using cyberblast.Kendo.Angular.Grid.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace cyberblast.Kendo.Angular.Grid.EntityFramework {
    public abstract class CrudRepository<TContext, TEntity, TId>
        where TContext : DbContext, new()
        where TEntity : class, IGenericEntity<TId> {

        public CrudRepository(DbContext context) {

        }

        protected static void OnDb(Action<TContext, DbSet<TEntity>> handler) {
            using (var context = new TContext()) {
                handler(context, context.Set<TEntity>());
            }
        }
        protected static async Task OnDbAsync(Func<DbContext, DbSet<TEntity>, Task> handler) {
            using (var context = new TContext()) {
                await handler(context, context.Set<TEntity>());
            }
        }

        public static TId Create(TEntity entity, Func<TId> createId = null) {
            if (createId != null && entity.Id.Equals(default(TId)))
                entity.Id = createId();
            OnDb((context, table) => {
                entity = table.Add(entity);
                context.SaveChanges();
            });
            return entity.Id;
        }

        public static TEntity CreateGet(TEntity entity, Func<TId> createId = null) {
            TEntity item = null;
            if (createId != null && entity.Id.Equals(default(TId)))
                entity.Id = createId();
            OnDb((context, table) => {
                entity = table.Add(entity);
                context.SaveChanges();
                item = table.FirstOrDefault(s => s.Id.ToString() == entity.Id.ToString());
            });
            return item;
        }

        public static TEntity CreateGet(TEntity entity, Func<TId> createId = null, params string[] includes) {
            TEntity item = null;
            if (createId != null && entity.Id.Equals(default(TId)))
                entity.Id = createId();
            OnDb((context, table) => {
                entity = table.Add(entity);
                context.SaveChanges();
                DbQuery<TEntity> query = table;
                if (includes != null && includes.Length > 0) {
                    foreach (string include in includes) {
                        query = query.Include(include);
                    }
                }
                item = query.FirstOrDefault(s => s.Id.ToString() == entity.Id.ToString());
            });
            return item;
        }

        public static TEntity Read(TId id) {
            TEntity item = null;
            OnDb((context, table) => {
                item = table.FirstOrDefault(s => id.ToString() == s.Id.ToString());
            });
            return item;
        }

        public static TEntity Read(Expression<Func<TEntity, bool>> filter, params string[] includes) {
            TEntity item = null;
            OnDb((context, table) => {
                DbQuery<TEntity> entity = table;
                if (includes != null && includes.Length > 0) {
                    foreach (string include in includes) {
                        entity = entity.Include(include);
                    }
                }
                item = entity.FirstOrDefault(filter);
            });
            return item;
        }

        public static TEntity Update(TEntity entity, Action<TEntity> map) {
            TEntity item = null;
            OnDb((context, table) => {
                var editor = table.FirstOrDefault(s => s.Id.ToString() == entity.Id.ToString());
                map(editor);
                context.SaveChanges();
                item = table.First(s => s.Id.ToString() == entity.Id.ToString());
            });
            return item;
        }

        public static void Delete(TId id) {
            OnDb((context, table) => {
                var delete = table.FirstOrDefault(s => s.Id.ToString() == id.ToString());
                if (delete != null && delete.Id.Equals(id)) {
                    table.Remove(delete);
                    context.SaveChanges();
                }
            });
        }

        public static List<TEntity> List(Expression<Func<TEntity, bool>> filter = null, params string[] includes) {
            List<TEntity> items = null;
            OnDb((context, table) => {
                DbQuery<TEntity> entity = table;
                if (includes != null && includes.Length > 0) {
                    foreach (string include in includes) {
                        entity = entity.Include(include);
                    }
                }
                IQueryable<TEntity> query;
                if (filter == null) {
                    query = entity;
                } else {
                    query = entity.Where(filter);
                }

                items = query.ToList();
            });
            return items;
        }

        public static List<TEntity> List(Expression<Func<TEntity, bool>> filter, List<SortDescriptor> sortDescriptors, params string[] includes) {
            List<TEntity> items = null;
            OnDb((context, table) => {
                DbQuery<TEntity> entity = table;
                if (includes != null && includes.Length > 0) {
                    foreach (string include in includes) {
                        entity = entity.Include(include);
                    }
                }
                IQueryable<TEntity> query;
                if (filter == null) {
                    query = entity;
                } else {
                    query = entity.Where(filter);
                }

                if (sortDescriptors != null && sortDescriptors.Count > 0) {
                    IOrderedQueryable<TEntity> ordered = null;
                    foreach (var sortDescriptor in sortDescriptors) {
                        if (ordered == null) {
                            ordered = query.OrderBy(sortDescriptor.Field, sortDescriptor.Dir == "desc");
                        } else {
                            ordered = ordered.OrderBy(sortDescriptor.Field, sortDescriptor.Dir == "desc");
                        }
                    }
                    query = ordered;
                }

                items = query.ToList();
            });
            return items;
        }

        public static List<TEntity> List(Expression<Func<TEntity, bool>> filter, List<SortDescriptor> sortDescriptors, int skip, int take, out int total, params string[] includes) {
            List<TEntity> items = null;
            int localTotal = 0;
            OnDb((context, table) => {
                DbQuery<TEntity> entity = table;
                if (includes != null && includes.Length > 0) {
                    foreach (string include in includes) {
                        entity = entity.Include(include);
                    }
                }
                IQueryable<TEntity> query;
                if (filter == null) {
                    query = entity;
                } else {
                    query = entity.Where(filter);
                }

                if (sortDescriptors != null && sortDescriptors.Count > 0) {
                    IOrderedQueryable<TEntity> ordered = null;
                    foreach (var sortDescriptor in sortDescriptors) {
                        if (ordered == null) {
                            ordered = query.OrderBy(sortDescriptor.Field, sortDescriptor.Dir == "desc");
                        } else {
                            ordered = ordered.OrderBy(sortDescriptor.Field, sortDescriptor.Dir == "desc");
                        }
                    }
                    query = ordered;
                }

                localTotal = query.Count();
                items = query.Skip(skip).Take(take).ToList();
            });
            total = localTotal;
            return items;
        }

        public static List<TEntity> List(string sqlQuery, params object[] parameters) {
            List<TEntity> items = null;
            OnDb((context, table) => {
                items = table.SqlQuery(sqlQuery, parameters).ToList();
            });
            return items;
        }
    }
}
