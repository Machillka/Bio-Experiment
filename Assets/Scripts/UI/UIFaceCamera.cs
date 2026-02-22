using UnityEngine;

namespace PCR
{
    public class UIFaceCamera : MonoBehaviour
    {
        private Camera _cam;

        void Start()
        {
            _cam = Camera.main;
        }

        void LateUpdate() // 使用 LateUpdate 防止抖动
        {
            transform.rotation = _cam.transform.rotation;
        }
    }
}