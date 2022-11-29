using System;
using TMPro;
using UnityEngine;

namespace UI {
    [Serializable]
    public struct DebugUi {
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private TextMeshProUGUI textMeshVel;
        [SerializeField] private TextMeshProUGUI textMeshLocalTime;

        public void SetStateName(string stateName) {
            if (textMesh != null) {
                textMesh.SetText(stateName);
            }
        }

        public void SetVelocity(Vector2 velocity) {
            if (textMeshVel == null) return;
            var x = Math.Round(velocity.x, 3);
            var y = Math.Round(velocity.y, 3);
            textMeshVel.SetText("xv:" + x + "\n" + "yv:" + y);
        }

        public void SetLocalTime(float localTime) {
            if (textMeshLocalTime == null) return;
            textMeshLocalTime.SetText("Local Time Multiplier: " + localTime);
        }
    }
}
