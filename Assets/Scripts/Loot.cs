using UnityEngine;
using Weapons;

public class Loot : MonoBehaviour
{
    private enum LootType
    {
        Ammo,
        Weapon,
        Health,
        Shield
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
        owner.TryGetComponent<PlayerController>(out var script);
        
        if (script == null) return;
        
        switch (type)
        {
            case LootType.Weapon:
                foreach (var w in script.GetAllWeapons())
                {
                    if (name != w.name) continue;
                
                    if (!script
                            .GetUnlockedWeapons()
                            .Exists(go => go.name == name))
                    {
                        script.GetUnlockedWeapons().Add(w);
                        script.SwitchWeapon(isTaken: true);
                    }
                    
                    w.GetComponent<Weapon>().ChangeAmmoQty((int)lootValue);
                    break;
                }
                break;
            
            case LootType.Ammo:
                foreach (var w in script.GetAllWeapons())
                {
                    if (name != w.name) continue;
                    w.GetComponent<Weapon>().ChangeAmmoQty((int)lootValue);
                    break;
                }
                break;
            
            case LootType.Health:
                script.ChangeHealth(
                    healthValue: (int) lootValue,
                    damaged: false);
                break;
            
            case LootType.Shield:
                var shield = script.GetPlayerShield();
                shield.gameObject.SetActive(true);
                shield.Activate(lootValue);
                break;
        }
    }

    private void DestroyLoot()
    {
        Destroy(gameObject);
    }
}
