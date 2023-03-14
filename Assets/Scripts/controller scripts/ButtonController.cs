using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonController : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onOptionClick ()
    {
        
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
    public void nextLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}
