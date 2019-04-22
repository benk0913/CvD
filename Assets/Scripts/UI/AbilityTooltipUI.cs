using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityTooltipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TitleText;
    [SerializeField] TextMeshProUGUI DescriptionText;
    [SerializeField] Transform AttributesContainer;
    [SerializeField] GameObject AttributePrefab;

    public void Show(string title = "", string description = "", List<string> attributes = null)
    {
        this.gameObject.SetActive(true);
        TitleText.text = title;
        DescriptionText.text = description;

        ClearAttributes();

        if(attributes != null)
        {
            GameObject tempAttribute;
            for(int i=0;i<attributes.Count;i++)
            {
                tempAttribute = ResourcesLoader.Instance.GetRecycledObject(AttributePrefab);
                tempAttribute.transform.SetParent(AttributesContainer, false);
                tempAttribute.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = attributes[i];
            }
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    void ClearAttributes()
    {
        while(AttributesContainer.childCount > 0)
        {
            AttributesContainer.GetChild(0).gameObject.SetActive(false);
            AttributesContainer.GetChild(0).transform.SetParent(transform);
        }
    }
}
