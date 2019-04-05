using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Characters;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] int sceneToLoad = -1;

        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Player>()) { StartCoroutine(ChangeScene()); }
        }

        private IEnumerator ChangeScene() {
            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            print("Scene finished loading!");
            Destroy(gameObject);
        }
    }
}