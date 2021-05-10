using UnityEngine;

namespace Enemy
{
    public class ProjectilePlayer : MonoBehaviour
    {
        private Vector3 direction;

        public void Setup(Vector3 dir)
        {
            direction = dir;
            Destroy(gameObject, 2f);
        }

        private void Update()
        {
            float speed = 5;
            transform.position += direction * (Time.deltaTime * speed);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag != "BalloonPad" && collider.tag != "Grass" && collider.tag != "Player")
            {
                Destroy(gameObject);
            }
        }
    }
}