using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


// Restart - handles the environment restarting when the experimenter hits space/backspace/escape

public class Restart : MonoBehaviour {

    //START MARKER
    public int startPosition; // keeps track of which start position was picked 
    static int prevPosition; //stores the last round's starting position to avoid repeats

    //used for tracking data output files
    string[] fileList = new string[] { GameSettings.marker_file,
                                       GameSettings.place_file,
                                       GameSettings.response_file,
                                       GameSettings.time_file,
                                       GameSettings.track_file };

    //current degree value
    public static float degree;

    void Start() {

        // Pick a start position based on a random number
        do startPosition = Random.Range(1, 9);
        while (startPosition == prevPosition); //makes sure no start positions are ever chosen twice in a row

        //make sure this position isn't picked next restart
        prevPosition = startPosition;

        //takes a new degree for the current degree value and takes it out of the stack, once you run out play a sound 
        print(System.String.Join(", ", GameSettings.degreeList.ToArray().Select(p => p.ToString()).ToArray()));
        try {
            //for every restart, pop a value from the degree list
            int index = Random.Range(0, GameSettings.degreeList.Count - 1);
            degree = GameSettings.degreeList[index];
            GameSettings.degreeList.RemoveAt(index);
        }
        catch (System.Exception) {
            // once all the angles have been removed, play a sound
            print("All trials complete");
        }
    }

    void Update() {

        // Continues the scene on key press Space
        if (Input.GetKeyDown("space")) {

            //appends "new trial" to the output file each time
            foreach (string file in fileList)
                System.IO.File.AppendAllText(file, "-------New Trial--------\r\n");

            if (GameSettings.env != "Training" && GameSettings.current_trial == GameSettings.trial_count)
                SceneManager.LoadScene("MainMenu");
            else {
                GameSettings.current_trial += 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        // Repeats the trial on key press backspace
        else if (Input.GetKeyDown("backspace")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            foreach (string file in fileList)
                System.IO.File.AppendAllText(file, "-------Restart--------\r\n");
        }

        // Returns to the main menu on escape
        else if (Input.GetKeyDown("escape"))
            SceneManager.LoadScene("MainMenu");
    }
}
