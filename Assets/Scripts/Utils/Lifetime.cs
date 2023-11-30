using UnityEngine;

namespace Utils
{
    public class Lifetime : MonoBehaviour
    {
        [SerializeField] private float destroyTime = 2f;
    
    
        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject,destroyTime);
        }

    
    }
}
