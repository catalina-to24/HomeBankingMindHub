﻿using HomeBankingMindHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBankingMindHub.Repositories
{
    public class ClientLoanRepository: RepositoryBase<ClientLoan>, IClientLoanRepository
    {
        public ClientLoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }
        public void Save(ClientLoan clientLoan)
        {
            SaveChanges();
        }
    }
}
