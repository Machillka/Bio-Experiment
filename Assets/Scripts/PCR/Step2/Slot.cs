using System.Collections;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [Header("Config")]
    public bool isTopLayer;
    public Slot complementSlot; // 对应的互补槽

    [Header("Runtime State")]
    public NucleoData currentData = null;           // 存储自身被填充的碱基数据
    public bool IsEmpty => currentData == null;

    /// <summary>
    /// 接受拖拽数据到当前 slot, 并且播放一些简易的动画
    /// </summary>
    /// <param name="data"></param>
    /// <param name="startGlobalPos"></param>
    /// <param name="isAutoFill"></param>
    public void AcceptData(NucleoData data, Vector3 startPos, bool isAutoFill = false)
    {
        currentData = data;

        // 视觉处理
        CreateVisuals(data, startPos);

        // 允许覆盖数据 但是强制符合碱基互补配对原则
        if (!isAutoFill && complementSlot != null)
        {
            var complementData = new NucleoData(data.GetComplement());
            // 从上方或者下方滑入
            Vector3 offsetPos = complementSlot.transform.position + (isTopLayer ? Vector3.down : Vector3.up) * 50f;
            complementSlot.AcceptData(complementData, offsetPos, true);
        }

        NucleoManager.Instance.CheckSequence();
    }

    private void CreateVisuals(NucleoData data, Vector3 startPos)
    {
        var prefab = DragController.Instance.nucleoPrefab;
        var obj = Instantiate(prefab, transform);
        // obj.transform.localPosition = Vector3.zero;
        obj.transform.position = startPos;
        StartCoroutine(SmoothSnap(obj.transform, Vector3.zero, 0.2f));


        // 初始化 View
        var view = obj.GetComponent<NucleoView>();
        view.Setup(data);
        view.SetRaycastState(false);        // 不阻挡射线,可以覆盖    
        // view.SetDraggingState(false); // 确保实心，阻挡射线
    }

    private IEnumerator SmoothSnap(Transform target, Vector3 localDest, float duration)
    {
        float elapsed = 0f;
        Vector3 startingLocalPos = target.localPosition;

        while (elapsed < duration)
        {
            // 使用 Lerp 实现平滑移动
            target.localPosition = Vector3.Lerp(startingLocalPos, localDest, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localPosition = localDest; // 确保最终对齐到 pivot (0,0,0)
    }
}