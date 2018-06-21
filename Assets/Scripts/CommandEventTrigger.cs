using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandEventTrigger : EventTrigger {

    Command cmd;

    void Start()
    {
        cmd = this.GetComponent<Command>();
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        cmd.OnMouseEnter();
    }

    public override void OnPointerExit(PointerEventData data)
    {
        cmd.OnMouseExit();
    }
}
