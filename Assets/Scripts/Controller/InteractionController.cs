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
            Debug.Log($"INPUT {Input.GetKeyDown(KeyCode.E)}");
            if (!Input.GetKeyDown(KeyCode.E)) return;
            
            Debug.Log("UPDATING");
            var hit = Physics2D.Raycast(
                handTransform.position,
                PlayerController.Instance.facingDirection == PlayerController.FacingDirection.Left ? Vector3.left : Vector3.right,
                0.1f, interactLayer
            );
            
            Debug.Log($"HIT {hit} {hit.collider}");

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