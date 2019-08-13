using UnityEngine;
using UnityEngine.UI;

public class EnableButton : MonoBehaviour {

    Button start_button;

    public Dropdown interface_selection;

    void Start()
    {
        //Fetch the Button GameObject
        start_button = GetComponent<Button>();

        //Add listener for when the value of the Dropdown changes, to take action
        interface_selection.onValueChanged.AddListener( delegate { CheckEnable(); });
    }

    void CheckEnable () {

        if (interface_selection.interactable && interface_selection.options[interface_selection.value].text != "Select")
            start_button.interactable = true;
        else start_button.interactable = false;
    }
}
