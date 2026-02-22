
namespace PCR
{
    using UnityEngine;

    public abstract class StepBase : MonoBehaviour
    {
        protected ExperimentContext Data => PCRManager.Instance.Data;

        // 由 PCR Manager 调用的入口
        public abstract void OnEnter();
        public abstract void OnExit();
    }
}