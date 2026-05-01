using UnityEngine;

public interface ICollectible
{
    Sprite Sprite { get; }

    void Init();
    ICollectible Collect(GameObject collector); // TODO: 나중에 플레이어로 교체 예정
}