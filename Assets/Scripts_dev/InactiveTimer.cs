using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InactiveTimer : MonoBehaviour
{
    public Image image,bg;
    public TextMeshProUGUI text;
    public Image adIcon;

    public float time;
    public bool play;
    public GameObject priorityCanvas;

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("RemoveAds", 0) == 1)
        {

            GameManager.instance.playTimer = 0;
            GameManager.instance.inactiveTimer = 0;
            image.fillAmount = 1f;
            GameManager.instance.interTimer = 0;
            adIcon.gameObject.SetActive(false);
            bg.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            return;
        }
        if (!play)
        {
            if (GameManager.instance.inactiveTimer >= time - 5 && (GameManager.instance.draggable) )
            {
                GameManager.instance.playTimer =time;
                bg.gameObject.SetActive(true);
                image.gameObject.SetActive(true);
                text.gameObject.SetActive(true);
                adIcon.gameObject.SetActive(true);
                image.fillAmount = 1 - (GameManager.instance.inactiveTimer - (time - 5)) / 5;
                text.text = ((int)(time - GameManager.instance.inactiveTimer) + 1).ToString();
            }
            else
            {
                image.fillAmount = 1f;

                adIcon.gameObject.SetActive(false);

                bg.gameObject.SetActive(false);
                image.gameObject.SetActive(false);
                text.gameObject.SetActive(false);


            }
        }
        
        else
        {
            if (GameManager.instance.playTimer >= time - 5&& (GameManager.instance.draggable))
            {
                bg.gameObject.SetActive(true);
                image.gameObject.SetActive(true);
                text.gameObject.SetActive(true);
                adIcon.gameObject.SetActive(true);
                image.fillAmount = 1 - (GameManager.instance.playTimer - (time - 5)) / 5;
                text.text = ((int)(time - GameManager.instance.playTimer) + 1).ToString();
            }
            else
            {
                image.fillAmount = 1f;

                adIcon.gameObject.SetActive(false);

                bg.gameObject.SetActive(false);
                image.gameObject.SetActive(false);
                text.gameObject.SetActive(false);


            }
        }
        if (image.fillAmount <= 0f)
        {
            GameManager.instance.playTimer = 0;
            GameManager.instance.inactiveTimer = 0;
            image.fillAmount = 1f;
            GameManager.instance.interTimer = 0;
            adIcon.gameObject.SetActive(false);
            bg.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            text.gameObject.SetActive(false);

        }
    }
}
