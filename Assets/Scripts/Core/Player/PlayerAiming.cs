using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class PlayerAiming : NetworkBehaviour
    {

        [Header("References")] 
        [SerializeField] private Transform turretTransform;
        [SerializeField] private InputReader inputReader;
        
        

   

        // Update is called once per frame
        void LateUpdate()
        {
            if(!IsOwner) {return;}

            Vector2 aimScreenPosition = inputReader.MousePosition;
            Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

            turretTransform.up = new Vector2(
                aimWorldPosition.x - turretTransform.position.x,
                aimWorldPosition.y - turretTransform.position.y);



        }
    }
}
