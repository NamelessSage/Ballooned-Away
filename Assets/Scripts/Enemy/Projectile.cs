using UnityEngine;

namespace Enemy
{
    public class Projectile : MonoBehaviour
    {
        private Vector3 direction;

        public void Setup(Vector3 dir)
        {
            direction = dir;
        }

        private void Update()
        {
            float speed = 5;
            transform.position += direction * (Time.deltaTime * speed);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag != "BalloonPad" && collider.tag != "Grass" && collider.tag != "Enemy")
            {
                Destroy(gameObject);
            }
        }
    }
}