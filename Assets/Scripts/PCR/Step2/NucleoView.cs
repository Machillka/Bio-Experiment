using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 视觉组件
/// </summary>
public class NucleoView : MonoBehaviour
{
    [Header("UI References")]
    public Image bgImage;
    public TextMeshProUGUI symbolText;
    public CanvasGroup canvasGroup;         // 用于控制射线阻挡

    public void Awake()
    {
        if (TryGetComponent<NucleoSource>(out var source))
        {
            Setup(new NucleoData(source.type));
        }
    }

    // 初始化视图的方法
    public void Setup(NucleoData data)
    {
        if (data == null) return;

        symbolText.text = data.type.ToString();

        symbolText.color = data.GetColor();

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    // 拖拽时的特殊视觉状
    public void SetDraggingState(bool isDragging)
    {
        SetRaycastState(!isDragging); // 拖拽时不阻挡射线
        canvasGroup.alpha = isDragging ? 0.6f : 1f;
    }

    public void SetRaycastState(bool canBlock)
    {
        canvasGroup.blocksRaycasts = canBlock;
    }
}