using System.Linq;
using Managers;


namespace UI
{
    public class RewardsCreator : NftInventoryCreator
    {
        protected override void Start()
        {
            UserDataManager.Instance.RewardsAndTokensLoaded += DrawInventory;
            var rewardNfts = UserDataManager
                .Instance
                .GetRewardNfts()
                .ToList();
            if (rewardNfts.Count > 0) DrawInventory(rewardNfts);
        }
    }
}