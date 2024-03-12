using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TransferItem : MonoBehaviour
    {
        [SerializeField] private TMP_InputField address;
        
        private Nft _nft;

        public void ShowTransferPanel(Nft nft)
        {
            _nft = nft;
            gameObject.SetActive(true);
        }

        public void Transfer()
        {
            UserDataManager.Instance.TransferToken(_nft.TokenId, address.text);
        }
    }
}
