using UnityEngine;
using UnityEngine.UI;

public class ActivateFenceShape : MonoBehaviour {

    Toggle fence_yes;

    public Text fence_shape_text;
    public Toggle fence_square, fence_round;

	// Use this for initialization
	void Start () {
        //Fetch the Toggle GameObject
        fence_yes = GetComponent<Toggle>();

        //Call once right away to activate if Toggle is initialized to yes.
        ToggleValueChanged(fence_yes);

        //Add listener for when the value of the Dropdown changes, to take action
        fence_yes.onValueChanged.AddListener(delegate {
            ToggleValueChanged(fence_yes);
        });
    }

	void ToggleValueChanged (Toggle yes_button) {
		if (yes_button.isOn)
        {
            fence_shape_text.gameObject.SetActive(true);
            fence_square.gameObject.SetActive(true);
            fence_round.gameObject.SetActive(true);
        } else {
            fence_shape_text.gameObject.SetActive(false);
            fence_square.gameObject.SetActive(false);
            fence_round.gameObject.SetActive(false);
        }
	}
}
