using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace UI
{
    public class NftInventoryCreator : MonoBehaviour
    {
        [SerializeField] private GameObject nftInventoryPrefab;
        [SerializeField] private SelectedItemPanel selectedItemPanel;

        void Awake()
        {
            var userDataManager = GameObject.FindGameObjectWithTag("Manager")
                .GetComponent<UserDataManager>();

            userDataManager.nftsReceived += DrawInventory;
        }

        private void DrawInventory(List<Nft> userNfts)
        {
            foreach (var t in userNfts)
            {
                var item = Instantiate(nftInventoryPrefab, transform);
                var script = item.GetComponent<NftInventoryItem>();
                script.InitNft((t.Name, t.Description, t.Value, t.ThumbnailUri));
                script.itemSelected += selectedItemPanel.ShowSelectedItem;
            }
        }
    }
}
