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

            if (player.IsWallClinging()) {
                anim = Anim.WallCling; // TODO: WallPush if IsGrounded
            } else if (player.IsJumping()) {
                anim = Anim.Jump;
            } else if (player.IsFalling()) {
                anim = Anim.Fall;
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
