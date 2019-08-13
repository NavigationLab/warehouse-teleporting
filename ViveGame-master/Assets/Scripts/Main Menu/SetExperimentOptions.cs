using UnityEngine;
using UnityEngine.UI;

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;


public class SetExperimentOptions : MonoBehaviour
{
    public Dropdown env_dropdown, nav_dropdown;
    public Toggle landmark_yes, landmark_no, fence_yes, fence_no, fence_square, fence_round, warehouse_big, warehouse_small;
    public Text fence_shape_text;

    void Awake()
    {
        //try
        //{
            string line;
            StreamReader reader = new StreamReader("experiment_config.txt", Encoding.Default);
            using (reader) { reader.ReadLine(); line = reader.ReadLine(); }
            if (line != null)
            {
                string exp = line.Split('=')[1].Trim();
                if (exp != "") configureUI(Int32.Parse(exp));
            }
        /*}
        catch
        {
            MessageBox.Show(Do_Nothing, "Something went wrong with experiment configuration. Check to make sure " +
                "experiment_config.txt is in the same directory as the Exe, and the 2nd line is " +
                "formatted as 'experiment=<number>'.", "Experiment Configuration Error", MessageBoxButtons.OK);
        }*/
    }

    void configureUI(int exp)
    {
        // Store the experiment number in case other modules need it
        GameSettings.experiment_num = exp;

        // Clear out the old dropdown options
        env_dropdown.ClearOptions();
        nav_dropdown.ClearOptions();

        // Make a list to store new environment options
        List<string> env_list = new List<string>();
        env_list.Add("Select");
        env_list.Add("Training");
        if (exp < 6) env_list.Add("Field");

        // Make a list to store new navigation options
        List<string> nav_list = new List<string>();
        nav_list.Add("Select");
        if (exp < 6) nav_list.Add("Concordant(Walking)");
        nav_list.Add("Hybrid (Teleporting & Turning)");
        nav_list.Add("Discordant (All Teleporting)");

        // Filter the Environment options depending on the experiment chosen
        switch (exp)
        {
            case 1:
                GameSettings.fence_yes = false;
                GameSettings.fence_no = true;
                fence_yes.interactable = false;
                fence_no.interactable = false;
                break;
            case 2:
                env_list.Add("Classroom");
                GameSettings.landmarks_yes = false;
                GameSettings.landmarks_no = true;
                landmark_yes.interactable = false;
                landmark_no.interactable = false;
                break;
            case 3:
                GameSettings.landmarks_yes = true;
                GameSettings.landmarks_no = false;
                landmark_yes.interactable = false;
                landmark_no.interactable = false;
                break;
            case 4:
                GameSettings.landmarks_yes = true;
                GameSettings.landmarks_no = false;
                landmark_yes.interactable = false;
                landmark_no.interactable = false;
                GameSettings.fence_yes = true;
                GameSettings.fence_no = false;
                fence_yes.interactable = false;
                fence_no.interactable = false;
                break;
            case 5:
                GameSettings.fence_yes = true;
                GameSettings.fence_no = false;
                fence_yes.interactable = false;
                fence_no.interactable = false;
                GameSettings.fence_square = true;
                GameSettings.fence_round = false;
                fence_square.interactable = false;
                fence_round.interactable = false;
                break;
            case 6:
                env_list.Add("Warehouse");
                env_list.Add("Library");
                break;
            default:
                env_list.Add("Classroom");
                env_list.Add("Warehouse");
                env_list.Add("Library");
                break;
        }

        // Add the new environment options
        foreach (string env in env_list)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = env;
            env_dropdown.options.Add(option);
        }

        // Add the new navigation options
        foreach (string nav in nav_list)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = nav;
            nav_dropdown.options.Add(option);
        }
    }
}
