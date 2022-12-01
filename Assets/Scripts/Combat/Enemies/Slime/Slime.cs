using System;
using Effects;
using Physics;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Combat.Enemies.Slime
{
    public class Slime : Enemy
    {
        private SlimeState CurrentState;
        public Animator Animator;
        [HideInInspector] public Vector2 Velocity;
        public Vector2 BigJumpForce;
        public Vector2 SmallJumpForce;
        public float IdleTime;
        public float Radius;
        private ParticleSystem _particles;
        [SerializeField] private GameObject explosion;

        public SlimeIdle IdleState;
        public SlimeFalling FallingState;
        public SlimeJump JumpState;

        protected override void Awake()
        {
            base.Awake();
            Animator = GetComponent<Animator>();
            _particles = GetComponentInChildren<ParticleSystem>();
            InitializeStates();
            health.OnDeath += Death;
        }

        private void InitializeStates()
        {
            IdleState = new SlimeIdle(this);
            FallingState = new SlimeFalling(this);
            JumpState = new SlimeJump(this);
            SetState(IdleState);
        }

        public void Gravity()
        {
            Velocity.y -= gravity * DeltaTime;
        }

        protected override void Update()
        {
            Cooldowns(DeltaTime);
            CurrentState.Update();
        }

        public void SetState(SlimeState nextState)
        {
            CurrentState?.ExitState();
            CurrentState = nextState;
            CurrentState?.EnterState();
        }

        protected override void WhenDamage()
        {
            _particles.Play();
            Animator.SetTrigger(DamageS);
        }

        protected override void WhenKnockBackApplied(Vector2 knockback)
        {
            Velocity = knockback;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        private void Death()
        {
            RestoreTime(Player.Instance);
            _particles.transform.parent = null;
            DefaultEffect effect = _particles.gameObject.AddComponent<DefaultEffect>();
            effect.TimeToLive = 1;
            Instantiate(explosion,transform.position,quaternion.identity);
            Destroy(gameObject);
        }
    }
}
