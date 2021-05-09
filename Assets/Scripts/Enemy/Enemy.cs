using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        public GameObject proj;
        private GameObject player;
        private GameObject Projectile;

        private void Start()
        {
            player = GameObject.Find("Player");
        }

        private void Update()
        {
            if (Projectile == null && Vector3.Distance(transform.position, player.transform.position) < 3)
            {
                Projectile = Instantiate(proj, new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), Quaternion.identity);
                Vector3 shootDir = (player.transform.position - Projectile.transform.position).normalized;
                shootDir.y = 0;
                Projectile.GetComponent<Projectile>().Setup(shootDir);
                Destroy(Projectile, 3f);
            }
        }
        

    }
}