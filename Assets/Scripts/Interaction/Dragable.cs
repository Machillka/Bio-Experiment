using UnityEngine;

namespace Bio.Interaction
{

    [RequireComponent(typeof(Collider))]
    public class Dragable : MonoBehaviour
    {
        [Header("Drag Settings")]
        public float hoverRadius = 0.15f;      // 悬停检测半径
        public float dropRadius = 0.2f;        // 放置检测半径
        public float returnSpeed = 8f;         // 回弹速度
        public bool smoothReturn = true;       // 是否平滑回弹

        private Camera cam;
        private bool dragging;
        private Vector3 offset;
        private float zDepth;

        private Vector3 originalPosition;
        private Quaternion originalRotation;

        private bool returning;
        private Vector3 returnTarget;

        void Start()
        {
            cam = Camera.main;
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        void Update()
        {
            if (returning && smoothReturn)
            {
                transform.position = Vector3.Lerp(
                    transform.position, returnTarget, Time.deltaTime * returnSpeed);

                if (Vector3.Distance(transform.position, returnTarget) < 0.01f)
                {
                    transform.position = returnTarget;
                    returning = false;
                }
            }
        }

        void OnMouseDown()
        {
            if (returning) returning = false;

            dragging = true;
            originalPosition = transform.position;
            originalRotation = transform.rotation;

            zDepth = cam.WorldToScreenPoint(transform.position).z;
            offset = transform.position - GetMouseWorldPos();
        }

        void OnMouseDrag()
        {
            if (!dragging) return;

            Vector3 targetPos = GetMouseWorldPos() + offset;
            transform.position = targetPos;

            // 悬停检测：找到最近的 DropZone（可选：用于高亮）
            Collider[] hits = Physics.OverlapSphere(transform.position, hoverRadius);
            DropZone bestZone = null;
            float bestDist = Mathf.Infinity;

            foreach (var h in hits)
            {
                var dz = h.GetComponent<DropZone>();
                if (dz == null) continue;

                float d = Vector3.Distance(transform.position, dz.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestZone = dz;
                }
            }

            // 通知 DropZone 悬停（用于高亮或 UI 提示）
            DropZone.SetCurrentHover(bestZone);
        }

        void OnMouseUp()
        {
            dragging = false;
            TryDrop();
        }

        Vector3 GetMouseWorldPos()
        {
            Vector3 mp = Input.mousePosition;
            mp.z = zDepth;
            return cam.ScreenToWorldPoint(mp);
        }

        void TryDrop()
        {
            // 销毁当前悬停高亮
            DropZone.SetCurrentHover(null);

            Collider[] hits = Physics.OverlapSphere(transform.position, dropRadius);
            DropZone bestZone = null;
            float bestDist = Mathf.Infinity;

            foreach (var h in hits)
            {
                var dz = h.GetComponent<DropZone>();
                if (dz == null) continue;

                float d = Vector3.Distance(transform.position, dz.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestZone = dz;
                }
            }

            if (bestZone != null)
            {
                bestZone.OnObjectDropped(gameObject);
            }
            else
            {
                // 没有命中 DropZone → 回弹
                ReturnToOriginal();
            }
        }

        public void ReturnToOriginal()
        {
            if (smoothReturn)
            {
                returning = true;
                returnTarget = originalPosition;
            }
            else
            {
                transform.position = originalPosition;
            }
            transform.rotation = originalRotation;
        }
    }

}