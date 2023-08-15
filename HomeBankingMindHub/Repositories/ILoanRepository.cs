using HomeBankingMindHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBankingMindHub.Repositories
{
    public interface ILoanRepository
    {
        Loan FindById(long id);
        IEnumerable<Loan> GetAllLoans();
    }
}
