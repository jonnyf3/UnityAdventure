using UnityEngine;
using RPG.Characters;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum PortalIdentifier { None, A, B, C }

        [Header("Info")]
        [SerializeField] PortalIdentifier identifier = PortalIdentifier.A;
        [SerializeField] Transform spawnPoint = null;

        [Header("Target")]
        [SerializeField] string sceneToLoad = "";   //TODO write a custom editor to create a dropdown based on Build Settings?
        [SerializeField] PortalIdentifier targetPortal = PortalIdentifier.None;
        
        private void OnTriggerEnter(Collider other) {
            var player = other.GetComponent<Player>();
            if (!player) { return; }

            player.StopControl();
            UsePortal();
        }

        private void UsePortal() {
            DontDestroyOnLoad(gameObject);
            SceneController.onLevelLoaded += OnTargetLevelLoaded;

            SceneController.LoadLevel(sceneToLoad);
        }
        private void OnTargetLevelLoaded() {
            var target = GetTargetPortal();
            if (target != null) { SpawnPlayerAtPosition(target.spawnPoint); }

            SceneController.onLevelLoaded -= OnTargetLevelLoaded;
            Destroy(gameObject);
        }

        private Portal GetTargetPortal() {
            if (targetPortal == PortalIdentifier.None) { return null; }

            foreach(var portal in FindObjectsOfType<Portal>()) {
                if (portal != this && portal.identifier == targetPortal) { return portal; }
            }
            return null;
        }
        private void SpawnPlayerAtPosition(Transform target) {
            var player = FindObjectOfType<Player>();
            player.transform.position = target.position;
            player.transform.forward = target.forward;
        }
    }
}