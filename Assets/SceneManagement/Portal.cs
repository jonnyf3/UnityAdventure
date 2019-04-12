using UnityEngine;
using RPG.Characters;
using System;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        public enum PortalIdentifier { None, A, B, C }

        //displayed by custom editor
        public PortalIdentifier identifier = PortalIdentifier.A;
        public string sceneToLoad = "";
        public PortalIdentifier targetPortal = PortalIdentifier.None;
        //set on prefab (can be accessed via Debug view if necessary)
        [SerializeField] Transform spawnPoint = null;
        
        private SceneController sc;

        private void OnTriggerEnter(Collider other) {
            var player = other.GetComponent<Player>();
            if (!player) { return; }

            player.StopControl();
            GetComponentInChildren<ParticleSystem>().Play();
            UsePortal();
        }

        private void UsePortal() {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            sc = FindObjectOfType<SceneController>();
            sc.onLevelLoaded += OnTargetLevelLoaded;

            sc.LoadLevel(sceneToLoad);
        }
        private void OnTargetLevelLoaded() {
            var target = GetTargetPortal();
            if (target != null) { SpawnPlayerAtPosition(target.spawnPoint); }

            sc.onLevelLoaded -= OnTargetLevelLoaded;
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