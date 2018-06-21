using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionProgramManager : MonoBehaviour {


    //Referencia aos Componentes da Janela do Programa de Ação

    //Command Description Window
    public GameObject descPanel;
    public Text descNameText, descDescriptionText;
    public Image descTargetImage;


    void Start()
    {
        if (LevelManager._instance.actionProgram == null)
            LevelManager._instance.actionProgram = this;
        else
            Destroy(this);
    }

    public void UpdateDescription(Command cmd)
    {
        if(!descPanel.activeInHierarchy)
        {
            descPanel.SetActive(true);
        }

        descNameText.text = cmd.cmdName.ToUpper();
        descDescriptionText.text = cmd.cmdDesc;
        descTargetImage.sprite = cmd.cmdTargetImage;
    }

    public void HideDescPanel()
    {
        if (descPanel.activeInHierarchy)
        {
            descPanel.SetActive(false);
        }
    }

}
