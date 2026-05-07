using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHp = 6;
    [SerializeField] private float _invincibilityDuration = 2f;
    [SerializeField] private float _blinkInterval = 0.1f;

    private int _currentHp;
    private bool _isProtected = false;
    private float _invincibilityTimer;
    private PlayerAnimator playerAnimator;
    private Renderer[] _renderers;

    public int MaxHp => _maxHp;
    public int CurrentHp => _currentHp;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        SetMaxHp(_maxHp, true);
    }

    private void Update()
    {
        if (_invincibilityTimer > 0f)
            _invincibilityTimer -= Time.deltaTime;
    }

    public void SetMaxHp(int newMax, bool healFull = false)
    {
        _maxHp = newMax;
        GameEvents.OnPlayerMaxHpChanged?.Invoke(_maxHp);

        if (healFull)
        {
            _currentHp = _maxHp;
            GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);
        }
    }

    public void AddMaxHp(int amount, bool heal = true)
    {
        _maxHp += amount;
        GameEvents.OnPlayerMaxHpChanged?.Invoke(_maxHp);

        if (heal)
        {
            _currentHp += amount;
            GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);
        }
    }

    public void TakeDamage(int amount)
    {
        if (_invincibilityTimer > 0f) return;

        if (_isProtected)
        {
            _isProtected = false;
        }
        else
        {
            _currentHp -= amount;

            if (_currentHp <= 0)
            {
                _currentHp = 0;
                GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);
                playerAnimator.PlayDie();
                Die();
                return;
            }

            playerAnimator.PlayDamage();
            GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);
        }

        _invincibilityTimer = _invincibilityDuration;
        StartCoroutine(BlinkCoroutine());
        GameEvents.OnPlayerHit?.Invoke();
    }

    private IEnumerator BlinkCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < _invincibilityDuration)
        {
            SetRenderersVisible(false);
            yield return new WaitForSeconds(_blinkInterval);
            SetRenderersVisible(true);
            yield return new WaitForSeconds(_blinkInterval);
            elapsed += _blinkInterval * 2f;
        }
        SetRenderersVisible(true);
    }

    private void SetRenderersVisible(bool visible)
    {
        foreach (var r in _renderers)
            if (r != null) r.enabled = visible;
    }

    public bool OnHeal(int amount)
    {
        if (_currentHp >= _maxHp) return false;

        _currentHp = Mathf.Min(_currentHp + amount, _maxHp);
        GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);

        return true;
    }

    private void Die()
    {
        GameEvents.OnPlayerDie?.Invoke();
    }

    public bool SetProtected(bool isProtected)
    {
        if (_isProtected == isProtected) return false;

        _isProtected = isProtected;
        return true;
    }
}
