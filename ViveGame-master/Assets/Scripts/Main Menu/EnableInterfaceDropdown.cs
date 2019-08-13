using UnityEngine;
using UnityEngine.UI;

public class EnableInterfaceDropdown : MonoBehaviour {

    Dropdown interface_dropdown;

    public Dropdown environment, scale;
    public Toggle landmark_yes, landmark_no, fence_yes, fence_no, fence_square, fence_round, warehouse_big, warehouse_small;

    void Start()
    {
        //Fetch the Dropdown GameObject
        interface_dropdown = GetComponent<Dropdown>();

        //Add listener for when the value of the Dropdown changes, to take action
        environment.onValueChanged.AddListener( delegate { CheckEnable(); });
        landmark_yes.onValueChanged.AddListener(delegate { CheckEnable(); });
        landmark_no.onValueChanged.AddListener( delegate { CheckEnable(); });
        fence_yes.onValueChanged.AddListener(   delegate { CheckEnable(); });
        fence_no.onValueChanged.AddListener(    delegate { CheckEnable(); });
        fence_square.onValueChanged.AddListener(delegate { CheckEnable(); });
        fence_round.onValueChanged.AddListener( delegate { CheckEnable(); });
        scale.onValueChanged.AddListener(       delegate { CheckEnable(); });
        warehouse_big.onValueChanged.AddListener(delegate { CheckEnable(); });
        warehouse_small.onValueChanged.AddListener(delegate { CheckEnable(); });
    }

    void CheckEnable () {

        switch (environment.options[environment.value].text)
        {
            case "Select":
                interface_dropdown.interactable = false;
                break;
            case "Training": case "Classroom":
                interface_dropdown.interactable = true;
                break;
            case "Field":
                interface_dropdown.interactable = ((landmark_yes.isOn || landmark_no.isOn) &&
                                                   (fence_yes.isOn && (fence_square.isOn || fence_round.isOn)) ||
                                                   (fence_no.isOn));
                break;
            case "Warehouse":
                interface_dropdown.interactable = ((warehouse_big.isOn || warehouse_small.isOn) &&
                                                   scale.options[scale.value].text != "Select");
                break;
            case "Library":
                interface_dropdown.interactable = scale.options[scale.value].text != "Select";
                break;
        }
    }
}
