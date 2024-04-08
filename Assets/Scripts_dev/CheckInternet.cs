using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInternet : MonoBehaviour
{
    public float checkInterval = 5f; // Thời gian giữa các lần kiểm tra (đơn vị: giây)

    private void Start()
    {
        StartCoroutine(CheckInternetConnectionRepeatedly());
    }

    private System.Collections.IEnumerator CheckInternetConnectionRepeatedly()
    {
        while (true)
        {
            CheckInternetConnection();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void CheckInternetConnection()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            noInternetUI.SetActive(false);
        }
        else
        {
            noInternetUI.SetActive(true);
        }
    }
    public GameObject noInternetUI;
   

   
}
