using UnityEngine;

namespace PCR
{
    [System.Serializable]
    public struct PCRStep
    {
        public float temperature;   // 目标温度 (℃)
        public float duration;      // 保持时间 (秒)
    }

    // TODO: 实现成 SO 文件用于配置与修改
    [System.Serializable]
    public class PCRProtocol
    {
        public string protocolName = "Standard PCR";

        [Header("Initialization")]
        public PCRStep initialDenaturation = new PCRStep { temperature = 94, duration = 10 };

        [Header("Cycling")]
        public int cycles = 30;
        public PCRStep denaturation = new PCRStep { temperature = 94, duration = 5 };
        public PCRStep annealing = new PCRStep { temperature = 55, duration = 5 };
        public PCRStep extension = new PCRStep { temperature = 72, duration = 10 };

        [Header("Finalization")]
        public PCRStep finalExtension = new PCRStep { temperature = 72, duration = 10 };
        public float holdTemperature = 4.0f;
    }
}