using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private AccountsController _accountsController;
        private CardsController _cardsController;
        private IAccountRepository _accountRepository;
        public ClientsController(IClientRepository clientRepository, AccountsController accountsController, CardsController cardsController, IAccountRepository accountRepository)

        {
            _clientRepository = clientRepository;
            _accountsController = accountsController;
            _cardsController = cardsController;
            _accountRepository = accountRepository;
        }

        [HttpGet]

        public IActionResult Get()

        {
            try

            {
                var clients = _clientRepository.GetAllClients();

                var clientsDTO = new List<ClientDTO>();


                foreach (Client client in clients)

                {

                    var newClientDTO = new ClientDTO

                    {
                        Id = client.Id,

                        Email = client.Email,

                        FirstName = client.FirstName,

                        LastName = client.LastName,

                        Accounts = client.Accounts.Select(ac => new AccountDTO

                        {
                            Id = ac.Id,

                            Balance = ac.Balance,

                            CreationDate = ac.CreationDate,

                            Number = ac.Number,

                            //no es necesario desde este controlador (client)
                            /*Transactions = ac.Transactions.Select(tr => new TransactionDTO 
                            
                            {
                                Id = tr.Id,
                                Type = tr.Type,
                                Amount = tr.Amount,
                                Description = tr.Description,
                                Date = tr.Date,
                            }).ToList(),*/




                        }).ToList(),

                        Credits = client.ClientLoans.Select

                        (cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }
                        ).ToList(), Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color,
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type
                        }).ToList()

                    };

                    clientsDTO.Add(newClientDTO);

                }


                return Ok(clientsDTO);

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
                var client = _clientRepository.FindById(id);


                if (client == null)

                { return NotFound(); }


                var clientDTO = new ClientDTO

                {

                    Id = client.Id,

                    Email = client.Email,

                    FirstName = client.FirstName,

                    LastName = client.LastName,

                    Accounts = client.Accounts.Select(ac => new AccountDTO

                    {
                        Id = ac.Id,

                        Balance = ac.Balance,

                        CreationDate = ac.CreationDate,

                        Number = ac.Number

                    }).ToList(), Credits = client.ClientLoans.Select

                        (cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }
                        ).ToList(), Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color,
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type
                        }).ToList()

                };

                return Ok(clientDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {   //user= cualquier persona que este logueada en el back(propio del BackEnd)
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
                //crea objeto DTO
                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost] //genera un usuario nuevo (informacion)
        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(client.Email) || String.IsNullOrEmpty(client.Password) || String.IsNullOrEmpty(client.FirstName) || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "datos inválidos");
                if (client.FirstName.Length < 3 || client.LastName.Length < 3)
                {
                    return StatusCode(400, "El nombre y el apellido debe tener mas de tres letras");
                }
                if (!Regex.IsMatch(client.FirstName, @"^[a-zA-Z\s]+$"))
                {
                    return StatusCode(400, "El nombre no puede tener caracteres");
                }
                if (!Regex.IsMatch(client.LastName, @"^[a-zA-Z\s]+$"))
                {
                    return StatusCode(400, "El apellido no puede tener caracteres");
                }
                //mail no valido
                if (!(Regex.IsMatch(client.Email, @"^([a-zA-Z0-9_.+-])+@(([a-zA-Z0-9-])+.)+([a-zA-Z0-9]{2,4})+$")))
                {

                    return StatusCode(400, "Email invalido");
                }

                if (client.Password.Length < 8)
                {
                    return StatusCode(400, "La contrasenia debe tener al menos 8 caracteres");
                }

                if (!Regex.IsMatch(client.Password, @"^(?=.[a-z])(?=.[A-Z])(?=.*\d).+$"))
                {
                    return StatusCode(400, "La contrasenia debe tener al menos de una letra mayuscula, una minuscula y un numero ");
                }
                //buscamos si ya existe el usuario
                Client user = _clientRepository.FindByEmail(client.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email esta en uso");
                }

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientRepository.Save(newClient);
                _accountsController.Post(newClient.Id);
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        public IActionResult Getaccounts()
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

                var accounts = client.Accounts;
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")] //crear hasta 3 cuentas 
        
        public IActionResult PostAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    Console.WriteLine("El mail del cliente no es valido");
                    return NotFound();
                }
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    Console.WriteLine("El cliente es null");
                    return NotFound();
                }
                if (client.Accounts.Count >2)
                {
                    Console.WriteLine("El cliente ya tiene mas de tres cuentas");
                    return StatusCode(403, "Usted ya tiene mas de 3 cuentas");
                }

                var account = _accountsController.Post(client.Id); //creamos lacuenta 
                if(account == null)
                {
                    return StatusCode(500, "Error al crear la cuenta");
                }
                return Created("", account);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        public IActionResult PostCards([FromBody] Card card)
        {
            try
            {

                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound();
                }
                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return NotFound();
                }
                if (card.Type != CardType.CREDIT.ToString() && card.Type != CardType.DEBIT.ToString())
                {
                    return BadRequest(" Tipo de tarjetas no validas");
                }
                if(card.Color != CardColor.GOLD.ToString() && card.Color != CardColor.SILVER.ToString() && card.Color != CardColor.TITANIUM.ToString())
                {
                    return BadRequest(" Color de tarjeta invalida");
                }
                int CardCount = client.Cards.Where(c => c.Type == card.Type).Count();
                if (CardCount > 2)
                {
                    return StatusCode(403, "Ya tiene 3 tarjetas de del mismo tipo");
                }
                int sameCard = client.Cards.Where(c => c.Color == card.Color && c.Type == card.Type).Count();
                if (sameCard == 1)
                {
                    return StatusCode(403, "Ya tiene una tarjetas del mismo tipo y color");
                }

                Card newCard = new Card
                {
                    CardHolder = client.FirstName + " " + client.LastName,
                    Type = card.Type,
                    Color = card.Color,
                    Number = new Random().Next(1000, 9999).ToString() + ("-") +
                             new Random().Next(1000, 9999).ToString() + ("-") +
                             new Random().Next(1000, 9999).ToString() + ("-") +
                             new Random().Next(1000, 9999).ToString(),
                    Cvv = new Random().Next(100, 999),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(4),
                    ClientId = client.Id,
                };

                var newCardDTO = _cardsController.Post(newCard);

                if (newCard == null)
                {
                    return StatusCode(500, "Error al crear la tarjeta");
                }
                return Created("", newCardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("current/cards")]
        public IActionResult GetCards()
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

                var cards = client.Cards;
                return Ok(cards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
