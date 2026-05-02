using UnityEngine;
using UnityEngine.UI;

public class UI_HeartSlot : MonoBehaviour
{
    [SerializeField] private Image heartImage;
    [SerializeField] private Sprite full;
    [SerializeField] private Sprite half;
    [SerializeField] private Sprite empty;

    public void SetState(int value)
    {
        switch (value)
        {
            case 0: heartImage.sprite = full; break;
            case 1: heartImage.sprite = half; break;
            case 2: heartImage.sprite = empty; break;
        }
    }
}
