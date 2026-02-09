using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private static StateManager _instance;

    public static StateManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new StateManager();
            return _instance;
        }
    }

    public ExperimentStepSO currentExpetimentStep;
    public List<ConditionSO> currentConditions;
    public int conditionIndex;

    private StateManager()
    {
        currentConditions = new();
        conditionIndex = 0;
    }

    public void TransitionToNextStep()
    {
        currentExpetimentStep = currentExpetimentStep.nextStep;

        currentConditions = currentExpetimentStep.conditions;
        conditionIndex = 0;
    }
}
