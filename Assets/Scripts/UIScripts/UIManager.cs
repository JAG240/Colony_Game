using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas MainUI;
    [SerializeField] GameObject TaskCharacterList;
    [SerializeField] GameObject Content;
    [SerializeField] GameObject CharacterTaskPanel;
    [SerializeField] GameObject OrdersMenu;
    [SerializeField] GameObject BuildMenu;
    private GameObject[] characters;

    void Start()
    {
        characters = GameObject.FindGameObjectsWithTag("Player");
        GenTaskMenuUI();
    }

    void Update()
    {
        
    }

    public void TaskMenuToggle()
    {
        TaskCharacterList.SetActive(!TaskCharacterList.activeInHierarchy);
        OrdersMenu.SetActive(false);
        BuildMenu.SetActive(false);
    }

    public void OrderMenuToggle()
    {
        OrdersMenu.SetActive(!OrdersMenu.activeInHierarchy);
        TaskCharacterList.SetActive(false);
        BuildMenu.SetActive(false);
    }

    private void GenTaskMenuUI()
    {
        foreach(GameObject character in characters)
        {
            GameObject tempPanel = Instantiate(CharacterTaskPanel, Vector3.one, Quaternion.identity, Content.transform);
            tempPanel.GetComponentInChildren<Text>().text = character.name;
            CharacterTasks tempCharTasks = character.GetComponent<CharacterTasks>();
            tempCharTasks.chopToggle = tempPanel.transform.Find("ChopToggle").GetComponent<Toggle>();
            tempCharTasks.mineToggle = tempPanel.transform.Find("MineToggle").GetComponent<Toggle>();
            tempCharTasks.SetListeners();
        }
    }

    public void BuildMenuToggle()
    {
        BuildMenu.SetActive(!BuildMenu.activeInHierarchy);
        TaskCharacterList.SetActive(false);
        OrdersMenu.SetActive(false);
    }
}