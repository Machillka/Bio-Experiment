using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperimentStepSO", menuName = "Bio-Lab/ExperimentStepSO")]
public class ExperimentStepSO : ScriptableObject
{
    public string stepName;
    [TextArea]
    public string description;
    public ExperimentStepSO nextStep;
    public float totalTime; // 执行该步骤使用了多长时间
    public List<ConditionSO> conditions;
}