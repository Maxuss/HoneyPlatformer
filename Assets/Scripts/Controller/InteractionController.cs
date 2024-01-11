using Objects;
using Program;
using UnityEngine;

namespace Controller
{
    public class InteractionController: MonoBehaviour
    {
        [SerializeField]
        private Transform handTransform;
        [SerializeField]
        private LayerMask interactLayer;
        
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;
            
            var hit = Physics2D.Raycast(
                handTransform.position,
                PlayerController.Instance.facingDirection == PlayerController.FacingDirection.Left ? Vector3.left : Vector3.right,
                0.1f, interactLayer
            );
            
            var isNotNull = hit.collider != null;
            switch (isNotNull)
            {
                case true when hit.collider.gameObject.TryGetComponent<IInteractable>(out var interactable):
                    Debug.Log("INTERACTABLE");
                    interactable.OnInteract();
                    break;
            }
        }
    }
}