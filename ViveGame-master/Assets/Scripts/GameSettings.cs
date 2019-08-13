using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameSettings : MonoBehaviour {

    public static GameSettings instance;

    // Form data for populating the Main Menu
    public static int experiment_num = 0;
    public static string participant_id = "";
    public static string gender = "Select";
    public static int trial_count = 12;
    public static string environment = "Select";
    public static bool landmarks_yes = false;
    public static bool landmarks_no = false;
    public static bool fence_yes = false;
    public static bool fence_no = false;
    public static bool fence_square = false;
    public static bool fence_round = false;
    public static string scale_selection = "Select";
    public static bool warehouse_big = true;
    public static bool warehouse_small = false; 
    public static string nav_interface = "Select";

    // Data collection file paths
    public static DirectoryInfo data_dir;
    public static string marker_file = "C://Users/alecglen/Desktop/test/marker.txt";
    public static string place_file = "C://Users/alecglen/Desktop/test/place.txt";
    public static string response_file = "C://Users/alecglen/Desktop/test/response.txt";
    public static string time_file = "C://Users/alecglen/Desktop/test/time.txt";
    public static string track_file = "C://Users/alecglen/Desktop/test/track.txt";
    /*public static string marker_file = "C://Users/psych_admin/Desktop/test/marker.txt";
    public static string place_file = "C://Users/psych_admin/Desktop/test/place.txt";
    public static string response_file = "C://Users/psych_admin/Desktop/test/response.txt";
    public static string time_file = "C://Users/psych_admin/Desktop/test/time.txt";
    public static string track_file = "C://Users/psych_admin/Desktop/test/track.txt";*/


    // Scene configuration variables. Having these separate from the Menu defaults allows
    // me to set a default environment and configuration for debugging
    public static string env = "Warehouse";
    public static bool landmarks = true;
    public static bool fence_presence = true;
    public static string fence_shape = "square";
    public static int scale = 5;
    public static string navMode = "Discordant";

    public static List<float> degreeList = new List<float> { 135, 112.5f, 90, 67.5f, 45, 22.5f, -22.5f, -45, -67.5f, -90, -112.5f, -135 };

    // State variables
    public static int current_trial = 1;


    void Awake()
    {
        instance = this;
    }
}