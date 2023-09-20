﻿using e_Estoque.CrossCutting.Notifications;
using e_Estoque.Data.Context;
using e_Estoque.Domain.Entities;
using e_Estoque.Domain.Interfaces.Repositories;

namespace e_Estoque.Data.Repositories
{
    public class CustomerAdressRepository : Repository<CustomerAdress>, ICustomerAdressRepository
    {
        public CustomerAdressRepository(
            EstoqueDbContext db, 
            INotifier notifier) : base(db, notifier)
        {
        }
    }
}
