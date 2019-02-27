using UnityEngine;

namespace RPG.Audio
{
    public class Voice : MonoBehaviour
    {
        private new AudioSource audio;

        private void Awake() {
            audio = gameObject.AddComponent<AudioSource>();
        }

        public void PlaySound(AudioClip clip) {
            //TODO assert clip is not empty/identify which sound clip needs setting
            if (!clip) { return; }
            audio.PlayOneShot(clip);
        }
        public void PlaySound(AudioClip[] sounds) {
            if (sounds.Length == 0) { return; }
            var clip = sounds[Random.Range(0, sounds.Length)];
            PlaySound(clip);
        }
    }
}