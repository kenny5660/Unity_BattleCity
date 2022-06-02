using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    internal class Tank : MonoBehaviour
    {
        public bool isMoving { get; private set; }

        public BulletController bulletPrefab;
        public GameObject explosionParticlesPrefab;
        public int ReloadSeconds = 5;

        public bool isEnemy;

        private float moveSpeed;
        protected float reloadTimer = 100;
        public FieldController fieldController;
        protected Cell[,] cells;
        private Vector2Int direction = Vector2Int.up;

        public virtual void Initialize(float moveSpeed, FieldController fieldController)
        {
            this.moveSpeed = moveSpeed;
            this.fieldController = fieldController;
            this.cells = fieldController.cells;
        }

        public IEnumerator TryMove(Vector2Int delta)
        {
            if (isMoving)
            {
                yield break;
            }

            isMoving = true;

            var rotationY = Vector2.SignedAngle(Vector2.up, delta * new Vector2Int(-1, 1));
            rotationY = rotationY < 0 ? rotationY + 360 : rotationY;
            var from = GetCoords();
            var tc = from + delta;

            bool canMove = tc.x > 0 && tc.x <= cells.GetLength(0) &&
                            tc.y > 0 && tc.y <= cells.GetLength(1) &&
                            cells[tc.x, tc.y].Occupant == null && cells[tc.x, tc.y].Space == CellSpace.Empty;

            direction = delta;
            if (canMove)
            {
                cells[tc.x, tc.y].Occupy(this);
                cells[from.x, from.y].Occupy(null);
            }
            else
            {
                cells[from.x, from.y].Occupy(this);
            }

            var currentPosition = new Vector3(from.x, 1, from.y);
            var targetPosition = new Vector3(tc.x, 1, tc.y);

            var currentRotation = gameObject.transform.rotation;

            var targetRotation = Quaternion.Euler(0, rotationY, 0);

            var moveTime = 1f / moveSpeed;
            float t = 0;
            while (t < moveTime)
            {
                t += Time.deltaTime;
                if (canMove)
                {
                    gameObject.transform.position = currentPosition + (t / moveTime) * (targetPosition - currentPosition);
                }

                // var f = Mathf.Min(1, 2 * t / moveTime);
                // gameObject.transform.eulerAngles = currentRotation + f * (targetRotation - currentRotation);
                this.transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, 2 * t / moveTime);
                yield return null;
            }
            if (canMove)
            {
                gameObject.transform.position = targetPosition;
            }
            gameObject.transform.rotation = targetRotation;

            isMoving = false;
        }

        protected Vector2Int GetCoords()
        {
            Vector2Int p = default;
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y].Occupant == this)
                    {
                        p = new Vector2Int(x, y);
                    }
                }
            }
            return p;
        }
        public void Update()
        {
            reloadTimer = reloadTimer + 1 * Time.deltaTime;
        }

        public bool Fire()
        {
            if (!isMoving)
            {
                if (reloadTimer > ReloadSeconds)
                {
                    var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                    bullet.Initialize(cells, isEnemy);
                    bullet.Fire(direction);
                    reloadTimer = 0;
                    return true;
                }
            }
            return false;
        }

        public virtual void Die()
        {
            StopAllCoroutines();
            var p = GetCoords();
            cells[p.x, p.y].Occupy(null);
            if (explosionParticlesPrefab)
            {
                GameObject explosion = (GameObject)Instantiate(explosionParticlesPrefab, transform.position, explosionParticlesPrefab.transform.rotation);
                Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
            }
            Destroy(gameObject);
        }
    }
}
