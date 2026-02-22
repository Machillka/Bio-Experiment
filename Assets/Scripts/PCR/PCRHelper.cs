using System;
using UnityEngine;

namespace PCR.Helper
{
    public enum ContainerType
    {
        Source,     // 试剂瓶 (无限/有限资源)
        Destination,// 目标反应管
        TipBox,     // 枪头盒
        Trash,      // 垃圾桶
        Pipette     // 移液枪本身
    }

    public enum PipetteActionType
    {
        EquipTip,       // 装枪头
        EjectTip,       // 卸枪头
        Aspirate,       // 吸液
        Dispense,       // 排液
        Blowout,        // 二档吹出 (排空)
        Error           // 操作错误
    }

}

