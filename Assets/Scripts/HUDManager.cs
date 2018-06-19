using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    //Action Menu
    public GameObject unitPanel, selectionMenuPanel, cancelMenuPanel, actionMenuPanel;

    //Turn Character Information
    public Image characterImage;
    public Text levelText, nameText, healthText, dataStreamText;
    public Slider healthSlider, dataSlider;

    //Target Character Information

    //Damage & Hit Preview Box

    private void Start()
    {
        if (LevelManager._instance.hud == null)
            LevelManager._instance.hud = this;
        else
            Destroy(this);
    }

    //Atualiza a HUD com as informações do personagem do turno atual
    public void UpdateTurnCharacter(Unit u)
    {
        characterImage.sprite = u.portraitSprite;
        levelText.text = "LV " + u.level.ToString("00");
        nameText.text = u.characterName;
        healthText.text = u.healthCurrent.ToString() + " / " + u.healthMax.ToString();
        //dataStreamText.text = u.healthMax.ToString();                         //Data Stream Points
        healthSlider.maxValue = u.healthMax;
        healthSlider.value = u.healthCurrent;

    }

    //Unit Panel
    public void ActivateUnitPanel()
    {
        if (!unitPanel.activeInHierarchy)
            unitPanel.SetActive(true);
    }

    public void HideUnitPanel()
    {
        if (unitPanel.activeInHierarchy)
            unitPanel.SetActive(false);
    }

    //Selection Menu Panel
    public void ActivateMenuPanel()
    {
        if(!selectionMenuPanel.activeInHierarchy)
            selectionMenuPanel.SetActive(true);
    }

    public void HideMenuPanel()
    {
        if(selectionMenuPanel.activeInHierarchy)
            selectionMenuPanel.SetActive(false);
    }

    //Action Menu Panel
    public void ActivateActionPanel()
    {
        if (!actionMenuPanel.activeInHierarchy)
            actionMenuPanel.SetActive(true);
    }

    public void HideActionPanel()
    {
        if (actionMenuPanel.activeInHierarchy)
            actionMenuPanel.SetActive(false);
    }

    //Cancel Menu Panel
    public void ActivateCancelPanel()
    {
        if (!cancelMenuPanel.activeInHierarchy)
            cancelMenuPanel.SetActive(true);
    }

    public void HideCancelPanel()
    {
        if (cancelMenuPanel.activeInHierarchy)
            cancelMenuPanel.SetActive(false);
    }


}
