using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCSelectOverview : MonoBehaviour
{
    [Header("Color settings")]
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color inactiveColor;

    [Header("References")]
    [SerializeField] private NPCSelectScroller scroller;

    [Header("Prefabs")]
    [SerializeField] private GameObject iconPrefab;

    // GameManager instance for easy access
    private GameManager gm = GameManager.gm;
    private List<CharacterIcon> icons = new();
    private int selectedCharacter = -1;

    void Start()
    {
        // Get character spaces
        foreach (var child in scroller.Children)
        {
            var character = child.GetComponentInChildren<SelectOption>().character;
            
            // Instantiate new character icon
            var iconInstantiation = Instantiate(iconPrefab, transform);
            var background = iconInstantiation.GetComponent<Image>();

            // Set appropriate background color
            background.color = character.isActive ?
                defaultColor : inactiveColor;

            // Add to list of icons
            var icon = new CharacterIcon(iconInstantiation, background, character);
            icons.Add(icon);
        }

        scroller.OnCharacterSelected.AddListener(SelectCharacter);
    }

    private void SelectCharacter()
    {
        // If there is no previously selected character, skip this
        if (selectedCharacter >= 0)
        {
            var prevIcon = icons[selectedCharacter];
            prevIcon.background.color = prevIcon.character.isActive ?
                defaultColor : inactiveColor;
        }

        selectedCharacter = scroller.SelectedChild;
        var icon = icons[selectedCharacter];
        icon.background.color = selectedColor;        
    }

    struct CharacterIcon
    {
        public CharacterIcon(GameObject gameObject, Image background, CharacterInstance character)
        {
            this.gameObject = gameObject;
            this.background = background;
            this.character = character;

            // Set the character's icon to their face
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = character.avatar;
        }

        public GameObject gameObject;
        public Image background;
        public CharacterInstance character;
    }
}

