// using UnityEngine;

// namespace PCR
// {

//     public class PipetteDragger : MonoBehaviour
//     {
//         [Header("Plane Settings")]
//         public float workHeight = 5.0f;     // 实验台的高度 (Y轴)
//         public LayerMask snapLayer;         // 吸附点的层级

//         [Header("Feel Settings")]
//         public float smoothSpeed = 20f;     // 跟手速度 (Lerp)
//         public float snapRadius = 1.0f;     // 吸附半径
//         public Vector3 dragOffsetLift = new Vector3(0, 0.05f, 0);

//         // --- 内部状态 ---
//         private Camera _cam;
//         private bool _isDragging = false;
//         private Vector3 _targetPosition;    // 逻辑目标点
//         private Vector3 _mouseOffset;       // 鼠标点击点与物体中心的偏移

//         // 当前吸附的目标
//         public LiquidContainer CurrentSnapTarget { get; private set; }

//         private void Start()
//         {
//             _cam = Camera.main;
//             Debug.Log(_cam);
//             _targetPosition = transform.position;
//         }

//         private void Update()
//         {
//             transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * smoothSpeed);
//             transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10f);
//         }

//         private void OnMouseDown()
//         {
//             _isDragging = true;
//             CurrentSnapTarget = null; // 重置吸附

//             Vector3 mouseOnPlane = GetMousePositionOnPlane();
//             _mouseOffset = transform.position - mouseOnPlane;
//             _mouseOffset.y = 0;         // y 轴最后计算
//         }

//         private void OnMouseDrag()
//         {
//             if (!_isDragging) return;

//             Vector3 rawPos = GetMousePositionOnPlane() + _mouseOffset;

//             rawPos.y = workHeight + dragOffsetLift.y;
//             CheckSnap(rawPos);
//         }

//         private void OnMouseUp()
//         {
//             _isDragging = false;

//             if (CurrentSnapTarget != null)
//             {
//                 Debug.Log($"<color=cyan>吸附在: {CurrentSnapTarget.name}</color>");
//             }
//             else
//             {
//                 _targetPosition = new Vector3(_targetPosition.x, workHeight, _targetPosition.z);
//             }
//         }


//         private Vector3 GetMousePositionOnPlane()
//         {
//             Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

//             Plane tablePlane = new Plane(Vector3.up, new Vector3(0, workHeight, 0));

//             float enter;
//             if (tablePlane.Raycast(ray, out enter))
//             {
//                 return ray.GetPoint(enter);
//             }
//             return transform.position;
//         }

//         // --- 磁吸检测逻辑 ---
//         private void CheckSnap(Vector3 worldPos)
//         {
//             Collider[] hits = Physics.OverlapSphere(worldPos, snapRadius, snapLayer);

//             Transform bestHitTransform = null;
//             LiquidContainer bestContainer = null;
//             float closestDist = float.MaxValue;

//             foreach (var hit in hits)
//             {
//                 float d = Vector3.Distance(worldPos, hit.transform.position);
//                 Debug.Log("Name: " + hit.name);
//                 if (d < closestDist)
//                 {
//                     if (hit.TryGetComponent<LiquidContainer>(out var container))
//                     {
//                         closestDist = d;
//                         bestHitTransform = hit.transform;
//                         bestContainer = container;
//                     }
//                 }
//             }
//             if (bestHitTransform != null)
//             {
//                 _targetPosition = bestHitTransform.position;
//             }
//             else
//             {
//                 _targetPosition = worldPos;
//             }
//             CurrentSnapTarget = bestContainer;
//         }

//         // 可视化辅助
//         private void OnDrawGizmos()
//         {
//             Gizmos.color = Color.yellow;
//             Gizmos.DrawWireSphere(transform.position, snapRadius);
//         }
//     }
// }

using UnityEngine;

namespace PCR
{
    public class PipetteDragger : MonoBehaviour
    {
        [Header("Plane Settings")]
        public float workHeight = 5.0f;
        public LayerMask snapLayer;

        [Header("Feel Settings - Movement")]
        public float smoothTime = 0.05f;    // 使用SmoothDamp的平滑时间，越小越跟手，越大越有惯性
        public float liftHeight = 0.5f;     // 拖拽时抬起的高度

        [Header("Feel Settings - Magnet")]
        public float searchRadius = 2.0f;   // 搜索半径：进入这个范围就开始检测
        public float snapIntensity = 1.5f;  // 吸附强度：值越大，吸附感越强
        [Range(0, 1)]
        public float snapHardThreshold = 0.3f; // 硬锁定阈值：距离中心多近时直接锁定 (0-1百分比)

