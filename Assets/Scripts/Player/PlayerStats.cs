using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Player의 Stat를 관리하는 컴포넌트<br/>
/// Modifier로 변동되는 Stats 정의
/// </summary>
[RequireComponent(typeof(PlayerActions))]
public class PlayerStats : MonoBehaviour
{
    private const float k_fps = 60f; // TODO: 나중에 프레임 관련한 스크립트로 이동

    [SerializeField] private StatData _base;
    [SerializeField] private float _MaxSpeed = 10f;

    private List<Modifier> _modifiers = new();

    public float Damage => CalcStat(PlayerStatType.Damage, _base.Damage);
    public float Delay => CalcStat(PlayerStatType.Delay, _base.Delay) / k_fps;
    public float Range => CalcStat(PlayerStatType.Range, _base.Range) / k_fps;
    public float ShotSpeed => CalcStat(PlayerStatType.ShotSpeed, _base.ShotSpeed);
    public float MoveSpeed => Mathf.Min(CalcStat(PlayerStatType.Speed, _base.Speed), _MaxSpeed);
    public float Luck => CalcStat(PlayerStatType.Luck, _base.Luck);

    private float CalcStat(PlayerStatType type, float baseValue)
    {
        var relevant = _modifiers.Where(m => m.StatType == type);
        float add = relevant.Where(m => m.Type == ModifierType.Addtive).Sum(m => m.Value);
        float mul = relevant.Where(m => m.Type == ModifierType.Multiplicative).Aggregate(1f, (acc, m) => acc * m.Value);
        return (baseValue + add) * mul;
    }

    public void AddModifier(Modifier modifier) => _modifiers.Add(modifier);

    public void RemoveModifiersFromSource(object source)
    {
        _modifiers.RemoveAll(m => m.Source == source);
    }
}