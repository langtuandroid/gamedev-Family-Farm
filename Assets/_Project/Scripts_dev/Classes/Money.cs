using System.Collections;
using _Project.Scripts_dev.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class Money : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [FormerlySerializedAs("pos")] [SerializeField] public GameObject[] _positions;
        [SerializeField] public GameObject _moneyPrefab;
        private bool _isUpdating;
        public float Quantity { get; set; }
        public Vector3 NextPosition { get; private set; }
        public Vector3 PreviousPosition { get; private set; }
        public int NewIndex { get; private set; }
        
       
        private void Awake()
        {
            foreach (GameObject go in _positions)
            {
                GameObject clone = Instantiate(_moneyPrefab, go.transform);
                clone.transform.position = go.transform.position;
                clone.SetActive(false);
            }
            NextPosition = _positions[0].transform.position;
            PreviousPosition = _positions[0].transform.position;
        
        }
        private void Update()
        {
            UpdateView();
            if (Quantity < 0) Quantity = 0;
       
        }
        private void UpdateView()
        {
            StartCoroutine(ActiveRoutine());
            if (Mathf.Ceil(Quantity / _gameManager.MoneyInPack) < _positions.Length)
                Delay();
        }
        public void UpdatePosition(bool add)
        {
            if(NewIndex<_positions.Length)
                NextPosition = _positions[NewIndex].transform.position;
            if(NewIndex>0)
                PreviousPosition = _positions[NewIndex-1].transform.position;
            if (add)
                NewIndex++;
            else if (Mathf.Ceil(Quantity / _gameManager.MoneyInPack) <= _positions.Length)
            {
                Debug.Log("Whu bug wtf man");
                NewIndex--;
            }
           
            if (NewIndex >= _positions.Length)
            {
                NewIndex = _positions.Length - 1;
            }
            if (NewIndex < 0)
            {
                NewIndex = 0;
            }
        }
        public void CalculatePositions()
        {
            for(int i=0; i < _positions.Length; i++)
            {
                if (!_positions[i].transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    NewIndex = i ;
                    break;
                }
            }
        }
        private IEnumerator ActiveRoutine()
        {
            for (int i = 0; i < (Quantity / _gameManager.MoneyInPack > _positions.Length ? _positions.Length :Mathf.Ceil(Quantity / _gameManager.MoneyInPack)); i++)
            {
                _positions[i].transform.GetChild(0).gameObject.SetActive(true);
                yield return null;
            }
        }
        private void Delay()
        {
            for (int i =(int) (Mathf.Ceil(Quantity / _gameManager.MoneyInPack)); i < _positions.Length; i++)
            {
                if (_positions[i].transform.childCount>0)
                    _positions[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
