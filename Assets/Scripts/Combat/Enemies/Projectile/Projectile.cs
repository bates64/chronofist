using System;
using Physics;
using Unity.Mathematics;
using UnityEngine;

namespace Combat.Enemies.Projectile
{
    public class Projectile : MonoBehaviour
    {
        public Vector2 direction;
        public float speed;
        public GameObject explosion;
        private float buffer;
        private float timer = 0;
        public float lifeTime = 5;
        private CircleCollider2D col;
        public ContactFilter2D levelLayer;

        private void Awake()
        {
            col = GetComponent<CircleCollider2D>();
        }

        private void Update()
        {
            timer += LocalTime.DeltaTimeAt(transform.position);
            Vector2 movement = direction * (speed * LocalTime.DeltaTimeAt(transform.position));
            transform.Translate(movement);
            if (buffer > 0)
            {
                buffer += Time.deltaTime;
            }
            if (buffer >= 0.15f || timer >= lifeTime)
            {
                Instantiate(explosion, transform.position, quaternion.identity);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            buffer = 0.01f;
        }
    }
}
