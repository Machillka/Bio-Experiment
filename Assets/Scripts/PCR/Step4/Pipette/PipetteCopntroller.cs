using System;
using PCR;
using PCR.Helper;
using UnityEngine;

namespace PCR
{
    [RequireComponent(typeof(PipetteDragger))]
    [RequireComponent(typeof(PipetteModel))]
    public class PipetteCopntroller : MonoBehaviour
    {
        private PipetteModel _model;
        private PipetteDragger _movement;

        [Header("Controls")]
        // TODO: 迁移到 input system 跨平台
        public KeyCode ActionKey = KeyCode.Space;
        public KeyCode EjectKey = KeyCode.T;

        private void Awake()
        {
            _model = GetComponent<PipetteModel>();
            _movement = GetComponent<PipetteDragger>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(ActionKey)) HandleAction();
            if (Input.GetKeyDown(EjectKey)) HandleEjection();
        }

        private void HandleAction()
        {
            var target = _movement.CurrentSnapTarget;
            if (target == null) return;


            if (target.Type == ContainerType.TipBox)
            {
                _model.EquipTip();
                Debug.Log("Equipped Tip");
                return;
            }


            if (!_model.HasTip)
            {
                Debug.LogWarning("No Tip!");
                return;
            }


            if (_model.HeldVolume <= 0) // 枪是空的 -> 吸液
            {
                // 只能从 Source 或 Destination 吸
                if (target.Type == ContainerType.Source || target.Type == ContainerType.Destination)
                {
                    float amountToAspirate = _model.SettingVolume;
                    // M2M: Target -> Pipette
                    var batch = target.RemoveBatch(amountToAspirate);
                    _model.Tank.AddBatch(batch);
                    Debug.Log($"Aspirated {amountToAspirate}uL");
                }
            }
            else // 枪里有液 -> 注液
            {
                if (target.Type == ContainerType.Destination || target.Type == ContainerType.Trash)
                {
                    var batch = _model.Tank.RemoveBatch(_model.HeldVolume);
                    target.AddBatch(batch);
                    Debug.Log("Dispensed All");
                }
            }
        }

        private void HandleEjection()
        {
            var target = _movement.CurrentSnapTarget;
            if (target != null && target.Type == ContainerType.Trash)
            {
                _model.EjectTip();
                Debug.Log("Tip Ejected");
            }
            else
            {
                Debug.LogWarning("Must eject tip over Trash bin!");
            }
        }
    }

}