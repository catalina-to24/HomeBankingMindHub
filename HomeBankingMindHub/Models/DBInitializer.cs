
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





        }
    }
}
