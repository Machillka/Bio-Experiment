
namespace PCR
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class StepBase : MonoBehaviour
    {
        protected ExperimentContext Data => PCRManager.Instance.Data;
        public List<GameObject> sceneObjects;
        // 由 PCR Manager 调用的入口
        public virtual void OnEnter()
        {
            SetCurrentSceneActive(true);
        }

        public virtual void OnExit()
        {
            SetCurrentSceneActive(false);
        }

        public void SetCurrentSceneActive(bool active)
        {
            foreach (var obj in sceneObjects)
            {
                obj.SetActive(active);
            }

            gameObject.SetActive(active);
        }
    }
}