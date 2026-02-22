using System;
using System.Collections.Generic;
using PCR;
using PCR.Core;
using TMPro;
using UnityEngine;

public class Step4Manager : StepBase
{
    [Header("Validation Data")]
    [SerializeField] private RecipeDataSO _targetRecipe;
    [SerializeField] private LiquidContainer _destinationTube; // 学生操作的目标管

    [Header("UI")]
    public TextMeshProUGUI statusText;

    // 监听事件
    private void OnEnable()
    {
        PCREventBus.OnLiquidUpdated += CheckProgress;
    }

    private void OnDisable()
    {
        PCREventBus.OnLiquidUpdated -= CheckProgress;
    }

    public override void OnEnter()
    {
        Debug.Log("Step 4 Started: PCR System Preparation");
        // _destinationTube.Clear();
    }

    public override void OnExit()
    {
        // 进入下一步
        PCRManager.Instance.NextStep();
        Debug.Log("Step 4 Completed");
    }


    private void CheckProgress(string containerID, float vol, Color col)
    {
        if (containerID != _destinationTube.ID)
            return;

        bool isCorrect = _targetRecipe.Validate(_destinationTube.GetContentsSnapshot(), out string message);

        // 更新 UI
        if (statusText)
            statusText.text = message;

        if (isCorrect)
        {
            Debug.Log("<color=green>SUCCESS: PCR System Prepared Perfectly!</color>");
            PCREventBus.PublishValidation(true, "Preparation Complete");
            // 步骤结束
            OnExit();

        }
        else
        {
            Debug.Log($"Progress: {message}");
        }
    }
}
