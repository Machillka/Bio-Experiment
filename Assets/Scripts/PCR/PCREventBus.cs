using System;
using UnityEngine;
namespace PCR.Core
{
    public static class PCREventBus
    {
        // 液体变化事件 (View层监听此事件更新UI/液面)
        public static event Action<string, float, Color> OnLiquidUpdated;
        public static void PublishLiquidUpdate(string containerID, float totalVol, Color mixColor)
        {
            OnLiquidUpdated?.Invoke(containerID, totalVol, mixColor);
        }
        // 步骤验证事件 (Manager监听此事件判断进度)
        public static event Action<bool, string> OnValidationResult;
        public static void PublishValidation(bool passed, string msg)
        {
            OnValidationResult?.Invoke(passed, msg);
        }
    }
}


