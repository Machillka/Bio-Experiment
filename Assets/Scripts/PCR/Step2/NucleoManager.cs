using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NucleoManager : MonoBehaviour
{
    public static NucleoManager Instance { get; private set; }

    public string targetSequence = "";          // 目标序列,使用inspector编辑
    public List<Slot> targetSlots = new();
    public Transform topLayerParent;    // TopLayer 父物体
    public Transform bottomLayerParent; // BottomLayer 父物体
    private void Awake()
    {
        Instance = this;
        AutoFindAndSortSlots();
        AutoLinkSlots();
    }

    public void CheckSequence()
    {
        if (string.IsNullOrEmpty(targetSequence) || targetSlots.Count == 0) return;

        string currentSequence = "";

        foreach (var slot in targetSlots)
        {
            if (slot.IsEmpty)
            {
                Debug.Log("序列未完成...");
                return;
            }
            currentSequence += slot.currentData.type.ToString();
        }

        Debug.Log($"当前序列: {currentSequence} | 目标: {targetSequence}");

        if (currentSequence == targetSequence)
        {
            OnStepComplete();
        }
        else
        {
            Debug.LogWarning("序列错误！请检查配对。");
        }
    }

    private void OnStepComplete()
    {
        Debug.Log("<color=green>PCR 引物设计成功！进入下一阶段...</color>");
        // TODO: 下一个步骤
    }

    [ContextMenu("AutoFind And Sort Slots")]
    private void AutoFindAndSortSlots()
    {
        Slot[] allSlots = topLayerParent.GetComponentsInChildren<Slot>();

        // 按照 x 轴排序, 逻辑上 5' -> 3'
        targetSlots = allSlots.OrderBy(s => s.transform.position.x).ToList();
    }

    [ContextMenu("Auto Link Complement Slots")] // 右键菜单执行
    public void AutoLinkSlots()
    {
        if (topLayerParent == null || bottomLayerParent == null)
        {
            Debug.LogError("请先赋值 TopLayer 和 BottomLayer 的父物体！");
            return;
        }

        // 获取所有子 Slot
        var topSlots = topLayerParent.GetComponentsInChildren<Slot>();
        var bottomSlots = bottomLayerParent.GetComponentsInChildren<Slot>();

        // 确保数量一致
        int count = Mathf.Min(topSlots.Length, bottomSlots.Length);

        for (int i = 0; i < count; i++)
        {
            // 互相绑定
            topSlots[i].complementSlot = bottomSlots[i];
            bottomSlots[i].complementSlot = topSlots[i];

            // 顺便改个名方便看
            topSlots[i].name = $"Slot_Top_{i}";
            bottomSlots[i].name = $"Slot_Bottom_{i}";
        }

        Debug.Log($"成功自动双向绑定了 {count} 对 Slot！");
    }
}
