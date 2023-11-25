using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NftType = Managers.UserDataManager.NftType;

namespace UI
{
    public class SelectedItemPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private TMP_Text itemValue;
        [SerializeField] private Image itemImage;

        public void ShowSelectedItem((string name, string desc, float value, NftType type, Sprite sprite) nftItem)
        {
            (itemName.text, itemDescription.text, itemValue.text, itemImage.sprite) = 
                (nftItem.name, nftItem.desc, nftItem.value.ToString(), nftItem.sprite);
            
            gameObject.SetActive(true);
        }
    }
}
