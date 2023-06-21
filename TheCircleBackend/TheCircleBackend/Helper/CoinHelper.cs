using TheCircleBackend.DBInfra;
using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Helper
{
    public class CoinHelper : ICoinHelper
    {
        private DomainContext domainContext { get; set; }
        private int userId { get; set; }

        public CoinHelper(DomainContext websiteUserRepo, int userId)
        {
            this.domainContext = websiteUserRepo;
            this.userId = userId;
        }

        // Sets the endDate of the stream to eventually increase the user's balance
        public void ChangeBalance(DateTime startDate, DateTime endDate)
        {
            TimeSpan timeDiff = endDate - startDate;                        // Calculates time difference
            int hours = Convert.ToInt32(Math.Floor(timeDiff.TotalHours));   // Cuts off overtime and converts to an int

            GetCoinCounter(hours);
        }

        // Get the amount of coins the user will receive
        private void GetCoinCounter(int hours)
        {
            int coins = 0;

            for (int i = 0; i < hours; i++)                     // Coin gain is doubled each hour and added onto the previous total
            {
                coins += (int)Math.Pow(2, i);
            }
            //int coins = (int)Math.Pow(2, hours - 1);          // Coin total is doubled each hour

            IncreaseBalance(coins);
        }

        // Update the user, adds coins to balance
        private void IncreaseBalance(int coins)
        {
            try
            {
                var user = this.domainContext.WebsiteUser.First(vs => vs.Id == this.userId);    // Get user with Id
                user.Balance += coins;                                                          // Increases user's balance
                this.domainContext.WebsiteUser.Update(user);                                    // Updates database
                this.domainContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
