using System.Collections;
using UnityEngine;

public class RoomTransitionManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _player;
    [SerializeField] private float _slideDuration = 0.4f;

    private bool _isTransitioning;

    private void OnEnable()
    {
        GameEvents.OnRoomTransition += StartTransition;
    }

    private void OnDisable()
    {
        GameEvents.OnRoomTransition -= StartTransition;
    }

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
        _player.position = spawnPoint.position;

        GameEvents.OnTransitionEnd?.Invoke();

        _isTransitioning = false;
    }
}
