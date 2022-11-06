using System;
using System.Collections.Generic;
using PassiveItems;
using Physics;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Player_
{
    [Serializable]
    public class PlayerProperties
    {
        [SerializeField] private float jumpHeight;
        [SerializeField] private float timeToJumpApex;
        [SerializeField] private float movementSpeed;
        [SerializeField] private List<PassiveItem> passiveItems;
        private Health.Health _health;
        private Gravity _gravity;
        private float _jumpVelocity;

        public Health.Health Health
        {
            get => _health;
            set => _health = value;
        }

        public Gravity Gravity => _gravity;
        public float JumpVelocity => _jumpVelocity;
        public float MovementSpeed => movementSpeed;

        public List<PassiveItem> PassiveItems => passiveItems;

        public void Init(GameObject gameObject)
        {
            SetupJump();
            _health = gameObject.GetComponent<Health.Health>();
        }
        
        private void SetupJump()
        {
            float gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            _jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            _gravity = new Gravity(gravity);
        }
        
    }
}