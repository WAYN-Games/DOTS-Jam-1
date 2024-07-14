using NaughtyAttributes;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Required]
    public Canvas canvas;


    public void ShowUI(bool show)
    {
        if (canvas == null) return;
        canvas.enabled = show;
    }

}
