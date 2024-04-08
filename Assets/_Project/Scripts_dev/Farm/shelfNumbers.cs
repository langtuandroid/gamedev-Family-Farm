using _Project.Scripts_dev.Classes;
using TMPro;
using UnityEngine;

namespace _Project.Scripts_dev.Farm
{
    public class shelfNumbers : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] Shelf shelf;
        void Update()
        {
            text.text ="x"+ shelf.currentQuantity.ToString();
        }
    }
}
