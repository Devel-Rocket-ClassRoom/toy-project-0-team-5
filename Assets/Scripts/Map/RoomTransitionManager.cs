using System.Collections;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _player;
    [SerializeField] private float _slideDuration = 0.4f;
    [SerializeField] private float _triggerCooldown = 0.5f;

    private bool _isTransitioning;
    private Rigidbody _playerRb;
    private Collider _playerCollider;

    private void Awake()
    {
        if (_player != null)
        {
            _playerRb = _player.GetComponent<Rigidbody>();
            _playerCollider = _player.GetComponent<Collider>();
        }
    }

    private void OnEnable() => GameEvents.OnRoomTransition += StartTransition;
    private void OnDisable() => GameEvents.OnRoomTransition -= StartTransition;

    private void StartTransition(Transform spawnPoint)
    {
        if (_isTransitioning) return;
        StartCoroutine(TransitionRoutine(spawnPoint));
    }

    private IEnumerator TransitionRoutine(Transform spawnPoint)
    {
        _isTransitioning = true;
        GameEvents.OnTransitionStart?.Invoke();

        Vector3 cameraOffset = _camera.transform.position - _player.position;
        Vector3 targetCameraPos = spawnPoint.position + cameraOffset;
        Vector3 fromPos = _camera.transform.position;
        float elapsed = 0f;

        while (elapsed < _slideDuration)
        {
            elapsed += Time.deltaTime;
            _camera.transform.position = Vector3.Lerp(fromPos, targetCameraPos, elapsed / _slideDuration);
            yield return null;
        }

        _camera.transform.position = targetCameraPos;

        if (_playerCollider != null) _playerCollider.enabled = false;

        if (_playerRb != null)
        {
            _playerRb.linearVelocity = Vector3.zero;
            _playerRb.position = spawnPoint.position;
        }
        else
        {
            _player.position = spawnPoint.position;
        }

        yield return new WaitForFixedUpdate();

        if (_playerCollider != null) _playerCollider.enabled = true;

        GameEvents.OnTransitionEnd?.Invoke();

        // 텔레포트 직후 문 트리거가 즉시 발동하는 것을 방지
        yield return new WaitForSeconds(_triggerCooldown);
        _isTransitioning = false;
    }
}
