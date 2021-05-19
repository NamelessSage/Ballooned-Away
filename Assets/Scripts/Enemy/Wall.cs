using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Wall : MonoBehaviour
    {
        public int Health = 3;
        private bool dead = false;

        private void Update()
        {
            if (Health <= 0 && !dead)
            {
                dead = true;
                StartCoroutine(breaker());
            }
        }
        private IEnumerator breaker()
        {
            yield return new WaitForSeconds(1);
            Destroy(gameObject);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Projectile"))
            {
                Health--;
            }
        }
        

    }
}