        // --- 内部状态 ---
        private Camera _cam;
        private bool _isDragging = false;
        private Vector3 _currentVelocity;   // SmoothDamp 用的速度变量
        private Vector3 _targetPosition;    // 最终计算出的目标位置
        private Vector3 _mouseWorldPos;     // 鼠标在平面上的原始位置
        private Vector3 _mouseOffset;

        public LiquidContainer CurrentSnapTarget { get; private set; }
        private LiquidContainer _lastHighlighedTarget; // 用于处理高亮/取消高亮

        private void Start()
        {
            _cam = Camera.main;
            _targetPosition = transform.position;
        }

        private void Update()
        {
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentVelocity, smoothTime);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10f);
        }

        private void OnMouseDown()
        {
            _isDragging = true;
            CurrentSnapTarget = null;

            // 计算鼠标点击点与物体中心的初始偏差
            _mouseWorldPos = GetMousePositionOnPlane();
            _mouseOffset = transform.position - _mouseWorldPos;
            _mouseOffset.y = 0;
        }

        private void OnMouseDrag()
        {
            if (!_isDragging) return;

            Vector3 rawMousePos = GetMousePositionOnPlane() + _mouseOffset;

            rawMousePos.y = workHeight + liftHeight;

            _targetPosition = CalculateMagneticPosition(rawMousePos);
        }

        private void OnMouseUp()
        {
            _isDragging = false;

            // 鼠标松开时，如果已经吸附了目标，就直接对齐过去
            if (CurrentSnapTarget != null)
            {
                _targetPosition = CurrentSnapTarget.transform.position;
                _targetPosition.y = workHeight; // 放回工作高度
                Debug.Log($"<color=cyan>成功吸附: {CurrentSnapTarget.name}</color>");

            }
            else
            {
                _targetPosition = new Vector3(_targetPosition.x, workHeight, _targetPosition.z);
            }

            if (_lastHighlighedTarget != null)
            {
                _lastHighlighedTarget.SetHighlight(false);
                _lastHighlighedTarget = null;
            }
        }

        private Vector3 CalculateMagneticPosition(Vector3 mousePos)
        {
            // 在大范围内搜索最近的物体
            Collider[] hits = Physics.OverlapSphere(mousePos, searchRadius, snapLayer);

            LiquidContainer bestContainer = null;
            float closestDist = float.MaxValue;
            Vector3 bestTargetPos = mousePos;

            foreach (var hit in hits)
            {
                // 忽略自身（如果有Collider的话）
                if (hit.transform == transform) continue;

                if (hit.TryGetComponent<LiquidContainer>(out var container))
                {
                    // 计算平面距离 (忽略Y轴差异)
                    float d = Vector3.Distance(new Vector3(mousePos.x, 0, mousePos.z),
                                               new Vector3(hit.transform.position.x, 0, hit.transform.position.z));

                    if (d < closestDist)
                    {
                        closestDist = d;
                        bestContainer = container;
                        bestTargetPos = hit.transform.position;
                        bestTargetPos.y = mousePos.y; // 保持拖拽高度
                    }
                }
            }

            // 处理高亮反馈
            if (bestContainer != _lastHighlighedTarget)
            {
                if (_lastHighlighedTarget)
                    _lastHighlighedTarget.SetHighlight(false);
                if (bestContainer)
                    bestContainer.SetHighlight(true);
                _lastHighlighedTarget = bestContainer;
            }

            CurrentSnapTarget = bestContainer;


            if (bestContainer != null)
            {

                float t = Mathf.Clamp01(closestDist / searchRadius);

                if (t < snapHardThreshold)
                {
                    return bestTargetPos;
                }

                float weight = Mathf.Pow(1 - t, snapIntensity);

                return Vector3.Lerp(mousePos, bestTargetPos, weight);
            }

            // 没有目标，完全跟随鼠标
            return mousePos;
        }

        private Vector3 GetMousePositionOnPlane()
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            Plane tablePlane = new Plane(Vector3.up, new Vector3(0, workHeight, 0));
            if (tablePlane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            return transform.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(transform.position, searchRadius);

            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, searchRadius * snapHardThreshold);

            if (CurrentSnapTarget != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, CurrentSnapTarget.transform.position);
            }
        }
    }
}