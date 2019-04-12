using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup cg;

        private void Awake() {
            cg = GetComponent<CanvasGroup>();
            cg.alpha = 0;
        }

        public IEnumerator FadeOut(float fadeDuration) {
            while (cg.alpha < 1) {
                cg.alpha += Time.unscaledDeltaTime / fadeDuration;
                yield return new WaitForEndOfFrame();
            }
        }
        public IEnumerator FadeIn(float fadeDuration) {
            while (cg.alpha > 0) {
                cg.alpha -= Time.unscaledDeltaTime / fadeDuration;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}