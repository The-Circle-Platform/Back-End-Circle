using TheCircleBackend.Domain.Models;
using TheCircleBackend.DomainServices.IRepo;

namespace TheCircleBackend.Helper
{
    public class CoinHelper
    {
        private IWebsiteUserRepo? websiteUserRepo { get; set; }
        private WebsiteUser user { get; set; }
        private DateTime startDate;
        private DateTime endDate;

        public CoinHelper(IWebsiteUserRepo websiteUserRepo, WebsiteUser user)
        {
            this.websiteUserRepo = websiteUserRepo;
            this.user = user;
            startDate = DateTime.Now;
        }

        public CoinHelper()
        {
            startDate = DateTime.Now;
        }

        // Sets the endDate of the stream to eventually increase the user's balance
        public void StopTimer()
        {
            //this.endDate = DateTime.Now;                                    // Sets endDate to Now 
            this.endDate = DateTime.Now.AddHours(5); // ### TEST DATA ###

            TimeSpan timeDiff = this.endDate - this.startDate;              // Calculates time difference
            int hours = Convert.ToInt32(Math.Floor(timeDiff.TotalHours));   // Cuts off overtime and converts to an int

            GetCoinCounter(hours);
        }

        // Get the amount of coins the user will receive
        private void GetCoinCounter(int hours)
        {
            int coins = 0;

            for (int i = 0; i < hours; i++)
            {
                coins += (int)Math.Pow(2, i);           // Coin gain is doubled each hour and added onto the previous total
            }
            //int coins = (int)Math.Pow(2, hours - 1);  // Coin total is doubled each hour

            IncreaseBalance(coins);
        }

        // Update the user, adds coins to balance
        private void IncreaseBalance(int coins)
        {
            try
            {
                this.user.Balance += coins;                             // Increases user's balance
                this.websiteUserRepo.Update(this.user, this.user.Id);   // Updates database
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
