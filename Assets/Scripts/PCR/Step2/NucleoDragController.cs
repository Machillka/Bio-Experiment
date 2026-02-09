using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour
{
    public static DragController Instance;

    [Header("Settings")]
    public GameObject nucleoPrefab;
    public Transform dragLayer;     // Canvas 下的一个最上层 Panel，防止遮挡

    private GameObject _currentDragObj;
    private NucleoView _currentDragView;
    private NucleoData _currentData;

    private void Awake() => Instance = this;

    // 1. 开始：生成临时的视觉替身
    public void StartDragging(NucleoData data, Vector2 pos)
    {
        _currentData = data;

        // 生成
        _currentDragObj = Instantiate(nucleoPrefab, dragLayer);
        _currentDragObj.transform.position = pos;

        // 初始化视觉
        _currentDragView = _currentDragObj.GetComponent<NucleoView>();
        _currentDragView.Setup(data);
        _currentDragView.SetDraggingState(true); // 变透明，穿透射线
    }

    // 2. 过程：跟随鼠标
    public void UpdateDrag(Vector2 pos)
    {
        if (_currentDragObj != null)
            _currentDragObj.transform.position = pos;
    }

    // 3. 结束：判断落点
    public void EndDrag(PointerEventData eventData)
    {
        GameObject target = eventData.pointerEnter;

        // 检查是否落在 Slot 上
        if (target != null && target.TryGetComponent<Slot>(out var slot))
        {
            // 不检查空 允许重做
            // 把数据交给 Slot
            slot.AcceptData(_currentData, _currentDragObj.transform.position);

            // 销毁拖拽用的临时替身 (Slot 会自己生成一个新的固定物体)
            Destroy(_currentDragObj);
            return;
        }

        // 失败：直接销毁
        Destroy(_currentDragObj);
        _currentDragObj = null;
        _currentData = null;
    }
}