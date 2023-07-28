
using System;
using System.Linq;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
	{
		public static void Initialize(HomeBankingContext context)
		{
			if (!context.Clients.Any())
			{
				//creamos los datos de prueba
				var clients = new Client[]
				{
					new Client { 
						
						FirstName="Eduardo", 
						LastName="Mendoza", 
						Email = "eduardomendoza@gmail.com",  
						Password="123456",},

					new Client {

						FirstName="Rafael",
						LastName="Rodriguez",
						Email = "rafarodriguez@gmail.com",
						Password="123456",},

					new Client
					{
						FirstName="Catalina",
						LastName="Tempra",
						Email="catatempra@gmail.com",
						Password="654321",},

				};

				//cada vez que se agrega un cliente lo guarda 
				foreach (Client client in clients)
				{
					context.Clients.Add(client);
				}

                //guardamos todo
                context.SaveChanges();
            }


             if (!context.Accounts.Any())
            {
                int NumberAcount = 1;
                var clients = context.Clients.ToList();
                foreach (Client client in clients)
                {
                    Random rnd = new Random();
                    Account account = new Account
                    {
                        ClientId = client.Id,
                        CreationDate = DateTime.Now,
                        Number = "VIN00" + NumberAcount.ToString(),
                        Balance = rnd.Next(1000, 50000)
                    };
                    context.Accounts.Add(account);
                    NumberAcount++;
                }
                context.SaveChanges();
            }


           if (!context.Transactions.Any())

            { var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");

                if (account1 != null)

                {
                    var transactions = new Transaction[]

                    {   new Transaction { AccountId= account1.Id, Amount = 100000, Date= DateTime.Now.AddHours(-4), Description = "Transferencia reccibida", Type = TransactionType.CREDIT.ToString() },

                        new Transaction { AccountId= account1.Id, Amount = -20000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre por un auto", Type = TransactionType.DEBIT.ToString() },

                        new Transaction { AccountId= account1.Id, Amount = -300, Date= DateTime.Now.AddHours(-8), Description = "Compra en tienda amazon", Type = TransactionType.DEBIT.ToString() },

                    };

                    foreach (Transaction transaction in transactions)

                    {context.Transactions.Add(transaction);}

                    context.SaveChanges();



                }

            }


        }
    }
}
