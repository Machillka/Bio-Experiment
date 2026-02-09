using System;
using UnityEngine;

/// <summary>
/// 在每一次操作之后进行广播check
/// </summary>
public static class ValidEventBus
{
    // WORKFLOW: 对于每一种 Action 实现对应事件
    public static Action<GameObject, GameObject> OnSlotPlace;
    public static Action<GameObject, GameObject> OnLiquidAdd;
}