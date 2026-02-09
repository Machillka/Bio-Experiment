using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 左上角提供的拖拽对象
/// </summary>
public class NucleoSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data")]
    public NucleoBaseTypes type;        // 在 Inspector 里设置碱基类型

    void Start()
    {
        Debug.Log($"fuck {type}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"[Drag Start] Object Name: {gameObject.name}, Type in Script: {type}");
        var data = new NucleoData(type);
        DragController.Instance.StartDragging(data, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragController.Instance.UpdateDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragController.Instance.EndDrag(eventData);
    }
}