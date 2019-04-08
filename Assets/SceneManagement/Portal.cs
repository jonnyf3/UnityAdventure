using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Characters;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum PortalIdentifier { None, A, B, C }

        [Header("Info")]
        [SerializeField] PortalIdentifier identifier;
        [SerializeField] Transform spawnPoint;

        [Header("Target")]
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] PortalIdentifier targetPortal;
        
        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Player>()) { StartCoroutine(ChangeScene()); }
        }

        private IEnumerator ChangeScene() {
            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            var target = GetTargetPortal();
            if (target != null) { SpawnPlayerAtPosition(target.spawnPoint); }
            
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