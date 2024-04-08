using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

using TMPro;
using DG.Tweening;

public class Unlock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] GameObject unlock;
    [SerializeField] float exp;
 
    private GameObject virtualCamera;

    public int minLevel;
    public float price;
    public float remain;
    bool unlocked,load;
    public float unlockTime;

    private Collider box;
    private void Start()
    {
        box = GetComponent<Collider>();
        remain = price;
        transform.rotation = Quaternion.identity;
        transform.Rotate(Vector3.up, 90f);
    }
    private void Update()
    {
        if (remain < 1) remain = 0;
        if(unlockTime<2)
        unlockTime += Time.deltaTime;
        if (!load)
        {
            if (GameManager.instance.load)
            {
                if (GameManager.instance.playTime < 3)
                {
                   
                    if (DataManager.instance.gameData != null)
                    {
                        remain = DataManager.instance.gameData.unlockMoney;
                        load = true;
                    }
                }
                else
                {
                    load = true;
                    remain = price;
                }

                if (DataManager.instance.gameData == null) {

                    load = true;
                } 
            }
            
            return;
        }
        if (unlocked) return;
        if (minLevel>0)
        {
            if (GameManager.instance.level < minLevel)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                box.enabled = false;
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(true);
                box.enabled = true;
                minLevel = 0;
                StartCoroutine(GameManager.instance.DelayFocus(transform.gameObject));
            }
        }
        priceText.text =  UIManager.instance.FormatNumber(Mathf.Floor( remain));
        if (remain <= 0)
        {
            StartCoroutine(DelayUnlock());
        }
        else
        {
            unlock.SetActive(false);
           
        }
    }
    IEnumerator DelayUnlock()
    {
        if (unlock.CompareTag("Area"))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.sounds[2]);
        }
        else
            SoundManager.instance.PlaySound(SoundManager.instance.sounds[1]);
        yield return new WaitForSeconds(0.3f);
        
        unlock.SetActive(true);
        unlocked = true;
        DotweenCodes.instance.PopOut(unlock.transform, new Vector3(1, 2f, 1), 0.4f);
        Instantiate(EffectManager.instance.rainbowUnlock, transform.position, EffectManager.instance.rainbowUnlock.transform.rotation);
        if (GameManager.instance.currentUnlocked < GameManager.instance.unlockOrder.Length-1)
        GameManager.instance.currentUnlocked++;
        GameManager.instance.UpdateUnlocked();
        GameManager.instance.currentExp += exp;
        virtualCamera = GameObject.FindGameObjectWithTag("FocusCamera");
        if (virtualCamera != null)
            virtualCamera.SetActive(false);

        gameObject.SetActive(false);
    }

    
}
