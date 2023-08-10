using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HomeBankingMindHub.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Card FindById(long id)
        {
            return FindByCondition(card => card.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Card> GetAllCards()
        {
            return FindAll().ToList();
        }

        public IEnumerable<Card> GetCardsByClient(long clientId)
        {

            return FindByCondition(card => card.ClientId == clientId)
            .ToList();

        }
        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }

        /*public string GetLastCardNumber()
        {
            var lastCard = FindAll()
                .OrderByDescending(card => card.Id)
                .FirstOrDefault();

            if (lastCard == null)
            {
                return null; // No hay cuentas en la base de datos
            }

            return lastCard.Number;
        }*/

        /*public string GenerateNextCardNumber()
        {
            string lastCardNumber = GetLastCardNumber();
            if (lastCardNumber == null)
            {
                return "3325-6745-7876-4446"; // Comienza desde un valor predeterminado si no hay cuentas
            }
            

            int lastNumber = int.Parse(lastCardNumber.Substring(4));
            int nextNumber = lastNumber + 1;
            string finalNumber = ;

          
            return nextNumber.ToString();
        }*/
    }
}
