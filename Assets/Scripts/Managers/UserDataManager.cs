using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Type = Nft.NftType;

namespace Managers
{
    public class UserDataManager : MonoBehaviour
    {
        public static UserDataManager Instance; 
        
        private List<Nft> _userNfts;
        private Dictionary<string, object> _equipNfts;
        
        public Action<List<Nft>> nftsReceived;

        private void Awake()
        {
            _equipNfts = new Dictionary<string, object>();
            
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            SceneManager.activeSceneChanged += ChangedActiveScene;
            /*
            *
            * Test case
            *     
            */
            _userNfts = new List<Nft>
            {
                new Nft 
                {
                    Name = "Viper",
                    Description = "This gun will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Viper.png",
                    Type = Type.Gun
                },
                new Nft 
                {
                    Name = "Claw",
                    Description = "This gun will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Claw.png",
                    Type = Type.Gun
                },
                new Nft 
                {
                    Name = "Sealer",
                    Description = "This SMG will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Sealer.png",
                    Type = Type.Smg
                },
                new Nft 
                {
                    Name = "Peacock",
                    Description = "This SMG will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Peacock.png",
                    Type = Type.Smg
                },
                new Nft 
                {
                    Name = "Defender",
                    Description = "This Shotgun will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Defender.png",
                    Type = Type.Shotgun
                },
                new Nft 
                {
                    Name = "DoomGuy",
                    Description = "This Shotgun will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/DoomGuy.png",
                    Type = Type.Shotgun
                },
                new Nft 
                {
                    Name = "Roaster",
                    Description = "This explosive will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Roaster.png",
                    Type = Type.Explosive
                },
                new Nft 
                {
                    Name = "Mines",
                    Description = "This explosive will go up your ass",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmUAMLrvyLf8nDC8cqX65C6ESSQNeMdabXBm4oZkQ6bvBx/Mines.png",
                    Type = Type.Explosive
                },
                new Nft 
                {
                    Name = "Tits Armor",
                    Description = "These tits will give you pleasure",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmVgpSsvkm5b5r9EtBcpfh3KhxLhKtPvMi3LyduZXWumVG",
                    Value = 2,
                    Type = Type.Armor
                },
                new Nft 
                {
                    Name = "Double Tits Armor",
                    Description = "These tits will give you pleasure",
                    ThumbnailUri = "https://ipfs.io/ipfs/QmRWxsMzeh8zEMJF6tnSxwi8zJeLXRD5tTdtCCquEdvaih",
                    Value = 4,
                    Type = Type.Armor
                },
                new Nft 
                {
                    Name = "Health",
                    Description = "This module increases the initial health level by 5%.",
                    Value = 5,
                    ThumbnailUri = "https://ipfs.io/ipfs/QmW8sa5UygUKg58LLzK7NoEDtCRyAQU4wZh1rbpFa6j7kP",
                    Type = Type.Module
                },
                new Nft 
                {
                    Name = "Damage",
                    Description = "This module increases the initial damage level by 10%.",
                    Value = 10,
                    ThumbnailUri = "https://ipfs.io/ipfs/QmXHndHfSMuNCvSU44EKN4wQ2fjdLbfy1NYXmLK91pJupW",
                    Type = Type.Module
                }
            };

            nftsReceived?.Invoke(_userNfts);
            /*
            *
            * Test case
            *     
            */
        }
        
        private void ChangedActiveScene(Scene current, Scene next)
        {
            if (next.name == "Main")
            {
                _equipNfts.Clear();
            }
        }


        public void Equip(object item)
        {
            if (item is NftInventoryItem module)
            {
                switch (module.type)
                {
                    case Type.Module:
                        _equipNfts.Add(module.title, module);       
                        break;
                    
                    case Type.Gun:
                        _equipNfts.Add(Type.Gun.ToString(), module);
                        break;
                    
                    case Type.Smg:
                        _equipNfts.Add(Type.Smg.ToString(), module);
                        break;
                    
                    case Type.Shotgun:
                        _equipNfts.Add(Type.Shotgun.ToString(), module);
                        break;
                    
                    case Type.Armor:
                        _equipNfts.Add(Type.Armor.ToString(), module);
                        break;
                    
                    case Type.Explosive:
                        _equipNfts.Add(Type.Explosive.ToString(), module);
                        break;
                    
                    case Type.Ability:
                        _equipNfts.Add(module.title, module);
                        break;
                }
            }
        }
        
        public void Unequip(object item)
        {
            if (item is NftInventoryItem module)
            {
                switch (module.type)
                {
                    case Type.Module:
                        _equipNfts.Remove(module.title);       
                        break;
                    
                    case Type.Gun:
                        _equipNfts.Remove(Type.Gun.ToString());
                        break;
                    
                    case Type.Smg:
                        _equipNfts.Remove(Type.Smg.ToString());
                        break;
                    
                    case Type.Shotgun:
                        _equipNfts.Remove(Type.Shotgun.ToString());
                        break;
                    
                    case Type.Armor:
                        _equipNfts.Remove(Type.Armor.ToString());
                        break;
                    
                    case Type.Explosive:
                        _equipNfts.Remove(Type.Explosive.ToString());
                        break;
                    
                    case Type.Ability:
                        _equipNfts.Remove(module.title);
                        break;
                }
            }
        }

        public Dictionary<string, object> GetEquipment()
        {
            return _equipNfts;
        }

        public List<Nft> GetUserNfts()
        {
            return _userNfts;
        }
    }
}
