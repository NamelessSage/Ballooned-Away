using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        public int Health = 3;

        private bool dead = false;
        public ParticleSystem parts;
        public AudioSource sound;

        public GameObject proj;
        private GameObject player;
        private GameObject Projectile;

        private int WaitTime = 5;
        private float PassedTime = 0;

        private void Start()
        {
            player = GameObject.Find("Player");
            PassedTime = WaitTime;
        }

        

        private void Update()
        {
            PassedTime = PassedTime - Time.deltaTime;

            if (PassedTime <= 0 && Projectile == null && Vector3.Distance(transform.position, player.transform.position) < 3)
            {
                Projectile = Instantiate(proj, new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), Quaternion.identity);
                Vector3 shootDir = (player.transform.position - Projectile.transform.position).normalized;
                shootDir.y = 0;
                Projectile.GetComponent<Projectile>().Setup(shootDir);
                Destroy(Projectile, 2f);

                PassedTime = WaitTime;
            }

            if (Health <= 0 && !dead)
            {
                dead = true;
                parts.Play();
                sound.Play();
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
            if (other.CompareTag("PlayerProjectile"))
            {
                Health--;
            }
        }


    }
}


