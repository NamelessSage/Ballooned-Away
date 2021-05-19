using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class Wall : MonoBehaviour
    {
        public Slider slider;
        GameObject thisObject;

        public int Health = 3;
        private bool dead = false;

        void Start()
        {
            setmaxHealt(Health);
               thisObject = transform.gameObject;
        }

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
            Destroy(gameObject.transform.parent.gameObject);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Projectile"))
            {
                Health--;
                takedamage();
            }
        }


        private void setmaxHealt(int health)
        {
            
            slider.maxValue = health;
            slider.value = health;
        }
        private void setHealt(int healt)
        {
            slider.value = healt;
        }

        private void takedamage()
        {
            setHealt(Health);

        }

    }


}
