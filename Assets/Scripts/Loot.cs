using UnityEngine;
using Weapons;

public class Loot : MonoBehaviour
{
    private enum LootType
    {
        Ammo,
        Weapon,
        Health,
        Shield,
        Nft
    }
    
    [SerializeField] private float lootValue;
    [SerializeField] private float destructionTime;
    [SerializeField] private LootType type;
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DestroyLoot), destructionTime);
    }

    public void ApplyLoot(GameObject owner)
    {
        owner.TryGetComponent<PlayerController>(out var playerScript);
        
        if (playerScript == null) return;
        
        switch (type)
        {
            case LootType.Weapon:
                var weapons = playerScript.GetEquippedWeapons();
                foreach (var weapon in weapons)
                {
                    if (name != weapon.Key) continue;
                    
                    var weaponScript = weapon.Value.GetComponent<Weapon>();
                    weaponScript.isUnlocked = false;
                    playerScript.SwitchWeapon(
                        weapon: weapon.Value,
                        isTaken: true);
                    weaponScript.ChangeAmmoQty((int)lootValue);
                    break;
                }
                break;
            
            case LootType.Ammo:
                foreach (var w in playerScript.GetEquippedWeapons())
                {
                    if (name != w.Key) continue;
                    
                    w.Value.GetComponent<Weapon>().ChangeAmmoQty((int)lootValue);
                    break;
                }
                break;
            
            case LootType.Health:
                playerScript.ChangeHealth(
                    healthValue: (int) lootValue,
                    damaged: false);
                break;
            
            case LootType.Shield:
                var shield = playerScript.GetPlayerShield();
                shield.gameObject.SetActive(true);
                shield.Activate(lootValue);
                break;
            
            case LootType.Nft:
                // todo: action of picking up an item
                break;
        }
    }

    private void DestroyLoot()
    {
        Destroy(gameObject);
    }
}
