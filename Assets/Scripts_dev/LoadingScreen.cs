using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image Loading_2_Image;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadingCoroutine());
        StartCoroutine(Delay());
    }
    IEnumerator Delay()
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
            Loading_2_Image.fillAmount = timeLoad / 5;
        }



        SceneManager.LoadSceneAsync(1);


    }
}
