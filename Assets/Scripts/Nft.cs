using System.Collections.Generic;
using System.Text.Json.Serialization;
using Helpers;

[JsonConverter(typeof(NftConverter))]
public class Nft
{
    public enum NftType
    {
        None,
        Armor,
        Gun,
        Shotgun,
        Smg,
        Explosive,
        Module,
        Ability
    }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public string ThumbnailUri { get; set; }
    public NftType Type { get; set; }
    public List<GameParameters> GameParameters { get; set; }
}


public class GameParameters
{
    public enum Type
    {
        Unit,
        Percent
    }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public float Value { get; set; }
    public Type MeasureType { get; set; }
}
