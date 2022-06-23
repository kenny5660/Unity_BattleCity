using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    internal class Flag : FieldObject
    {
        public GameObject explosionParticlesPrefab;
        protected FieldController fieldController;
        void Start()
        {
            fieldController = GameObject.FindObjectOfType<FieldController>();
        }
        public override void Die()
        {
            fieldController.EndGame(false);
            if (explosionParticlesPrefab)
            {
                GameObject explosion = (GameObject)Instantiate(explosionParticlesPrefab, transform.position, explosionParticlesPrefab.transform.rotation);
                Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
            }
            Destroy(gameObject);
        }
    }

}