using UnityEngine;
using System.Collections;

public class NPC_Animator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float idleInterval = 7f;

    private readonly string[] idleTriggers =
    {
        "Idle2",
        "Idle3",
        "Idle4",
        "Idle5"
    };

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        StartCoroutine(CoRandomIdle());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator CoRandomIdle()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleInterval);

            int index = Random.Range(0, idleTriggers.Length);
            animator.SetTrigger(idleTriggers[index]);
        }
    }
}
