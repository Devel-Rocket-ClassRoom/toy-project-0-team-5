using UnityEngine;

public class BgmManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _normalBgm;
    [SerializeField] private AudioClip _shopBgm;
    [SerializeField] private AudioClip _bossBgm;

    private void OnEnable()
    {
        GameEvents.OnNormalRoomEnter += PlayNormal;
        GameEvents.OnShopRoomEnter   += PlayShop;
        GameEvents.OnBossRoomEnter   += PlayBoss;
    }

    private void OnDisable()
    {
        GameEvents.OnNormalRoomEnter -= PlayNormal;
        GameEvents.OnShopRoomEnter   -= PlayShop;
        GameEvents.OnBossRoomEnter   -= PlayBoss;
    }

    private void PlayNormal()           => Play(_normalBgm);
    private void PlayShop()             => Play(_shopBgm);
    private void PlayBoss(EnemyBase _)  => Play(_bossBgm);

    private void Play(AudioClip clip)
    {
        if (clip == null || _audioSource.clip == clip) return;
        _audioSource.clip = clip;
        _audioSource.loop = true;
        _audioSource.Play();
    }
}
