using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class Flag : FieldObject
    {
        public GameObject explosionParticlesPrefab;
        public MenuController menuController;
        void Start()
        {
            menuController = GameObject.FindObjectOfType<MenuController>();
        }
        public override void Die()
        {
            menuController.EndGame(false);
            if (explosionParticlesPrefab)
            {
                GameObject explosion = (GameObject)Instantiate(explosionParticlesPrefab, transform.position, explosionParticlesPrefab.transform.rotation);
                Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
            }
            Destroy(gameObject);
        }
    }

}