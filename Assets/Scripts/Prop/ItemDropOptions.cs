using UnityEngine;

public class ItemDropOptions : MonoBehaviour
{
    [SerializeField] private GameObject[] _itemPrefabs;
    [SerializeField][Range(0.1f, 1f)] private float _probability = 0.3f;

    private void OnDestroy()
    {
        if (Random.value < _probability)
        {
            Instantiate(
                _itemPrefabs[Random.Range(0, _itemPrefabs.Length)],
                transform.position,
                Quaternion.identity);
        }
    }
}