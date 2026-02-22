using UnityEngine;

/// <summary>
/// 纯数据 相当于 vlaue
/// </summary>
[CreateAssetMenu(fileName = "ReagentData", menuName = "PCR/ReagentData")]
public class ReagentDataSO : ScriptableObject
{
    public string id;
    public string displayName;  // 显示名称
    public Color liquidColor;   // 液体颜色
}
