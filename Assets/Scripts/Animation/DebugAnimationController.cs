using System;
using UnityEngine;

namespace Animation
{
    public class DebugAnimationController : MonoBehaviour
    {
        public AnimationManagerMc.McAnimation anim;
        private AnimationManagerMc.McAnimation _previousAnim;
        private AnimationManagerMc _manager;

        private void Awake()
        {
            _manager = GetComponent<AnimationManagerMc>();
        }

        private void Update()
        {
            if (_previousAnim != anim)_manager.PlayAnimation(anim);
            _previousAnim = anim;
        }
    }
}
