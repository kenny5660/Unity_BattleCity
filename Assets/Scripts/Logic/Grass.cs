using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class Grass : FieldObject
    {
        public GameObject explosionParticlesPrefab;
        public override void Die()
        {
            if (explosionParticlesPrefab)
            {
                GameObject explosion = (GameObject)Instantiate(explosionParticlesPrefab, transform.position, explosionParticlesPrefab.transform.rotation);
                Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
            }
            Destroy(gameObject);
        }
    }
}
