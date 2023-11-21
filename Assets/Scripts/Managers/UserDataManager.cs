using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UserDataManager : MonoBehaviour
    {
        private List<Nft> _userNfts;
        public Action<List<Nft>> nftsReceived;
        void Start()
        {
            /*
           *
           * Test case
           *     
           */
            _userNfts = new List<Nft>();
            _userNfts.Add(new Nft 
            {
                Name = "Health",
                Description = "This module increases the initial health level by 5%.",
                Value = "5",
                ThumbnailUri = "https://ipfs.io/ipfs/QmW8sa5UygUKg58LLzK7NoEDtCRyAQU4wZh1rbpFa6j7kP"
            });
            _userNfts.Add(new Nft 
            {
                Name = "Damage",
                Description = "This module increases the initial damage level by 10%.",
                Value = "10",
                ThumbnailUri = "https://ipfs.io/ipfs/QmXHndHfSMuNCvSU44EKN4wQ2fjdLbfy1NYXmLK91pJupW"
            });

            nftsReceived?.Invoke(_userNfts);
            /*
            *
            * Test case
            *     
            */
        }
    }
}
