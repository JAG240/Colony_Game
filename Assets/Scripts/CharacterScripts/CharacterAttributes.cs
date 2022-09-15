using UnityEngine;
using UnityEngine.UI;

public class CharacterAttributes : MonoBehaviour
{
    private NameReader nameReader;
    public Character charClass;
    private Canvas canvas;
    private new Text name;

    #region Reead only vars
    //These are for read only purposes 
    [SerializeField] private float chopSkill;
    [SerializeField] private float mineSkill;
    [SerializeField] private float buildSkill;
    #endregion

    private void Awake()
    {
        nameReader = GameObject.Find("Reader").GetComponent<NameReader>();

        charClass = new Character();
        canvas = GetComponentInChildren<Canvas>();
        name = canvas.GetComponentInChildren<Text>();

        charClass.Name = nameReader.GetName();
        name.text = charClass.Name;
        gameObject.name = charClass.Name;
        SetCharacterAttributes();
    }

    void Start()
    {
        chopSkill = charClass.Skills[ObjectTaskScript.TaskType.chop];
        mineSkill = charClass.Skills[ObjectTaskScript.TaskType.mine];
        buildSkill = charClass.Skills[ObjectTaskScript.TaskType.build];
    }

    private void SetCharacterAttributes()
    {
        charClass.Skills[ObjectTaskScript.TaskType.chop] = Random.Range(0.5f, 2f);
        charClass.Skills[ObjectTaskScript.TaskType.mine] = Random.Range(0.5f, 2f);
        charClass.Skills[ObjectTaskScript.TaskType.build] = Random.Range(0.5f, 2f);
        charClass.Attributes["carryWeight"] = Random.Range(0.5f, 2f);
    }
}
