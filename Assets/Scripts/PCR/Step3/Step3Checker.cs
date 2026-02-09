using System.Collections.Generic;
using Bio.Helper;
using UnityEngine;

public class Step3Checker : MonoBehaviour
{
    public HashSet<string> NeededInstrumemts = new() { "蒸馏水", "微量离心管", "枪头" };
    private Pot _pot = new();
    private bool _isSuccess = false;


    private void OnDropped(ExperimentObject from, ExperimentObject to)
    {
        if (NeededInstrumemts.Contains(from.Name))
        {
            NeededInstrumemts.Remove(from.Name);
        }
        // TODO 
    }

    private void OnButtonClick()
    {
        if (NeededInstrumemts.Count == 0)
            _isSuccess = true;
    }

    private void OnEnable()
    {
        // TODO: 订阅事件
    }

    private void OnDisable()
    {
        // TODO: 取消订阅事件
    }

    private void Update()
    {
        if (_isSuccess)
        {
            // 转化到下一个步骤
        }
    }
}
