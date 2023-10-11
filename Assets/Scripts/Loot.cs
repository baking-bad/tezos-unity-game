using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    private enum LootType
    {
        Bullets,
        Weapon,
        Health,
        Armor
    }
    
    [SerializeField] private float destructionTime;
    [SerializeField] private LootType type;
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DestroyLoot), destructionTime);
    }

    public void ApplyLoot()
    {
        switch (type)
        {
            case LootType.Weapon:
                break;
            case LootType.Bullets:
                break;
            case LootType.Health:
                break;
            case LootType.Armor:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DestroyLoot()
    {
        Destroy(gameObject);
    }
}
