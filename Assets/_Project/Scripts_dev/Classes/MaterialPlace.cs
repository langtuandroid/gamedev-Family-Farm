using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts_dev.Classes
{
    public class MaterialPlace : MonoBehaviour
    {
        public int CurrentQuantity { get; set; }
        [FormerlySerializedAs("pos")] public GameObject[] _positions;
        public void UpdateMat()
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                if (_positions[i].transform.childCount != 0) continue;
                for (int j = i + 1; j < _positions.Length; j++)
                {
                    if (_positions[j].transform.childCount <= 0) continue;
                    GameObject child = _positions[j].transform.GetChild(0).gameObject;
                    child.transform.parent = _positions[i].transform;
                    child.transform.position = _positions[i].transform.position;
                    break;
                }
            }

            foreach (var pos in _positions)
            {
                if (pos.transform.childCount > 0)
                {
                    pos.transform.GetChild(0).localPosition = Vector3.zero;
                }
            }
        }
    }
}
