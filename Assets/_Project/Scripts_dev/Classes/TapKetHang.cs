using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class TapKetHang : MonoBehaviour
    {
        [Inject] private EffectManager _effectManager;
        [SerializeField] private GameObject stackPrefab;
  
        public void CreateStacks(Cart cart)
        {
            float sums = 0;
      
            List<GameObject> items = cart.cart;
            int numbers = cart.cart.Count ;
            foreach (GameObject item in items)
            {

                sums += item.GetComponent<Product>().goods.income;
            }

            foreach (GameObject item in items)
            {
                Destroy(item);
            }
            items.Clear();

            StartCoroutine(DelayRefund(sums,numbers));
        }
        IEnumerator DelayRefund(float sums,int numbers)
        {
            for (int i = 0; i < numbers; i++)
            {
                yield return null;
                _effectManager.GetMoneyEffect(sums /( numbers*2), GameObject.FindGameObjectWithTag("Player").transform);
            }
        }
    }
}
 
