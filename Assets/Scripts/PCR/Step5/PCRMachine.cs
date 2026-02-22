using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

namespace PCR
{
    public class PCRMachine : MonoBehaviour
    {
        [Header("Configuration")]
        public float heatRate = 4.0f; // 升温速度 (度/秒)
        public float coolRate = 3.0f; // 降温速度 (度/秒)
        public float timeScale = 1.0f; // 时间倍率 (1.0 = 实时, 5.0 = 5倍速)

        [Header("Correct Protocol (The Answer)")]
        public float targetAnnealingTemp = 55.0f;
        public float allowableError = 2.0f; // 允许误差 ±2度

        [Header("UI References (World Space)")]
        public TextMeshProUGUI statusText;   // 显示当前状态 (e.g. "Denaturation")
        public TextMeshProUGUI tempText;     // 显示大号温度 (e.g. "94.0°C")
        public TextMeshProUGUI cycleText;    // 显示循环数 (e.g. "Cycle 5/30")
        public TextMeshProUGUI timerText;    // 显示倒计时
        // TODO: 夹带私货 放置一个跑动的小人协程
        public Slider progressSlider;        // 总体进度条
        public Button startButton;

        // 内部状态
        private float _currentBlockTemp = 25.0f; // 当前温度
        private bool _isRunning = false;
        private LiquidContainer _loadedTube = new();     // 当前放入的试管

        // 用户当前 Machine 设置
        // TODO: (通过UI修改这个对象)
        public PCRProtocol userProtocol;

        // 进度条计算变量
        private float _experimentStartTime;
        private float _estimatedTotalDuration;

        private void Start()
        {
            UpdateScreenDisplay("Ready", 0, 0);
            startButton.onClick.AddListener(StartPCR);
        }

        private void Update()
        {
            if (_isRunning && progressSlider != null && _estimatedTotalDuration > 0)
            {
                float timePassed = Time.time - _experimentStartTime;
                float progress = Mathf.Clamp01(timePassed / _estimatedTotalDuration);
                progressSlider.value = progress;
            }
        }


        // 外部调用：放入试管
        public void LoadTube(LiquidContainer tube)
        {
            _loadedTube = tube;
            // NOTE: 是否有放入试管的步骤
        }

        [ContextMenu("Start PCR")]
        public void StartPCR()
        {
            Debug.Log("Button Clicked");
            if (_isRunning) return;
            // if (_loadedTube == null)
            // {
            statusText.text = "No Tube!";
            // return;
            // }
            // NOTE: runtime 修改 timescale 不会触发总时间的改变, 导致进度条不满
            _estimatedTotalDuration = CalculateTotalEstimatedTime();
            _experimentStartTime = Time.time;
            StartCoroutine(RunProtocolRoutine());
        }

        private float CalculateTotalEstimatedTime()
        {
            float total = 0f;
            total += userProtocol.initialDenaturation.duration;
            total += (userProtocol.denaturation.duration +
                      userProtocol.annealing.duration +
                      userProtocol.extension.duration) * userProtocol.cycles;
            total += userProtocol.finalExtension.duration;

            float rampTimePerSwitch = 2.0f;
            int totalSwitches = 1 + (userProtocol.cycles * 3) + 1; // 初始 + 循环次数*3 + 结尾

            total += totalSwitches * rampTimePerSwitch;
            return total / timeScale;
        }

        private IEnumerator RunProtocolRoutine()
        {
            _isRunning = true;
            startButton.interactable = false;

            // 变性
            yield return RunStep("Init. Denaturation", userProtocol.initialDenaturation);

            // 循环
            for (int i = 0; i < userProtocol.cycles; i++)
            {
                UpdateScreenDisplay("Denaturation", i + 1, userProtocol.cycles);
                yield return RunStep("Denaturation", userProtocol.denaturation);

                UpdateScreenDisplay("Annealing", i + 1, userProtocol.cycles);
                yield return RunStep("Annealing", userProtocol.annealing);

                UpdateScreenDisplay("Extension", i + 1, userProtocol.cycles);
                yield return RunStep("Extension", userProtocol.extension);
            }

            // 最后延伸
            UpdateScreenDisplay("Final Extension", userProtocol.cycles, userProtocol.cycles);
            yield return RunStep("Final Extension", userProtocol.finalExtension);


            if (progressSlider) progressSlider.value = 1.0f;

            yield return ChangeTemperature(userProtocol.holdTemperature);
            statusText.text = "Finished - Hold 4°C";
            _isRunning = false;
            startButton.interactable = true;

            CalculateResult();
        }

        // 模拟单个步骤：变温 -> 保持
        private IEnumerator RunStep(string stepName, PCRStep step)
        {
            statusText.text = stepName;

            // 阶段A：变温
            yield return ChangeTemperature(step.temperature);

            // 阶段B：保持时间
            float timer = step.duration;
            while (timer > 0)
            {
                timer -= Time.deltaTime * timeScale;
                timerText.text = $"{timer:F0}s";
                yield return null;
            }
        }

        // 模拟真实的物理变温过程
        private IEnumerator ChangeTemperature(float target)
        {
            while (Mathf.Abs(_currentBlockTemp - target) > 0.1f)
            {
                // 根据是升温还是降温选择速率
                float rate = target > _currentBlockTemp ? heatRate : coolRate;

                // 移向目标温度
                _currentBlockTemp = Mathf.MoveTowards(_currentBlockTemp, target, rate * Time.deltaTime * timeScale);

                // 刷新屏幕
                tempText.text = $"{_currentBlockTemp:F1}°C";
                yield return null;
            }
            _currentBlockTemp = target;
        }

        private void UpdateScreenDisplay(string state, int currentCycle, int totalCycles)
        {
            statusText.text = state;
            cycleText.text = totalCycles > 0 ? $"{currentCycle}/{totalCycles}" : "";
        }

        // TODO: 通知全局单例进行下一个步骤
        private void CalculateResult()
        {
            if (_loadedTube == null) return;

            // 检查用户的复性温度设置是否在引物Tm值的允许范围内
            float tempDiff = Mathf.Abs(userProtocol.annealing.temperature - targetAnnealingTemp);

            bool success = tempDiff <= allowableError;

            // 将结果写入试管
            _loadedTube.ReceivePCRResult(success);

            Debug.Log(success ? "<color=green>PCR Success!</color>" : "<color=red>PCR Failed: Wrong Temp</color>");
        }

        // TODO: 做学生输入温度和其他设置 —— 修改数据
    }
}