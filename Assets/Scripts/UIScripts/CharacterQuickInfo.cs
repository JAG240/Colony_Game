using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterQuickInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject quickInfo;
    private CharacterAttributes characterAttributes;
    private string Name;
    private GameObject tempQuickInfo;

    void Start()
    {
        Name = GetComponent<Text>().text;
        characterAttributes = GameObject.Find(Name).GetComponent<CharacterAttributes>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tempQuickInfo = Instantiate(quickInfo, new Vector3(transform.position.x, transform.position.y + 100, 0f), Quaternion.identity, transform.root.transform);
        tempQuickInfo.gameObject.transform.Find("Name").GetComponent<Text>().text = Name;
        tempQuickInfo.gameObject.transform.Find("Chop").GetComponent<Text>().text = "Chop: " + characterAttributes.charClass.Skills[ObjectTaskScript.TaskType.chop].ToString("F1");
        tempQuickInfo.gameObject.transform.Find("Mine").GetComponent<Text>().text = "Mine: " + characterAttributes.charClass.Skills[ObjectTaskScript.TaskType.mine].ToString("F1");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tempQuickInfo);
    }
}
