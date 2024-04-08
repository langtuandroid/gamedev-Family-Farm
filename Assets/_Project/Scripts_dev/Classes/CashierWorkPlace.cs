using UnityEngine;

namespace _Project.Scripts_dev.Classes
{
    public class CashierWorkPlace : MonoBehaviour
    {
        [SerializeField] Cashier cashier;
        public GameObject currentCashier;
        private void Update()
        {
            if (currentCashier == null || currentCashier.activeInHierarchy) return;
            currentCashier = null;
            cashier.cashierIsHere = false;
            cashier.playerIsHere = false;
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Farmer"))
            {
                cashier.cashierIsHere = true;
                currentCashier = other.gameObject;
            }
            if(other.CompareTag("Player"))
            {
                cashier.playerIsHere = true;
                currentCashier = other.gameObject;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == currentCashier)
            {
                currentCashier = null;
                cashier.cashierIsHere = false;
                cashier.playerIsHere = false;
            }
        }
    }
}
