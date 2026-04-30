using UnityEngine;

public enum ChargeType
{
    OnRoomClear,
    OnHit,
}

public abstract class ActiveItemBase : IUseable
{
    private int _id;
    protected ChargeType _chargeType;
    protected int _currentCharge;
    protected int _maxCharge;

    public ActiveItemBase(int id, ChargeType chargeType, int maxCharge)
    {
        _id = id;
        _chargeType = chargeType;
        _maxCharge = maxCharge;
    }

    public abstract void Use();

    public void Init()
    {
        _currentCharge = 0;
    }

    public void Using()
    {
        switch (_chargeType)
        {
            case ChargeType.OnRoomClear:
                // TODO: GameEvents.OnRoomClear += OnCharge;
                break;
            case ChargeType.OnHit:
                // TODO: GameEvents.OnPlayerHit += OnCharge;
                break;
        }
    }

    public void Dispose()
    {
        switch (_chargeType)
        {
            case ChargeType.OnRoomClear:
                // TODO: GameEvents.OnRoomClear -= OnCharge;
                break;
            case ChargeType.OnHit:
                // TODO: GameEvents.OnPlayerHit -= OnCharge;
                break;
        }
    }

    private void OnCharge()
    {
        _currentCharge = Mathf.Min(_currentCharge + 1, _maxCharge);
    }
}

/// <summary>
/// 아이템 예시
/// </summary>
public class Dice : ActiveItemBase
{
    public Dice(int id, ChargeType chargeType, int maxCharge) : base(id, chargeType, maxCharge) { }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }
}