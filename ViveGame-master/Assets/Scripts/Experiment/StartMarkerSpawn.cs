using UnityEngine;

//This script positions the starting marker at the starting position of the triangle, using the start position given to it by Restart.cs

public class StartMarkerSpawn : MonoBehaviour {
    
    float xCoord; //x coordinate of startmarker
    float zCoord; //z coordinate of startmarker
    public static int degree; //degree of the triangle
    private GameObject GameController; //used to handle restarting the game

    // Use this for initialization
    void Start() {

        //main game controller 
        GameController = GameObject.Find("GameController");
        Restart RestartScript = GameController.GetComponent<Restart>(); 

        xCoord = 0;
        zCoord = 0;

        /* Depending on random num picked in Restart.cs, set start marker to start position and rotation towards origin -- places markers 
         * at sides and corners of room with 3ft clearance -- corners are pushed in a bit more, 4ft clearance
         * Marker start positions in room based on starting position #
         *  _______________________
         * |           1           | -
         * |    8             5    |
         * | 4      <-(0,0)      2 |      //<-(0, 0) represents origin and starting rotation          
         * |     7           6     |
         * |___________3___________| +  
         *  -                      +
         */
        switch (RestartScript.startPosition)
        {
            case 1:  xCoord = -1.52f;                  degree = 270; break;
            case 2:                   zCoord = 2.13f;  degree = 0;   break;
            case 3:  xCoord = 1.52f;                   degree = 90;  break;
            case 4:                   zCoord = -2.13f; degree = 180; break;
            case 5:  xCoord = -1.22f; zCoord = 1.83f;  degree = 315; break;
            case 6:  xCoord = 1.22f;  zCoord = 1.83f;  degree = 45;  break;
            case 7:  xCoord = 1.22f;  zCoord = -1.83f; degree = 135; break;
            default: xCoord = -1.22f; zCoord = -1.83f; degree = 225; break;
        }

        // Set the starting marker to the appropriate rotation and coordinates so it faces the origin
        this.transform.Rotate(0, degree, 0);
        this.transform.position = new Vector3(xCoord * GameSettings.scale, 0, zCoord * GameSettings.scale);

        // Write the marker position and time the appropriate files
        System.IO.File.AppendAllText(GameSettings.marker_file, "Starting Marker:" + this.transform.position + "\r\n");
        System.IO.File.AppendAllText(GameSettings.time_file, "Start time:" + Time.time + "\r\n");
    }
}
