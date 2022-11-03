using System;
using TMPro;
using UnityEngine;

namespace UI
{
    [Serializable]
    public struct DebugUi
    {
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private TextMeshProUGUI textMeshVel;
        public void SetStateName(string stateName)
        {
            textMesh.SetText(stateName);
        }

        public void SetVelocity(Vector2 velocity)
        {
            double x = System.Math.Round(velocity.x, 3);
            double y = System.Math.Round(velocity.y, 3);
            textMeshVel.SetText("X:" + x + "\n" + "Y:" + y);
        }
        
    }
}