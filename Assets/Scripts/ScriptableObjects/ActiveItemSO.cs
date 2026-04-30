using UnityEngine;

[CreateAssetMenu(menuName = "ActiveItemSO", order = 0)]
public class ActiveItemSO : ScriptableObject
{
    public ChargeType ChargeType;
    public int MaxCharge;
}