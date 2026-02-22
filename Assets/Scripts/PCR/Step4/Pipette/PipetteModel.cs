using UnityEngine;

namespace PCR
{
    [RequireComponent(typeof(LiquidContainer))]
    public class PipetteModel : MonoBehaviour
    {
        // [Header("Spec")]
        // public float maxVolume = 100f;  // 量程 (uL)
        // public float transferSpeed = 20f; // 单次点击吸排量

        // [SerializeField] private bool _hasTip = false;

        // public LiquidContainer internalContainer;
        // public ReagentDataSO CurrentReagent;

        // private float _currentVolume => internalContainer.CurrentTotalVolume;

        // public bool HasTip => _hasTip;
        // public float CurrentVolume => internalContainer.CurrentTotalVolume;
        // public Color LiquidColor => internalContainer.liquidColor;

        // private void Start()
        // {
        //     NotifyStateChange();
        // }

        // // 装枪头
        // public bool TryEquipTip()
        // {
        //     if (_hasTip) return false; // 已经有了

        //     _hasTip = true;
        //     PCREventBus.PublishAction(PipetteActionType.EquipTip);
        //     NotifyStateChange();
        //     return true;
        // }

        // // 卸枪头
        // public bool TryEjectTip()
        // {
        //     if (!_hasTip) return false;
        //     if (internalContainer.CurrentTotalVolume > 0)
        //     {
        //         // 有液体不能随便卸 防止污染环境
        //         // 允许卸载但视为错误
        //         Debug.LogWarning("警告：带液卸枪头！");
        //     }

        //     _hasTip = false;
        //     _currentVolume = 0; // 液体随枪头丢弃
        //     _liquidColor = Color.clear;

        //     PCREventBus.PublishAction(PipetteActionType.EjectTip);
        //     NotifyStateChange();
        //     return true;
        // }

        // // 吸液
        // public void Aspirate(LiquidContainer sourceContainer, float amount)
        // {
        //     if (!_hasTip)
        //     {
        //         PCREventBus.PublishAction(PipetteActionType.Error);
        //         return;
        //     }

        //     var actualVolume = sourceContainer.RemoveLiquid(amount, out var reagent);

        //     // 颜色混合逻辑
        //     if (_currentVolume > 0)
        //     {
        //         _liquidColor = Color.Lerp(_liquidColor, reagent.liquidColor, 0.5f);
        //     }
        //     else
        //     {
        //         _liquidColor = reagent.liquidColor;
        //     }

        //     // 限制最大容量
        //     float spaceLeft = maxVolume - _currentVolume;
        //     float actualAmount = Mathf.Min(amount, spaceLeft);

        //     _currentVolume += actualAmount;

        //     PCREventBus.PublishAction(PipetteActionType.Aspirate);
        //     NotifyStateChange();
        // }

        // // 4. 排液 (一档：排出一部分)
        // public float Dispense(float amountRequest, out Color outColor)
        // {
        //     if (_currentVolume <= 0)
        //     {
        //         outColor = Color.clear;
        //         return 0;
        //     }

        //     float actualAmount = Mathf.Min(amountRequest, _currentVolume);
        //     _currentVolume -= actualAmount;
        //     outColor = _liquidColor;

        //     if (_currentVolume <= 0.01f) _liquidColor = Color.clear;

        //     PCREventBus.PublishAction(PipetteActionType.Dispense);
        //     NotifyStateChange();
        //     return actualAmount;
        // }

        // // 5. 吹出 (二档：强制排空剩余液体)
        // public float Blowout(out Color outColor)
        // {
        //     if (_currentVolume <= 0)
        //     {
        //         outColor = Color.clear;
        //         return 0;
        //     }

        //     float allAmount = _currentVolume;
        //     outColor = _liquidColor;

        //     _currentVolume = 0;
        //     _liquidColor = Color.clear;

        //     PCREventBus.PublishAction(PipetteActionType.Blowout);
        //     NotifyStateChange();
        //     return allAmount;
        // }

        // private void NotifyStateChange()
        // {
        //     PCREventBus.PublishPipetteState(_currentVolume, maxVolume, _hasTip, _liquidColor);
        // }

        [Header("Hardware Specs")]
        public float MaxRange = 10f; // 最大量程 10uL

        [field: SerializeField] public bool HasTip { get; private set; }
        [field: SerializeField] public float SettingVolume { get; private set; } = 2.5f; // 设定量程

        private LiquidContainer _internalTank;

        public float HeldVolume => _internalTank.CurrentVolume;

        private void Awake()
        {
            _internalTank = GetComponent<LiquidContainer>();
        }

        public void EquipTip() => HasTip = true;
        public void EjectTip()
        {
            HasTip = false;
            // 卸枪头时，里面的液体也一起丢了
            _internalTank.RemoveBatch(_internalTank.CurrentVolume);
        }

        public void SetVolume(float vol)
        {
            SettingVolume = Mathf.Clamp(vol, 0.1f, MaxRange);
        }

        // 代理操作：将内部容器暴露给 Controller
        public LiquidContainer Tank => _internalTank;
    }
}

