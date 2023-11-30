using UnityEngine;

namespace Utils
{
    public class DestroyItselfAtCollision : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Destroy(gameObject);
        }
    }
}
