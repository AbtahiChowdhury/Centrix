using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UpdateLevelInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI levelInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (gameObject.name)
        {
            case "Level1Button":
                SetLevel1Info();
                break;
            case "Level2Button":
                SetLevel2Info();
                break;
            case "Level3Button":
                SetLevel3Info();
                break;
            default:
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        levelInfo.text = "";
    }

    public void SetLevel1Info()
    {
        levelInfo.text = "Level: Samsara\n\n" +
                         "Song: Samsara\n" +
                         "Artist: Xtrullor";
    }

    public void SetLevel2Info()
    {
        levelInfo.text = "Level: Glacier Galaxy\n\n" +
                         "Song: Glacier Galaxy\n" +
                         "Artist: EEK! & Lockyn";
    }

    public void SetLevel3Info()
    {
        levelInfo.text = "Level: Abyss of 7th\n\n" +
                         "Song: Abyss of 7th\n" +
                         "Artist: Nexhend";
    }
}
