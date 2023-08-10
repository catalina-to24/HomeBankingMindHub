﻿using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;
        

        public AccountsController(IAccountRepository accountRepository)

        {
            _accountRepository = accountRepository;
   
        }

        [HttpGet]

        public IActionResult Get()
        {
            try

            {
                var accounts = _accountRepository.GetAllAccounts();

                var accountsDTO = new List<AccountDTO>();


                foreach (Account account in accounts)

                {

                    var newAccountDTO = new AccountDTO

                    {
                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(tr => new TransactionDTO
                        {
                            Id = tr.Id,

                            Type = tr.Type,

                            Amount = tr.Amount,

                            Description = tr.Description,

                            Date = tr.Date,

                        }).ToList()

                    };

                    accountsDTO.Add(newAccountDTO);

                }


                return Ok(accountsDTO);

            }

            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpGet("{id}")]

        public IActionResult Get(long id)

           {

            try

            {
                var account = _accountRepository.FindById(id); if (account == null)

                { return NotFound(); }


                var accountDTO = new AccountDTO

                {

                    Id = account.Id,

                    Number = account.Number,

                    CreationDate = account.CreationDate,

                    Balance = account.Balance,

                    Transactions = account.Transactions.Select(tr => new TransactionDTO

                    {
                        Id = tr.Id,

                        Type = tr.Type,

                        Amount = tr.Amount,

                        Description = tr.Description,

                        Date = tr.Date,
                    }).ToList()
                };

                return Ok(accountDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpPost] 
        
        public IActionResult Post(long clientId)
        {
            try
            {
                string accountNumber = _accountRepository.GenerateNextAccountNumber();
                
                Account newAccount = new Account
                {
                    ClientId = clientId,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    Number = accountNumber /*new Random().Next(100000, 999999).ToString()*/

                };

                _accountRepository.Save(newAccount);
                AccountDTO newaccDTO = new AccountDTO
                {
                    Id = newAccount.Id,
                    Balance = newAccount.Balance,
                    CreationDate = newAccount.CreationDate,
                    Number = newAccount.Number
                };
                return Created("", newaccDTO);
                
             
            }
            catch
            {
                return null;
            }
        }

    }
}
