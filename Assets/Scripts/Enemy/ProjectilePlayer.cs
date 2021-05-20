using UnityEngine;

namespace Enemy
{
    public class ProjectilePlayer : MonoBehaviour
    {
        public AudioSource woosh;
        public AudioSource hit;

        private Vector3 direction;

        public void Setup(Vector3 dir)
        {
            direction = dir;
            woosh.Play();
            Destroy(gameObject, 2f);
        }

        private void Update()
        {
            float speed = 5;
            transform.position += direction * (Time.deltaTime * speed);
        }

        private void OnTriggerEnter(Collider collider)
        {
            hit.Play();
            if (collider.tag != "BalloonPad" && collider.tag != "Grass" && collider.tag != "Player")
            {
                Destroy(gameObject);
            }
        }
    }
}