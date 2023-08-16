﻿using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;

        }


        [HttpPost]
        public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return NotFound();
                }

                var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
                if (loan == null)
                {
                    return BadRequest("El tipo de prestamo ingresado es incorrecto");
                }
                if (string.IsNullOrEmpty(loanApplicationDTO.Payments) || loanApplicationDTO.Amount <= 0 ||  string.IsNullOrEmpty(loanApplicationDTO.ToAccountNumber) || loanApplicationDTO.Payments == "0")
                {
                    return BadRequest("Por favor complete todos los campos");
                }

                if (loanApplicationDTO.Amount > loan.MaxAmount)
                {
                    return BadRequest("El monto ingresado supera el monto maximo permitido");
                }

                var paymentList = loan.Payments.Split(',');

                if (!paymentList.Contains(loanApplicationDTO.Payments))
                { 
                    return BadRequest("Seleccione las cuotas correctas por favor");
                }
               
                var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);

                if (account == null)
                {
                    return Forbid();
                }
                var sameAcc = client.Accounts.Where(acc => acc.Number == account.Number).FirstOrDefault();
                if (sameAcc == null)
                {
                    return BadRequest("La cuenta de destino no coincide");
                }

                account.Balance += loanApplicationDTO.Amount;
                _accountRepository.Save(account);

                Transaction transaction = new Transaction
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = loanApplicationDTO.Amount,
                    Description = loan.Name.ToString() + " loan approved",
                    Date = DateTime.Now,
                    AccountId = account.Id
                };

                _transactionRepository.Save(transaction);

                ClientLoan clientLoan = new ClientLoan
                {
                    Amount = loanApplicationDTO.Amount + loanApplicationDTO.Amount * 0.2,
                    Payments = loanApplicationDTO.Payments,
                    ClientId = client.Id,
                    LoanId = loan.Id
                };

                _clientLoanRepository.Save(clientLoan);

                return Ok(loanApplicationDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();
                var loansDTO = new List<LoanDTO>();
                foreach (Loan loan in loans)
                {
                    LoanDTO loanDTO = new LoanDTO() 
                    { 
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,

                    };
                    loansDTO.Add(loanDTO);

                }
                return Ok(loansDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
