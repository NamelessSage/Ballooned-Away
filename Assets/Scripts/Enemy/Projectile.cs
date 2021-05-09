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
            float speed = 1;
            transform.position += direction * (Time.deltaTime * speed);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Player")
            {
                Debug.Log("player");
                Destroy(gameObject, 0f);
            }
            else if (collider.tag != "BalloonPad" && collider.tag != "Grass" && collider.tag != "Enemy")
            {
                Debug.Log("not player : " + collider.tag);
                Destroy(gameObject, 0f);
            }
        }
    }
}