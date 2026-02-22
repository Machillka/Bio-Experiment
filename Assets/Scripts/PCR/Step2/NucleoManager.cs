using System.Collections.Generic;
using System.Linq;
using PCR;
using UnityEngine;

public class NucleoManager : StepBase
{
    public static NucleoManager Instance { get; private set; }

    private string targetSequence = "";          // 目标序列, 通过黑板传递
    public List<Slot> targetSlots = new();
    public Transform topLayerParent;    // TopLayer 父物体
    public Transform bottomLayerParent; // BottomLayer 父物体
    public GameObject step2GO;

    private void Awake()
    {
        Instance = this;
        AutoFindAndSortSlots();
        AutoLinkSlots();
        step2GO.SetActive(false);
    }

    /// <summary>
    /// 在每一次操作的时候 Check 然后弹出对应的 hint
    /// </summary>
    public void CheckSequence()
    {
        if (string.IsNullOrEmpty(targetSequence) || targetSlots.Count == 0) return;

        string currentSequence = "";

        // TODO: 做一个 Hint UI
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
            OnExit();
        }
        else
        {
            Debug.LogWarning("序列错误！请检查配对。");
        }
    }

    public override void OnExit()
    {
        Debug.Log("<color=green>PCR 引物设计成功！进入下一阶段...</color>");
        // TODO: 处理引物序列信息
        PCRManager.Instance.NextStep();
    }

    public override void OnEnter()
    {
        Debug.Log("Step2 引物设计 enter");

        step2GO.SetActive(true);

        if (!string.IsNullOrEmpty(Data.TargetDnaSequence))
        {
            // FIXME: 实际上 target sequence 需要截取部分进行显示; 而且引物也不是直接这样得到的
            // FIXME: 缺失 显示 DNA 序列逻辑, 缺失截取逻辑
            targetSequence = Data.TargetDnaSequence;
        }
    }

    #region Slot Operation
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

        Debug.Log($"成功自动双向绑定了 {count} 对 Slot");
    }
    #endregion
}
