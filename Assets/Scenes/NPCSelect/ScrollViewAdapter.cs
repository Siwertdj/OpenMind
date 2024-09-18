using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NewBehaviourScript : MonoBehaviour
{
    public Texture2D[] portraits;
    public RectTransform prefab;
    public Text countText;

    public ScrollRect scrollView;
    public RectTransform content;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateItems()
    {
        int newCount;
        int.TryParse(countText.text, out newCount);
        StartCoroutine(FetchItemsFromServer(newCount, results => OnReceivedItems(results)));
    }

    void OnReceivedItems(item[] items)
    {
        
    }
    
    IEnumerator FetchItemsFromServer(int count, Action<item[]> onDone)
    {
        // Simulation of server delay (van de tutorial)
        yield return new WaitForSeconds(2f);

        var results = new item[count];
        for (int i = 0; i < count; i++)
        {
            results[i] = new item();
            results[i].title = "Person " + i;
            results[i].iconIndex = UnityEngine.Random.Range(0, portraits.Length);
        }

        onDone(results);
    }

    public class item
    {
        public string title;
        public int iconIndex;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
