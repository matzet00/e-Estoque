﻿using e_Estoque.CrossCutting.Notifications;
using e_Estoque.Data.Context;
using e_Estoque.Domain.Entities;
using e_Estoque.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace e_Estoque.Data.Repositories
{
    public class SaleRepository : Repository<Sale>, ISaleRepository
    {
        public SaleRepository(
            EstoqueDbContext db,
            INotifier notifier) : base(db, notifier)
        {
        }

        public override async Task<IEnumerable<Sale>> Find(Expression<Func<Sale, bool>> predicate)
        {
            return await DbSet
                .AsNoTracking()
                .Include("Customer")
                .Include("SaleProduct")
                .Include("SaleProduct.Product")
                .Where(predicate)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Sale>> GetAll()
        {
            return await DbSet
                 .Include("Customer")
                 .Include("SaleProduct")
                 .Include("SaleProduct.Product")
                 .AsNoTracking()
                 .ToListAsync();
        }

        public override async Task<Sale> GetById(Guid id)
        {
            return await DbSet
                .Include("Customer")
                .Include("SaleProduct")
                .Include("SaleProduct.Product")
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public override async Task Remove(Guid id)
        {
            var entity = await GetById(id);
            entity.UpdatedAt = DateTime.Now;
            entity.DeletedAt = DateTime.Now;
            DbSet.Update(entity);
        }

        public override async Task<IEnumerable<Sale>> Search(Expression<Func<Sale, bool>> predicate = null, Func<IQueryable<Sale>, IOrderedQueryable<Sale>> orderBy = null, int? pageSize = null, int? pageIndex = null)
        {
            var query = DbSet.AsQueryable();
            var count = query.Count();
            int pages = 0;

            if (predicate != null)
            {
                query = query.Include("Customer").Include("SaleProduct").Include("SaleProduct.Product").Where(predicate);
            }

            if (pageSize != null && pageSize.HasValue && pageSize > 0)
            {
                pages = count / pageSize.Value;

                if (pageIndex != null && pageIndex.HasValue && pageIndex.Value > 0)
                {
                    if (pageIndex.Value > pages)
                    {
                        query = query.OrderBy(x => x.Id).Skip(pageSize.Value * pages).Take(pageSize.Value);
                    }
                    else
                    {
                        query = query.OrderBy(x => x.Id).Skip(pageSize.Value * pageIndex.Value).Take(pageSize.Value);
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.Id).Skip(pageSize.Value);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }


    }
}