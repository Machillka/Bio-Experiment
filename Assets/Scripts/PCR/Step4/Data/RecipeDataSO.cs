using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 配置所有需要添加的试剂
/// </summary>
[CreateAssetMenu(fileName = "RecipeData", menuName = "PCR/RecipeData")]
public class RecipeDataSO : ScriptableObject
{
    public List<Ingredient> ingredients;
    public float totalTargetVolume = 50f;
    [Tooltip("总误差容许范围")]
    public float totalTolerance;

    /// <summary>
    /// 验证是否符合条件
    /// </summary>
    /// <param name="currentContents"></param>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    // TOOD: 封装成一个枚举类型的错误信息；或者 error context
    public bool Validate(Dictionary<ReagentDataSO, float> currentContents, out string errorMsg)
    {
        float currentTotal = 0;
        foreach (var val in currentContents.Values) currentTotal += val;

        if (Mathf.Abs(currentTotal - totalTargetVolume) > totalTolerance)
        {
            errorMsg = $"总量错误: 当前 {currentTotal:F1}uL / 目标 {totalTargetVolume}uL";
            return false;
        }

        foreach (var item in ingredients)
        {
            if (!currentContents.ContainsKey(item.reagent))
            {
                // 其实只是检查了一种缺少的成分
                errorMsg = $"缺少成分: {item.reagent.displayName}";
                return false;
            }

            float amount = currentContents[item.reagent];
            if (Mathf.Abs(amount - item.targetVolume) > item.tolerance)
            {
                errorMsg = $"{item.reagent.displayName} 量错误: {amount:F1}uL (目标: {item.targetVolume})";
                return false;
            }
        }

        errorMsg = "";
        return true;

    }

}

/// <summary>
/// 单种成分
/// </summary>
[Serializable]
public class Ingredient
{
    public ReagentDataSO reagent;
    public float targetVolume; // uL
    public float tolerance = 0.1f;      // 容错范围 (+/- uL)
}