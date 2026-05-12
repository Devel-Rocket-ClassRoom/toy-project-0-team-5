using UnityEngine;
using UnityEngine.UI;

public class UI_PassiveItemSlot : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;

    public void Init(Sprite icon)
    {
        iconImage.sprite = icon;
    }
}
