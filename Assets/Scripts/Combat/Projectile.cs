using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] bool isSeeking = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifetime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lingerAfterHit = 2f;
        [SerializeField] UnityEvent onHit;

        Health target = null;
        GameObject attacker = null;
        float damage;

        void Start() {
            if (!target) return;
            transform.LookAt(GetAimLocation());
            Destroy(gameObject, maxLifetime);
        }

        // Update is called once per frame
        void Update()
        {
            if (!target) return;

            if (isSeeking && !target.IsDead) {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject attacker, float damage) {
            this.target = target;
            this.damage = damage;
            this.attacker = attacker;
        }

        void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Health>() != target) return;
            if (target == null) return;
            if (target.IsDead) return;

            onHit.Invoke();
            target.TakeDamage(attacker, damage);

            speed = 0;

            if (hitEffect != null) {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }
            foreach (GameObject toDestroy in destroyOnHit) {
                Destroy(toDestroy);
            }
            Destroy(gameObject, lingerAfterHit);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            else
            {
                return target.transform.position + Vector3.up * targetCapsule.height / 2;
            }
        }
    }
}
