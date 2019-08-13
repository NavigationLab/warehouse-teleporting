using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.IO;
using System.Collections.Generic;
//using CorruptedSmileStudio.MessageBox;


public class StartButtonPress : MonoBehaviour
{
    public InputField p_id, trial_box;
    public Dropdown gender_dropdown, environment_dropdown, scale_dropdown, nav_dropdown;
    public Toggle landmark_yes, landmark_no, fence_yes, fence_no, fence_square, fence_round, warehouse_big, warehouse_small;

    public void start_environment()
    {
        if (validate_selections() && validate_data_dir()) {
            set_data_file_paths();
            configure_environment();
        }
    }

    private bool validate_selections() {

        // Grab and validate participant ID
        GameSettings.participant_id = p_id.text.ToString();
        if (GameSettings.participant_id == "") {
            //MessageBox.Show(Stop_Error, "Please enter a valid ID", "Invalid Participant ID", MessageBoxButtons.OK);
            return false; }

        // Grab and validate gender selection
        GameSettings.gender = gender_dropdown.options[gender_dropdown.value].text;
        if (GameSettings.gender == "Select") {
            //MessageBox.Show(Stop_Error, "Please Select an option for gender.", "Invalid Gender Selection", MessageBoxButtons.OK);
            return false; }

        // Grab and validate trial count selection
        bool okay = int.TryParse(trial_box.text.ToString(), out GameSettings.trial_count);
        if (!okay) {
            //MessageBox.Show(Stop_Error, "Please enter a valid number of experiment trials", "Invalid Trial Count", MessageBoxButtons.OK);
            return false; }

        // Grab and validate environment selection and environment-specific options
        GameSettings.environment = environment_dropdown.options[environment_dropdown.value].text;
        switch (GameSettings.environment)
        {
            default:  // If environment isn't set to a known scene, or still to "Select", there's an error.
                //MessageBox.Show(Stop_Error, "Please select an environment.", "Invalid Environment Selection", MessageBoxButtons.OK);
                return false;

            case "Training": case "Classroom":
                break;

            case "Field":
                if (!(landmark_yes.isOn || landmark_no.isOn) ||
                    !(fence_yes.isOn || fence_no.isOn) ||
                    (fence_yes.isOn && !(fence_square.isOn || fence_round.isOn))) {
                    //MessageBox.Show(Stop_Error, "Please make sure your landmarks and fence options are set", "Invalid Environment Options", MessageBoxButtons.OK);
                    return false; }
                GameSettings.landmarks_yes = landmark_yes.isOn;
                GameSettings.landmarks_no = landmark_no.isOn;
                GameSettings.fence_yes = fence_yes.isOn;
                GameSettings.fence_no = fence_no.isOn;
                GameSettings.fence_square = fence_square.isOn;
                GameSettings.fence_round = fence_round.isOn;
                break;

            case "Warehouse": case "Library":
                if (scale_dropdown.options[scale_dropdown.value].text == "Select") {
                    //MessageBox.Show(Stop_Error, "Please make sure your scale option is set", "Invalid Environment Options", MessageBoxButtons.OK);
                    return false; }
                GameSettings.scale_selection = scale_dropdown.options[scale_dropdown.value].text;
                GameSettings.warehouse_big = warehouse_big.isOn;
                GameSettings.warehouse_small = warehouse_small.isOn;
                break;
        }
        // Grab and validate navigation interface selection
        GameSettings.nav_interface = nav_dropdown.options[nav_dropdown.value].text;
        if (GameSettings.nav_interface == "Select") {
            //MessageBox.Show(Stop_Error, "Please make sure your interface setting is correct.", "Invalid Interface Selection", MessageBoxButtons.OK);
            return false; }

        // If all the above pass, validation is successful
        return true;
    }

    private bool validate_data_dir() {
        
        string path = "Data/";

        // If the experiment # is specified in experiment_config.txt, it will save within a top-level Experiment_X folder
        if (GameSettings.experiment_num > 0)
            path += "Experiment " + GameSettings.experiment_num.ToString() + "/";

        // Next level is the navigation interface
        path += GameSettings.nav_interface + "/";

        // Then the environment and options
        path += GameSettings.environment;
        switch (GameSettings.environment) {
            case "Field":
                if (GameSettings.landmarks_yes) path += "_WithLandmarks";
                else if (GameSettings.landmarks_no) path += "_NoLandmarks";
                if (GameSettings.fence_yes)
                {
                    if (GameSettings.fence_square) path += "_SquareFence";
                    else if (GameSettings.fence_round) path += "_RoundFence";
                }
                else if (GameSettings.fence_no) path += "_NoFence";
                break;
            case "Warehouse":
                if (GameSettings.warehouse_big)
                    path += "Big";
                else
                    path += "Small";
                path += "_" + GameSettings.scale_selection.Substring(0, 1) + "x";
                break;
            case "Library":
                path += "_" + GameSettings.scale_selection.Substring(0,1) + "x";
                break;
        }
        path += "/";
        
        // Finally, the participant ID and gender
        path += "P" + GameSettings.participant_id + "_" + GameSettings.gender[0] + "/";

        // Check if participant folder exists.
        // If yes, ask confirmation for overwrite before creating.
        GameSettings.data_dir = new DirectoryInfo(path);
        if (GameSettings.data_dir.Exists) {
            //MessageBox.Show(Folder_Exists, "A folder for the participant ID you entered already exists. " +
            //    "Proceeding will overwrite this folder. Are you sure you wish to continue?", "Folder Already Exists", MessageBoxButtons.YesNo);
            return false; }

        // If no (or if 2nd time around after Folder_Exists() confirmation), good to go
        return true;
    }

    private void set_data_file_paths()
    {
        GameSettings.data_dir.Create();
        GameSettings.marker_file = GameSettings.data_dir.FullName + "Marker.txt";
        GameSettings.place_file = GameSettings.data_dir.FullName + "Place.txt";
        GameSettings.response_file = GameSettings.data_dir.FullName + "Response.txt";
        GameSettings.time_file = GameSettings.data_dir.FullName + "Time.txt";
        GameSettings.track_file = GameSettings.data_dir.FullName + "Track.txt";
    }

    public void configure_environment()
    {
        GameSettings.env = GameSettings.environment;
        GameSettings.landmarks = GameSettings.landmarks_yes;
        GameSettings.fence_presence = GameSettings.fence_yes;

        if (GameSettings.fence_presence) {
            if (GameSettings.fence_square) GameSettings.fence_shape = "square";
            else if (GameSettings.fence_round) GameSettings.fence_shape = "round"; }

        if (GameSettings.environment == "Warehouse")
        {
            if (int.TryParse(GameSettings.scale_selection[0].ToString(), out GameSettings.scale))
                print(GameSettings.scale);
            else
            {
                print("Failure: " + GameSettings.scale_selection);
                GameSettings.scale = 1;
            }
        }
        else
            GameSettings.scale = 1;
        

        GameSettings.navMode = GameSettings.nav_interface.Split(' ')[0].Trim();
        GameSettings.current_trial = 1;

        GameSettings.degreeList = new List<float>();
        for (float i = -135.0f; i< 136.0f; i += 270.0f / GameSettings.trial_count)
            GameSettings.degreeList.Add(i);
        GameSettings.degreeList.Remove(0.0f);

        SceneManager.LoadScene(GameSettings.env);
    }

    //void Stop_Error(DialogResult result) { }

    //void Folder_Exists(DialogResult result)
    //{
    //    if (result == DialogResult.Yes)
    //    {
    //        GameSettings.data_dir.Delete(true);
    //        this.start_environment();
    //    }
    //}
}