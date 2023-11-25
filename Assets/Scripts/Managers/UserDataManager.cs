using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UserDataManager : MonoBehaviour
    {
        private List<Nft> _userNfts;
        public Action<List<Nft>> nftsReceived;
        
        public enum NftType
        {
            Armor,
            Gun,
            Shotgun,
            Smg,
            Explosive,
            Module,
            Ability
        }
        void Start()
        {
            /*
           *
           * Test case
           *     
           */
            _userNfts = new List<Nft>
            {
                new Nft 
                {
                    Name = "Additional health",
                    Description = "This module increases the initial health level by 5%.",
                    Value = 5,
                    ThumbnailUri = "https://ipfs.io/ipfs/QmW8sa5UygUKg58LLzK7NoEDtCRyAQU4wZh1rbpFa6j7kP",
                    Type = NftType.Module
                },
                new Nft 
                {
                    Name = "Additional damage",
                    Description = "This module increases the initial damage level by 10%.",
                    Value = 10,
                    ThumbnailUri = "https://ipfs.io/ipfs/QmXHndHfSMuNCvSU44EKN4wQ2fjdLbfy1NYXmLK91pJupW",
                    Type = NftType.Module
                },
                new Nft 
                {
                    Name = "Super Gun",
                    Description = "This gun will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmXfKV67Q7RQLbK5PzPa4sZ2ahmQxSNAGatBdAsGTE2nYf",
                    Type = NftType.Gun
                },
                new Nft 
                {
                    Name = "Super SMG",
                    Description = "This SMG will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmcPozCRgp9NsVWucFRzcQyBo4atAEHQnnpkERex5dHZSH",
                    Type = NftType.Smg
                },
                new Nft 
                {
                    Name = "Super Explosive",
                    Description = "This gun will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmRkjew54Azvpw7oa6uqhq4SLkuWK5T6fYgPzaPYFEdmae",
                    Type = NftType.Explosive
                },
                new Nft 
                {
                    Name = "Tits armor",
                    Description = "These tits will give you pleasure",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmVgpSsvkm5b5r9EtBcpfh3KhxLhKtPvMi3LyduZXWumVG",
                    Value = 10,
                    Type = NftType.Armor
                }
            };

            nftsReceived?.Invoke(_userNfts);
            /*
            *
            * Test case
            *     
            */
        }
    }
}
