using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerContainerUI : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public Image healthbarfill;
    public Image chargebarfill;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateScoreText (int score)
    {
        scoreText.text = score.ToString();
    }

    public void updateHealthBar (int curHp, int maxHp)
    {
        healthbarfill.fillAmount = (float)curHp / (float)maxHp;
    }


    public void updateChargeBar(float chargeDmg, float maxChargeDmg)
    {
        chargebarfill.fillAmount = chargeDmg / maxChargeDmg;
    }

    public void initialize(Color color)
    {
        scoreText.color = color;
        healthbarfill.color = color;
        scoreText.text = "0";
        healthbarfill.fillAmount = 1;
        chargebarfill.fillAmount = 0;


    }

}
