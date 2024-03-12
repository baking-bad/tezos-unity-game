using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Type = Nft.NftType;

namespace UI
{
    public class SelectedItemPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private Image itemImage;
        [SerializeField] private TransferItem transferPopup;
        private Nft _nft;

        public void ShowSelectedNft(Nft nftItem, Sprite nftSprite)
        {
            _nft = nftItem;
            (itemName.text, itemDescription.text, itemImage.sprite) = 
                (nftItem.Name, nftItem.Description, nftSprite);
            
            gameObject.SetActive(true);
        }

        public void HideSelectedNft()
        {
            gameObject.SetActive(false);
        }

        public void ShowTransferPanel()
        {
            transferPopup.ShowTransferPanel(_nft);
        }
    }
}
