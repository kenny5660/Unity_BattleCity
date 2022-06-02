using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    internal class Player : Tank
    {
        public override void Initialize(float moveSpeed, FieldController fieldController)
        {
            base.Initialize(moveSpeed, fieldController);
            isEnemy = false;

        }
        public override void Die()
        {
            fieldController.PlayerDie();
            base.Die();
        }
    }
}
