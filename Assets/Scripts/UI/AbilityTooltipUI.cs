using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityTooltipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TitleText;
    [SerializeField] TextMeshProUGUI DescriptionText;

    public void Show(string title, string description)
    {
        this.gameObject.SetActive(true);
        TitleText.text = title;
        DescriptionText.text = description;
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

}
