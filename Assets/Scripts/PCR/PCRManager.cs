namespace PCR
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class PCRManager : MonoBehaviour
    {
        private static PCRManager _instance;
        public static PCRManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<PCRManager>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("SimpleSingleton");
                        _instance = obj.AddComponent<PCRManager>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }

        [Header("Experiment Data")]
        public ExperimentContext Data = new();

        [Header("Step")]
        [SerializeField] private List<StepBase> allSteps = new();

        // 通过下标进行移动
        private int _currentStepIndex = -1;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // 初始化流程
            InitializeExperiment();
        }

        private void InitializeExperiment()
        {
            Data.Clear();

            // 3. 进入第一步
            if (allSteps.Count > 0)
            {
                _currentStepIndex = 0;
                EnterStep(_currentStepIndex);
            }
            else
            {
                Debug.LogWarning("PCRManager: 没有配置任何步骤 (Steps) !");
            }
        }

        public void NextStep()
        {
            int nextIndex = _currentStepIndex + 1;
            if (nextIndex < allSteps.Count)
            {
                _currentStepIndex = nextIndex;
                EnterStep(_currentStepIndex);
            }
            else
            {
                FinishExperiment();
            }
        }

        private void FinishExperiment()
        {
            // 可以弹出一个结算动画（
        }

        private void EnterStep(int idx)
        {
            var step = allSteps[idx];
            if (step != null)
            {
                Debug.Log($"PCR Flow: Entering Step {idx} - {step.name}");
                step.OnEnter();                // 触发 Step 的 OnEnter
            }
        }
    }

}