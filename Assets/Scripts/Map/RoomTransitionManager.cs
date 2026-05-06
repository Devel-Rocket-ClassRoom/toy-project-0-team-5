using System.Collections;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _player;
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

    private void StartTransition(Transform spawnPoint, Transform nextRoom)
    {
        if (_isTransitioning) return;
        StartCoroutine(TransitionRoutine(spawnPoint, nextRoom));
    }

    private IEnumerator TransitionRoutine(Transform spawnPoint, Transform nextRoom)
    {
        _isTransitioning = true;
        _player.GetComponent<PlayerMovement>().enabled = false;
        GameEvents.OnTransitionStart?.Invoke();

        Vector3 targetCameraPos = new(nextRoom.position.x, _camera.transform.position.y, nextRoom.position.z - 15f);
        Vector3 fromPos = _camera.transform.position;
        float elapsed = 0f;

        while (elapsed < _slideDuration)
        {
            elapsed += Time.deltaTime;
            _camera.transform.position = Vector3.Lerp(fromPos, targetCameraPos, elapsed / _slideDuration);
            yield return null;
        }

        _camera.transform.position = targetCameraPos;
        Debug.Log($"Camera slide completed: {_camera.transform.position}");

        if (_playerCollider != null) _playerCollider.enabled = false;

        if (_playerRb != null)
        {
            _playerRb.linearVelocity = Vector3.zero;
            _player.transform.position = spawnPoint.position;
        }
        else
        {
            _player.transform.position = spawnPoint.position;
        }
        Debug.Log($"Player moved to: {_player.transform.position}");

        yield return new WaitForFixedUpdate();

        if (_playerCollider != null) _playerCollider.enabled = true;
        _player.GetComponent<PlayerMovement>().enabled = true;
        GameEvents.OnTransitionEnd?.Invoke();

        // 텔레포트 직후 문 트리거가 즉시 발동하는 것을 방지
        yield return new WaitForSeconds(_triggerCooldown);
        _isTransitioning = false;
    }
}
