using System;
using UnityEngine;

namespace Animation
{
    public class AnimationManagerMc : MonoBehaviour
    {
        public static readonly int StateId = Animator.StringToHash("StateId");
        public static readonly int PrevStateId = Animator.StringToHash("PrevStateId");
        public static readonly int Play = Animator.StringToHash("Play");

        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayAnimation(McAnimation anim)
        {
            _animator.SetInteger(PrevStateId,_animator.GetInteger(StateId));
            _animator.SetInteger(StateId,(int) anim);
            _animator.SetTrigger(Play);
            _animator.Update(0);
        }

        public enum McAnimation
        {
            Idle = 0,
            Run = 1,
            Fall = 2,
            Jump = 3,
            WallCling = 4,
            DashForward = 5,
            DashBackward = 6,
            Slide = 7,
            Push = 8,
            Punch1 = 9,
            Punch2 = 10,
            Punch3 = 11,
            AirPunch1 = 12,
            AirPunch2 = 13,
            AirPunch3 = 14,
            Uppercut = 15,
            Slam = 16,
            Damage = 17
        }
    }
}
