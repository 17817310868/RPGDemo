using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResLoadPanel : MonoBehaviour
{

    public Text text;
    public Slider slider;

    // Start is called before the first frame update
    private void Awake()
    {
        EventManager.Instance.AddListener<string>("UpdateResLoadText", (t) =>
        {
            text.text = t;
        });
        EventManager.Instance.AddListener<float>("UpdateResLoadProgress", (p) =>
         {
             slider.value = p;
         });
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
