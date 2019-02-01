using UnityEngine;
using RPG.Characters;

namespace RPG.Core
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] AudioClip clip;
        [SerializeField] float triggerRadius = 5f;

        [SerializeField] bool isOneTimeOnly = true;
        private bool hasPlayed = false;

        private AudioSource source;
    
        void Start() {
            //Add required components via code at runtime
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = clip;

            var triggerVolume = gameObject.AddComponent<SphereCollider>();
            triggerVolume.radius = triggerRadius;
            triggerVolume.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.attachedRigidbody || other.isTrigger) { return; }
            
            var objectInRange = other.attachedRigidbody.gameObject;
            if (objectInRange.GetComponent<Player>()) {
                PlayAudioClip();
            }
        }

        private void PlayAudioClip() {
            if (isOneTimeOnly && hasPlayed) { return; }

            if (!source.isPlaying) {
                source.Play();
                hasPlayed = true;
            }
        }

        //Visualise trigger area, even though collider is only added at run-time
        //private void OnDrawGizmos() {
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawWireSphere(transform.position, triggerRadius);
        //}
    }
}