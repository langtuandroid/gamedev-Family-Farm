using System.Collections;
using System.Collections.Generic;
using _Project.Scripts_dev.Managers;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Project.Scripts_dev.Classes
{
    public class Cart : MonoBehaviour
    {
        [Inject] private SoundManager _soundManager;
        public List<GameObject> cart;
        public int inCart;

        public GameObject[] CartPos;
        private List<string> objectUsingCart;

        private bool taking;
        private void Start()
        {
            objectUsingCart = new List<string>();
        }
        private void UpdateCartNumber()
        {
            inCart = cart.Count;
        }
        public void AiAdd(Shelf stack,int need)
        {
            
            if (!objectUsingCart.Contains(name)&&!taking)
            {
                StartCoroutine(DelayAiAdd(stack,need));
            }
        }
        private IEnumerator DelayAiAdd(Shelf stack,int need)
        {
            taking = true;
       
            objectUsingCart.Add(name);
            for (int i = 0; i < (need>CartPos.Length?CartPos.Length:need); i++)
            {
            
                if (stack.Quantity <= 0) break;
                GameObject pos = CartPos[i];
           
                if (pos.transform.childCount == 0)
                {
                    GameObject clone = Instantiate(stack._productsToDisplay.ItemPrefab, stack.transform.position, pos.transform.rotation);
                    clone.transform.parent = pos.transform;
                    cart.Add(clone);
                    ParabolicMovement(clone, CartPos[i].transform.position,0.5f, 1.5f, () => { clone.transform.DOLocalMove(Vector3.zero, 0.1f); });
                    stack.Quantity--;
                    _soundManager.CreateSound(_soundManager.Clips[7], transform.position, 1f);
                    _soundManager.CreateSound(_soundManager.Clips[7], transform.position, 1f);
                    yield return new WaitForSeconds(0.05f);
                }

                UpdateCart();
            }
            objectUsingCart.Remove(name);
            taking = false;
       
        }
        public void FarmerAdd(Stack stack, int need)
        {
            if (!objectUsingCart.Contains(name)&&!taking)
                StartCoroutine(DelayFarmerAdd(stack, need));
        
        }
        private IEnumerator DelayFarmerAdd(Stack stack, int need)
        {
            taking = true;
            objectUsingCart.Add(name);
            for (int i =0; i < (need > CartPos.Length ? CartPos.Length : need); i++)
            {
                if (stack.currentQuantity <= 0) break;
                GameObject pos = CartPos[i];

                if (pos.transform.childCount == 0)
                {
                    GameObject clone = Instantiate(stack.productToShow.ItemPrefab, stack.transform.GetChild(0).position, pos.transform.rotation);
                    clone.transform.parent = pos.transform;
                    cart.Add(clone);
                    ParabolicMovement(clone, CartPos[i].transform.position,0.5f, 1.5f, () => { clone.transform.DOLocalMove(Vector3.zero, 0.1f); });
                    stack.currentQuantity--;
                    _soundManager.CreateSound(_soundManager.Clips[7], transform.position, 1f);
                    _soundManager.CreateSound(_soundManager.Clips[7], transform.position, 1f);
                    yield return new WaitForSeconds(0.05f);
                }

                UpdateCart();
            }
            objectUsingCart.Remove(name);
            taking = false;
        }
        public void Add(Stack stack,int maxCart)
        {
        
            if (!objectUsingCart.Contains(name)&&!taking)
            {
                StartCoroutine(DelayAdd(stack,maxCart));

            }
        }
        private IEnumerator DelayAdd(Stack stack,int maxCart)
        {
            taking = true;
            objectUsingCart.Add(name);
            for (int i = 0; i <maxCart; i++)
            {
                if (stack.currentQuantity <= 0) break;
                GameObject pos = CartPos[i];
                if (pos.transform.childCount == 0)
                {
                    GameObject clone = Instantiate(stack.productToShow.ItemPrefab, stack.transform.position, pos.transform.rotation);
                    clone.transform.parent = pos.transform;
                    cart.Add(clone);
                    ParabolicMovement(clone, CartPos[i].transform.position,0.3f, 1.5f, () => { clone.transform.DOLocalMove(Vector3.zero, 0.1f); });
                    stack.currentQuantity--;
                    _soundManager.CreateSound(_soundManager.Clips[7], transform.position, 1f);
                    _soundManager.CreateSound(_soundManager.Clips[7], transform.position, 1f);
                    yield return new WaitForSeconds(0.05f);
                }

                UpdateCart();
            }
            objectUsingCart.Remove(name);
            taking = false;
        }
        private void ParabolicMovement(GameObject go, Vector3 targetPosition, float duration, float height, TweenCallback OnComplete)
        {
            Vector3[] path = new Vector3[3];
            path[0] = go.transform.position;
            path[1] = (targetPosition + go.transform.position) / 2 + new Vector3(0, 2 * height, 0);
            path[2] = targetPosition;

            go.transform.DOPath(path, duration, PathType.CatmullRom, PathMode.Full3D, 10, Color.red)
                .SetEase(Ease.OutQuad)
                .OnComplete(OnComplete);
        }
  

        public bool  Remove(string name,int id, MaterialPlace matPlace, int max, bool destroy = false, Shelf shelf = null)
        {
     
            if (!objectUsingCart.Contains(name)&&!taking)
            {
                StartCoroutine(RemoveInterval(name,id, matPlace,max,destroy,shelf));
                return true;
            }
            return false;
        }
        private bool RemoveAnim(int id, MaterialPlace matPlace, bool destroy,Shelf shelf=null)
        {
       
            for (int i = 0; i < matPlace._positions.Length; i++)
            {
                if (matPlace._positions[i].transform.childCount == 0||destroy)
                {
                    for (int j = 0; j < (cart.Count); j++)
                    {


                        if (cart[j].GetComponent<Product>().Goods.Id == id)
                        {
                            ParabolicMovement(cart[j], matPlace._positions[i].transform.position,0.75f, 1.5f, () => {
                                if (destroy) Destroy(matPlace._positions[i].transform.GetChild(0).gameObject);
                                matPlace.UpdateMat();
                                if (shelf != null) shelf.Quantity++;
                            });
                        
                            cart[j].transform.parent = matPlace._positions[i].transform;
                            cart[j].transform.rotation = matPlace._positions[i].transform.rotation;
                            cart[j].transform.Rotate(0, 0, 90);
                            matPlace.CurrentQuantity++;
                            cart.Remove(cart[j]);
                            UpdateCart();


                            matPlace.UpdateMat();
                            return true;
                        }
                    }
                }
            
                matPlace.UpdateMat();
                if (i == matPlace._positions.Length-1) return false;
            }
       
            return true;
        }
        private IEnumerator RemoveInterval(string name,int id, MaterialPlace matPlace,int max, bool destroy,Shelf shelf)
        {
            taking = true;
            objectUsingCart.Add(name);
            bool goOn=true;
            int feedNumber=0;
            while (goOn && feedNumber < max)
            {
                feedNumber++;
                goOn = false;

                yield return new WaitForSeconds(0.05f);
                _soundManager.CreateSound(_soundManager.Clips[8], transform.position, 1f);
                if ( RemoveAnim(id, matPlace, destroy, shelf))
                {

                    foreach (GameObject go in cart)
                    {
                        if (go.GetComponent<Product>().Goods.Id == id)
                        {
                            goOn = true;
                            break;
                        }

                    }
                }
            
           
            }
            objectUsingCart.Remove(name);
            taking = false;
        }
        private void UpdateCart()
        {
            for (int i = 0; i < CartPos.Length; i++)
            {
                if (CartPos[i].transform.childCount == 0)
                {
                    for (int j = i + 1; j < CartPos.Length; j++)
                    {
                        if (CartPos[j].transform.childCount > 0)
                        {
                            GameObject child = CartPos[j].transform.GetChild(0).gameObject;
                            child.transform.parent = CartPos[i].transform;
                            child.transform.position = CartPos[i].transform.position;
                            break;
                        }
                    }
                }
            }
            UpdateCartNumber();
        }
        public void Clear()
        {
            cart.Clear();
            foreach(GameObject go in CartPos)
            {
                if (go.transform.childCount > 0)
                {
                    Destroy(go.transform.GetChild(0).gameObject);
                
                }
            }
            UpdateCartNumber();
        }
    
    }
}
