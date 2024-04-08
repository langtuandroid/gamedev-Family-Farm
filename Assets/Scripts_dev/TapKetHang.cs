using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TapKetHang : MonoBehaviour
{
    public Transform[] pos;

    [SerializeField] GameObject stackPrefab;
    private void Start()
    {

    }
    private void Update()
    {

    }
   /* public void CreateStacks(Cart cart)
    {
        List<GameObject> items = cart.cart;
        List<GoodsAmount> goods = new List<GoodsAmount>();
        foreach (GameObject item in items)
        {
            GoodsAmount ga = new GoodsAmount(item.GetComponent<Farm.Product>().goods, items.Count(i => i.GetComponent<Farm.Product>().goods.id == item.GetComponent<Farm.Product>().goods.id));
            if (!goods.Contains(ga))
            {
                goods.Add(ga);
            }
        }
        for (int i = 0; i < goods.Count; i++)
        {
            if (pos[i].childCount > 0)
            {
                Destroy(pos[i].GetChild(0).gameObject);
            }
            GameObject stackGO = Instantiate(stackPrefab, pos[i].position, Quaternion.identity);


            stackGO.transform.parent = pos[i].transform;
            Stack stack = stackGO.GetComponent<Stack>();
            stack.productToShow = goods[i].goods;
            stack.currentQuantity = goods[i].quantity;


        }
        foreach (GameObject item in items)
        {
            Destroy(item);
        }
        // items = null;
        cart.Clear();
        goods = null;
    }
*/
    public void CreateStacks(Cart cart)
    {
        float sums = 0;
      
        List<GameObject> items = cart.cart;
        int numbers = cart.cart.Count ;
        foreach (GameObject item in items)
        {

            sums += item.GetComponent<Farm.Product>().goods.income;
        }

        foreach (GameObject item in items)
        {
            Destroy(item);
        }
        // items = null;
        items.Clear();

        StartCoroutine(DelayRefund(sums,numbers));
    }
    IEnumerator DelayRefund(float sums,int numbers)
    {
        for (int i = 0; i < numbers; i++)
        {
            yield return null;
            EffectManager.instance.GetMoneyEffect(sums /( numbers*2), GameObject.FindGameObjectWithTag("Player").transform);
        }
    }
}
    public struct GoodsAmount
{
    public Goods goods;
    public int quantity;

    public GoodsAmount(Goods goods, int quantity)
    {
        this.goods = goods;
        this.quantity = quantity;
    }
}