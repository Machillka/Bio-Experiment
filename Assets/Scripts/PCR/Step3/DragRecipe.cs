using System.Collections.Generic;
using UnityEngine;

namespace PCR
{
    [CreateAssetMenu(fileName = "DragRecipe", menuName = "PCR/DragRecipe")]
    public class DragRecipe : ScriptableObject
    {
        public List<string> neededIds;      // 所有需要的 id

        // NOTE:
        // 判定可以更简单 —— 直接查看子物体是否全部被 destroy, 或者 active 被 set false
        // 节省配方的空间, 但是为了方便查看,依旧定义配方字段
    }
}