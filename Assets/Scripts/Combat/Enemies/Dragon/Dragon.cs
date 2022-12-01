using System;
using Effects;
using Physics;
using Unity.Mathematics;
using UnityEngine;

namespace Combat.Enemies.Dragon
{
    public class Dragon : Enemy
    {
        private DragonState CurrentState;
        public float Speed;
        public Animator Animator;
        public Vector2 Velocity;
        public float FlyRadius;
        public float AggroRadius;
        public float FireRadius;
        [HideInInspector] public Vector2 Home;
        public float IdleTime;
        public float StunTime;
        public GameObject projectile;
        private ParticleSystem _particles;
        [SerializeField] private GameObject explosion;

        public DragonIdle IdleState;
        public DragonReturnToHome ReturnToHomeState;
        public DragonChase ChaseState;
        public DragonFireState FireState;
        public DragonKnockbackState KnockbackState;

        private bool WithinRange(float radius)
        {
            return Mathf.Abs(Vector2.Distance(Enemy.PlayerPosition,transform.position)) <= radius;
        }

        public bool WithinAggro => WithinRange(AggroRadius);
        public bool WithinFire => WithinRange(FireRadius);

        protected override void Awake()
        {
            base.Awake();
            Animator = GetComponent<Animator>();
            _particles = GetComponentInChildren<ParticleSystem>();
            InitializeStates();
            health.OnDeath += Death;

            CurrentState = IdleState;
        }

        private void Start()
        {
            Home = transform.position;
        }

        private void InitializeStates()
        {
            IdleState = new DragonIdle(this);
            ReturnToHomeState = new DragonReturnToHome(this);
            ChaseState = new DragonChase(this);
            FireState = new DragonFireState(this);
            KnockbackState = new DragonKnockbackState(this);
        }

        public void SetState(DragonState nextState)
        {
            CurrentState?.ExitState();
            CurrentState = nextState;
            CurrentState?.EnterState();
        }

        public void SkipToIdle()
        {
            CurrentState?.ExitState();
            CurrentState = IdleState;
            IdleState.SetTimer(IdleTime);
        }

        protected override void Update()
        {
            Cooldowns(DeltaTime);
            CurrentState.Update();
        }

        protected override void WhenDamage()
        {
            _particles.Play();
            Animator.SetTrigger(DamageS);
        }

        public void Fire()
        {
            Projectile.Projectile p = Instantiate(projectile, transform.position, quaternion.identity).GetComponent<Projectile.Projectile>();
            p.direction = PlayerPosition - (Vector2) transform.position;
        }

        protected override void WhenKnockBackApplied(Vector2 knockback)
        {
            Velocity = knockback;
            SetState(KnockbackState);
        }

        public void Gravity()
        {
            Velocity.y -= gravity * DeltaTime;
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AggroRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Home, FlyRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, FireRadius);
        }
    }
}
