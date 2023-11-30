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

        public void ShowSelectedItem((string name, string desc, Sprite sprite) nftItem)
        {
            (itemName.text, itemDescription.text, itemImage.sprite) = 
                (nftItem.name, nftItem.desc, nftItem.sprite);
            

            gameObject.SetActive(true);
        }

        public void HideSelectedItem()
        {
            gameObject.SetActive(false);
        }
    }
}
