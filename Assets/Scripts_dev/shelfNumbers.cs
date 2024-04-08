using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class shelfNumbers : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Shelf shelf;
    // Start is called before the first frame update
  
    // Update is called once per frame
    void Update()
    {
        text.text ="x"+ shelf.currentQuantity.ToString();
    }
}
