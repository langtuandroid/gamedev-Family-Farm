using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts_dev.UI
{
    public class Loading : MonoBehaviour
    {
        [SerializeField] private Image _loadImage;
        private void Start()
        {
            StartCoroutine(LoadingCoroutine());
            StartCoroutine(WaitRoutine());
        }
        private IEnumerator WaitRoutine()
        {
            yield return new WaitForSeconds(2);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        }
        private IEnumerator LoadingCoroutine()
        {
            float timeLoad = 0f;
            while (timeLoad < 5)
            {
                if (timeLoad < 4)
                {
                    yield return new WaitForEndOfFrame();
                    timeLoad += Time.time / 4;
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    timeLoad = 5f;
                }
                _loadImage.fillAmount = timeLoad / 5;
            }
            SceneManager.LoadSceneAsync(1);
        }
    }
}
