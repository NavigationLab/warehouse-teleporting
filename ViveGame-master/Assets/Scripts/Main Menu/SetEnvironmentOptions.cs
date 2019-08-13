using UnityEngine;
using UnityEngine.UI;

public class SetEnvironmentOptions : MonoBehaviour
{
    Dropdown m_Dropdown;

    public Text landmark_text, fence_text, fence_shape_text, scale_text, warehouse_size_text;
    public Toggle landmark_yes, landmark_no, fence_yes, fence_no, fence_square, fence_round, warehouse_big, warehouse_small;
    public Dropdown scale;

    void Start()
    {
        //Fetch the Dropdown GameObject
        m_Dropdown = GetComponent<Dropdown>();

        //Add listener for when the value of the Dropdown changes, to take action
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });

        // Shuffle through the dropdown quick to reissue configuration when returning from trial
        m_Dropdown.value = m_Dropdown.options.FindIndex((i) => { return i.text.Equals("Select"); });
        m_Dropdown.value = m_Dropdown.options.FindIndex((i) => { return i.text.Equals(GameSettings.environment); });
    }

    // Set the correct interface active depending on the dropdown value
    void DropdownValueChanged(Dropdown change)
    {
        switch (change.options[change.value].text)
        {
            case "Select": case "Training": case "Classroom":
                landmark_text.gameObject.SetActive(false);
                landmark_yes.gameObject.SetActive(false);
                landmark_no.gameObject.SetActive(false);
                fence_text.gameObject.SetActive(false);
                fence_yes.gameObject.SetActive(false);
                fence_no.gameObject.SetActive(false);
                fence_shape_text.gameObject.SetActive(false);
                fence_square.gameObject.SetActive(false);
                fence_round.gameObject.SetActive(false);
                scale_text.gameObject.SetActive(false);
                scale.gameObject.SetActive(false);
                warehouse_size_text.gameObject.SetActive(false);
                warehouse_big.gameObject.SetActive(false);
                warehouse_small.gameObject.SetActive(false);
                break;
            case "Field":
                landmark_text.gameObject.SetActive(true);
                landmark_yes.gameObject.SetActive(true);
                landmark_no.gameObject.SetActive(true);
                fence_text.gameObject.SetActive(true);
                fence_yes.gameObject.SetActive(true);
                fence_no.gameObject.SetActive(true);
                scale_text.gameObject.SetActive(false);
                scale.gameObject.SetActive(false);
                warehouse_size_text.gameObject.SetActive(false);
                warehouse_big.gameObject.SetActive(false);
                warehouse_small.gameObject.SetActive(false);

                if (GameSettings.experiment_num == 4 || GameSettings.experiment_num == 5) {
                    fence_shape_text.gameObject.SetActive(true);
                    fence_square.gameObject.SetActive(true);
                    fence_round.gameObject.SetActive(true);
                } else {
                    fence_shape_text.gameObject.SetActive(false);
                    fence_square.gameObject.SetActive(false);
                    fence_round.gameObject.SetActive(false);
                }
                break;
            case "Warehouse":
                landmark_text.gameObject.SetActive(false);
                landmark_yes.gameObject.SetActive(false);
                landmark_no.gameObject.SetActive(false);
                fence_text.gameObject.SetActive(false);
                fence_yes.gameObject.SetActive(false);
                fence_no.gameObject.SetActive(false);
                fence_shape_text.gameObject.SetActive(false);
                fence_square.gameObject.SetActive(false);
                fence_round.gameObject.SetActive(false);
                scale_text.gameObject.SetActive(true);
                scale.gameObject.SetActive(true);
                warehouse_size_text.gameObject.SetActive(true);
                warehouse_big.gameObject.SetActive(true);
                warehouse_small.gameObject.SetActive(true);
                break;
            case "Library":
                landmark_text.gameObject.SetActive(false);
                landmark_yes.gameObject.SetActive(false);
                landmark_no.gameObject.SetActive(false);
                fence_text.gameObject.SetActive(false);
                fence_yes.gameObject.SetActive(false);
                fence_no.gameObject.SetActive(false);
                fence_shape_text.gameObject.SetActive(false);
                fence_square.gameObject.SetActive(false);
                fence_round.gameObject.SetActive(false);
                scale_text.gameObject.SetActive(true);
                scale.gameObject.SetActive(true);
                warehouse_size_text.gameObject.SetActive(false);
                warehouse_big.gameObject.SetActive(false);
                warehouse_small.gameObject.SetActive(false);
                break;
        }
    }
}