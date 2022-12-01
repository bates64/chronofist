using System;
using Physics;
using UnityEngine;

namespace Health
{
    public class PlayerRelay : MonoBehaviour
    {
        private Player player;

        private void Awake()
        {
            player = GetComponentInParent<Player>();
        }

        private void OnTriggerStay2D(Collider2D col) {
            player.RelayCol(col);
        }
    }
}
