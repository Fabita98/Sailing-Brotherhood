using Unity.Netcode;
using UnityEngine;

namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{

    public class ClientNetworkRigidbody : NetworkBehaviour
    {
        private Rigidbody rb;
        private Vector3 targetPosition;
        private Quaternion targetRotation;

        [SerializeField] private float lerpRate = 10f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                rb.isKinematic = false;
            }
            else
            {
                rb.isKinematic = false;
            }
        }

        private void FixedUpdate()
        {
            if (!IsOwner)
            {
                // Interpolate the position and rotation from the server's values
                rb.position = Vector3.Lerp(rb.position, targetPosition, lerpRate * Time.fixedDeltaTime);
                rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, lerpRate * Time.fixedDeltaTime);
            }
        }

        public void SetTransform(Vector3 position, Quaternion rotation)
        {
            targetPosition = position;
            targetRotation = rotation;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SyncTransformServerRpc(Vector3 position, Quaternion rotation)
        {
            // Update the target position and rotation on the server
            SetTransform(position, rotation);

            // Sync the position and rotation to all clients
            SyncTransformClientRpc(position, rotation);
        }

        [ClientRpc]
        public void SyncTransformClientRpc(Vector3 position, Quaternion rotation)
        {
            if (!IsOwner)
            {
                // Update the target position and rotation on clients
                SetTransform(position, rotation);
            }
        }
    }
}
