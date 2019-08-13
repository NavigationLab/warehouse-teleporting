using UnityEngine;
using UnityEngine.UI;

public class ConfigureMenu : MonoBehaviour {

    public InputField participant_box, trial_box;
    public Dropdown gender_dropdown, environment_dropdown, scale_dropdown, nav_dropdown;
    public Toggle landmarks_yes, landmarks_no, fence_yes, fence_no, fence_square, fence_round, warehouse_big, warehouse_small;

    // Set all the menu fields based on either the defaults for the 
    // first run, or the last configuration if returning from a scene
    void Start () {
        // Set participant ID
        participant_box.text = GameSettings.participant_id;

        // Set gender selection
        gender_dropdown.value = gender_dropdown.options.FindIndex(
            (i) => { return i.text.Equals(GameSettings.gender); });

        // Set number of trials
        trial_box.text = GameSettings.trial_count.ToString();

        // Set environment selection
        environment_dropdown.value = environment_dropdown.options.FindIndex(
            (i) => { return i.text.Equals(GameSettings.environment); });

        // Under Field environment, set landmark toggles selection
        landmarks_yes.isOn = GameSettings.landmarks_yes;
        landmarks_no.isOn = GameSettings.landmarks_no;

        // Under Field environment, set fence presence toggles selection
        fence_yes.isOn = GameSettings.fence_yes;
        fence_no.isOn = GameSettings.fence_no;

        // Under Field environment, set fence shape toggles selection
        fence_square.isOn = GameSettings.fence_square;
        fence_round.isOn = GameSettings.fence_round;

        // Under Warehouse & Library environments, set scale selection
        scale_dropdown.value = scale_dropdown.options.FindIndex(
            (i) => { return i.text.Equals(GameSettings.scale_selection); });

        // Under Warehouse, set the warehouse size toggles selection
        warehouse_big.isOn = GameSettings.warehouse_big;
        warehouse_small.isOn = GameSettings.warehouse_small;

        // Set the navigation interface selection
        nav_dropdown.value = nav_dropdown.options.FindIndex(
            (i) => { return i.text.Equals(GameSettings.nav_interface); });
    }
}
