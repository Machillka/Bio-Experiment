using System.Collections.Generic;
using Bio.Interaction;
using PCR;
using PCR.Core;
using UnityEngine;
using UnityEngine.UI;

public class Step3Manager : StepBase
{
    public List<EquipmentData> needMoveGameobjects;
    public Button submitButton;             // 提交开始灭菌的 button

    private void Awake()
    {
        SetCurrentSceneActive(false);
    }

    private void OnEnable()
    {
        submitButton.onClick.AddListener(OnButtonClicled);
        PCREventBus.OnDragableDrop += OnObjectDrop;
    }

    private void OnDisable()
    {
        submitButton.onClick.RemoveListener(OnButtonClicled);
        PCREventBus.OnDragableDrop -= OnObjectDrop;
    }

    /// <summary>
    /// 在物体被放置到 drop zone 的时候 做一个简单的事件通知
    /// </summary>
    public void OnObjectDrop(EquipmentData id)
    {
        if (needMoveGameobjects.Contains(id))
            needMoveGameobjects.Remove(id);
    }

    public override void OnEnter()
    {
        if (needMoveGameobjects.Count == 0)
        {
            Debug.LogWarning("No GO in list");
        }
        SetCurrentSceneActive(true);
    }

    public override void OnExit()
    {
        SetCurrentSceneActive(false);
        Debug.Log("<color=green> Step 3 Completed successfully!");
        PCRManager.Instance.NextStep();
    }

    /// <summary>
    /// 检查是否全部都拖拽结束
    /// </summary>
    /// <returns>
    /// true: 合法
    /// false: 非法
    /// </returns>
    public bool CheckDragableDone => needMoveGameobjects.Count == 0;

    public void OnButtonClicled()
    {

        if (CheckDragableDone)
        {
            // TODO: 添加 UI 变化
            OnExit();
        }
        else
        {
            Debug.LogWarning("Still something forgot");
        }
    }

}
