using System;
using UnityEngine;
using Physics;
using Anim = Animation.AnimationManagerMc.McAnimation;
using General;

namespace Animation {
    [RequireComponent(typeof(AnimationManagerMc))]
    [RequireComponent(typeof(Player))]
    public class PlayerAnimationController : MonoBehaviour {
        private Anim anim;
        private AnimationManagerMc animManager;
        private Player player;
        private SpriteRenderer spriteRenderer;

        private void Awake() {
            animManager = GetComponent<AnimationManagerMc>();
            player = GetComponent<Player>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void LateUpdate() {
            var previousAnim = anim;

            if (player.GetAttackType() == Player.AttackType.DashForward) {
                anim = Anim.DashForward;
            } else if (player.GetAttackType() == Player.AttackType.DashBackward) {
                anim = Anim.DashBackward;
            } else if (player.GetAttackType() == Player.AttackType.Jab1) {
                anim = player.IsAirbourne() ? Anim.AirPunch1 : Anim.Punch1;
            } else if (player.GetAttackType() == Player.AttackType.Jab2) {
                anim = player.IsAirbourne() ? Anim.AirPunch2 : Anim.Punch2;
            } else if (player.GetAttackType() == Player.AttackType.Jab3) {
                anim = player.IsAirbourne() ? Anim.AirPunch3 : Anim.Punch3;
            } else if (player.IsWallPushing()) {
                anim = Anim.Push;
            } else if (player.IsWallSliding()) {
                anim = Anim.WallCling;
            } else if (player.IsJumping()) {
                anim = Anim.Jump;
            } else if (player.IsFalling()) {
                anim = Anim.Fall;
            } else if (player.IsSlidng()) {
                anim = Anim.Slide;
            } else if (player.IsMovingHorizontally()) {
                anim = Anim.Run;
            } else {
                anim = Anim.Idle;
            }

            if (anim != previousAnim)
                animManager.PlayAnimation(anim);

            // Flip sprite if facing left
            spriteRenderer.flipX = player.IsFacingLeft();
            //spriteRenderer.transform.localPosition = new Vector3(player.IsFacingLeft() ? Util.PIXEL * -0.5f : 0f, 0f, 0f);
        }
    }
}
