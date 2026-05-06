public enum ModifierType
{
    Additive,
    Multiplicative
}

public class Modifier
{
    public PlayerStatType StatType { get; }
    public ModifierType Type { get; }
    public float Value { get; }
    public object Source { get; }

    public Modifier(PlayerStatType statType, ModifierType modType, float value, object source)
    {
        StatType = statType;
        Type = modType;
        Value = value;
        Source = source;
    }
}