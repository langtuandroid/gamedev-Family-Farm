using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev.Managers;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class TapKetHang : MonoBehaviour
    {
        [Inject] private EffectManager _effectManager;
        [SerializeField] private GameObject _stackPrefab;
  
        public void Stack(Cart cart)
        {
            float sums = 0;
            List<GameObject> items = cart.cart;
            int numbers = cart.cart.Count ;
            foreach (GameObject item in items)
            {
                sums += item.GetComponent<Product>().Goods.Income;
            }

            foreach (GameObject item in items)
            {
                Destroy(item);
            }
            items.Clear();

            StartCoroutine(RefundRoutine(sums,numbers));
        }
        private IEnumerator RefundRoutine(float sums,int numbers)
        {
            for (int i = 0; i < numbers; i++)
            {
                yield return null;
                _effectManager.MoneyEffect(sums /( numbers*2), GameObject.FindGameObjectWithTag("Player").transform);
            }
        }
    }
}
 
