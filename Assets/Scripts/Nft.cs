public class Nft
{
    public enum NftType
    {
        Armor, // add 2 slots
        Gun,
        Shotgun,
        Smg,
        Explosive,
        Module,
        Ability
    }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public float Value { get; set; }
    public string ThumbnailUri { get; set; }
    public NftType Type { get; set; }
}
