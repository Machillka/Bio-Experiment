using UnityEngine;

namespace Bio.Interaction
{
    /// <summary>
    /// collider 表示可交互范围
    /// </summary>
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class DropZone : MonoBehaviour
    {
        [Header("Zone Settings")]
        public string acceptedType = "";        // 空 = 接受所有实体类型
        public bool snapToZone = true;          // 是否吸附到 DropZone 中心
        public Vector3 snapOffset = new Vector3(0, 0.05f, 0);
        public bool allowMultiple = true;       // 是否允许多个物体放在同一 DropZone

        [Header("Highlight")]
        public Color hoverColor = Color.yellow;
        public Color validColor = Color.green;
        public Color invalidColor = Color.red;

        private static DropZone currentHover;
        private Renderer rend;
        private Color originalColor;

        void Start()
        {
            rend = GetComponentInChildren<Renderer>();
            if (rend != null)
                originalColor = rend.material.color;
        }

        public static void SetCurrentHover(DropZone dz)
        {
            if (currentHover == dz) return;

            // 清除之前的高亮
            if (currentHover != null)
                currentHover.SetHighlight(false, Color.clear);

            currentHover = dz;

            if (currentHover != null)
                currentHover.SetHighlight(true, currentHover.hoverColor);
        }

        void SetHighlight(bool on, Color color)
        {
            if (rend == null) return;
            rend.material.color = on ? color : originalColor;
        }

        public void OnObjectDropped(GameObject obj)
        {
            // var source = obj.GetComponent<EntityBase>();
            // var target = GetComponent<EntityBase>(); // 容器或设备本身也可以是实体

            // if (source == null)
            // {
            //     Debug.LogWarning("DropZone: Dropped object has no EntityInstance");
            //     return;
            // }

            // bool typeOk = string.IsNullOrEmpty(acceptedType) || source.Type == acceptedType;

            // if (!typeOk)
            // {
            //     // 类型不匹配 → 高亮红色 + 回弹
            //     SetHighlight(true, invalidColor);
            //     // UIManager.Instance?.ShowMessage($"该区域不接受类型：{source.Type}");
            //     obj.GetComponent<Dragable>()?.ReturnToOriginal();
            //     return;
            // }

            // 类型匹配 → 吸附位置
            if (snapToZone)
            {
                obj.transform.position = transform.position + snapOffset;
            }

            // 短暂高亮绿色
            SetHighlight(true, validColor);

            // InteractionManager.Instance?.OnDrop(source, target);
        }

        void OnDisable()
        {
            if (currentHover == this)
                currentHover = null;
        }
    }


}