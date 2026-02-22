namespace PCR
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    /// <summary>
    /// 纯数据类，用于各个步骤之间传输数据，类似 blackboard
    /// </summary>
    [Serializable]
    public class ExperimentContext
    {
        [Header("Step 1")]
        public string TargetDnaSequence;
        public string GeneName;          // 基因名称
        public string BtGeneSequence;    // 目标DNA序列
        public int GenBankID;            // ID ( 假设需要做多种 presets 进行选择 使用 uuid 进行查找)

        [Header("Step 2")]
        public string ForwardPrimer;     // 上游引物
        public string ReversePrimer;     // 下游引物
        public float MeltingTemp;        // 熔解温度(Tm)

        // WORKFLOW: 之后步骤产生的数据
        // TODO: 写一个 editor 实现调整


        [Header("Step 3")]
        public bool IsEquipmentSterilized;

        // kv: 试剂名 -> 加入的体积(uL), 用于本地记录
        public Dictionary<string, float> TubeContents = new Dictionary<string, float>();

        [Header("Step 4")]
        // TODO: 使用 recipe 查找一致性
        public readonly Dictionary<string, float> TargetRecipe = new Dictionary<string, float>
        {
            { "Buffer", 5f },
            { "dNTPs", 1f },
            { "Primer1", 2.5f },
            { "Primer2", 2.5f },
            { "H2O", 30f },
            { "TaqEnzyme", 1f },
            { "TemplateDNA", 8f } // 总共 50uL
        };


        /// <summary>
        /// 重置数据
        /// </summary>
        public void Clear()
        {
            BtGeneSequence = "";
            ForwardPrimer = "";
            ReversePrimer = "";
            IsEquipmentSterilized = false;
            TubeContents.Clear();
        }

    }
}