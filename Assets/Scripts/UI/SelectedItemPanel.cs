using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SelectedItemPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private TMP_Text itemValue;
        [SerializeField] private Image itemImage;

        public void ShowSelectedItem((string name, string desc, string value, Sprite sprite) nftItem)
        {
            (itemName.text, itemDescription.text, itemValue.text, itemImage.sprite) = 
                (nftItem.name, nftItem.desc, nftItem.value, nftItem.sprite);
            
            transform.GetChild(0)?.gameObject.SetActive(true);
        }
    }
}
