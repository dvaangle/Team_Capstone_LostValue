using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo_HealthBar : MonoBehaviour
{
    #region
    [SerializeField]
    private float content_fillAmount;
    [SerializeField]
    private Image healthBar_Content;
    [SerializeField]
    private Text healthBar_ValueText;

    public float MaxValue { get; set; }

    public float Value
    {
        set
        {
            healthBar_ValueText.text = value + "/" + MaxValue;
            content_fillAmount = Map(value, 0, MaxValue, 0, 1);
        }
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleBar();
    }

    private void HandleBar()
    {
        if(content_fillAmount != healthBar_Content.fillAmount)
        {
            healthBar_Content.fillAmount = content_fillAmount;
        }
    }

    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
