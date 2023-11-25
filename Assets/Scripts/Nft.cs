using NftType = Managers.UserDataManager.NftType;
public class Nft
{
    public string Name { get; set; }
    public string Description { get; set; }
    public float Value { get; set; }
    public string ThumbnailUri { get; set; }
    public NftType Type { get; set; }
}
