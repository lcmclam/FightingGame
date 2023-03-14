using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI slidervalue;
    // Start is called before the first frame update
    void Start()
    {
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat("roundTimer", 100));
        slidervalue.text = slider.value.ToString();

        slider.onValueChanged.AddListener((value) => {
            slidervalue.text = value.ToString();
            PlayerPrefs.SetFloat("roundTimer", value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
