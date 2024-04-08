using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetUpgradeForTab : MonoBehaviour
{
    public List< LevelMangament> levelMangaments;
    public GameObject itemPrefab;
    public Transform contents;
    public int type;

    private void Start()
    {
        levelMangaments = new List<LevelMangament>();
        UpdateList();

    }
    private void OnEnable()
    {
        UpdateList();
    }
    void UpdateList()
    {
        levelMangaments.Clear();
        foreach(Transform t in contents)
        {
            Destroy(t.gameObject);
        }
        LevelMangament[] lm = FindObjectsOfType<LevelMangament>();
        
       /* foreach (LevelMangament l in lm)
        {
            if (l.transform.GetChild(0).gameObject.activeInHierarchy)
                if (l.type == type)
                    levelMangaments.Add(l);
        }*/
        foreach (GameObject l in GameManager.instance.unlockOrder)
        {
            if (l.GetComponent<LevelMangament>()!=null)
                if (l.GetComponent<LevelMangament>().type == type)
                    levelMangaments.Add(l.GetComponent<LevelMangament>());
        }
        if (type != 3)
            levelMangaments.Sort((a, b) => a.goods.id.CompareTo(b.goods.id));
        for(int i=0; i < levelMangaments.Count; i++)
        {
            GameObject item = Instantiate(itemPrefab, contents);
            item.GetComponent<UpgradeItem>().levelMangament = levelMangaments[i];
        }
    }
}
