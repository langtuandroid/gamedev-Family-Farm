using UnityEngine;

namespace _Project.Scripts_dev.Classes
{
    public class CashierWorkPlace : MonoBehaviour
    {
        [SerializeField] private Cashier _cashier;
        private GameObject _currentCashier;
        private void Update()
        {
            if (_currentCashier == null || _currentCashier.activeInHierarchy) return;
            _currentCashier = null;
            _cashier.cashierIsHere = false;
            _cashier.playerIsHere = false;
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Farmer"))
            {
                _cashier.cashierIsHere = true;
                _currentCashier = other.gameObject;
            }
            if(other.CompareTag("Player"))
            {
                _cashier.playerIsHere = true;
                _currentCashier = other.gameObject;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == _currentCashier)
            {
                _currentCashier = null;
                _cashier.cashierIsHere = false;
                _cashier.playerIsHere = false;
            }
        }
    }
}
