using _Project.Scripts_dev.Classes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts_dev.Farm
{
    public class ShelfCounter : MonoBehaviour
    {
        [FormerlySerializedAs("text")] [SerializeField] private TextMeshProUGUI _text;
        [FormerlySerializedAs("shelf")] [SerializeField] private Shelf _shelf;

        private void Update()
        {
            _text.text ="x"+ _shelf.Quantity;
        }
    }
}
