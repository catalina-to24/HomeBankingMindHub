
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
				};

				//cada vez que se agrega un cliente lo guarda 
				foreach (Client client in clients)
				{
					context.Clients.Add(client);
				}

				//guardamos
				context.SaveChanges();
			}

		}
	}
}
