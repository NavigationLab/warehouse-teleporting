using System;
using UnityEngine;
//using CorruptedSmileStudio.MessageBox;

/*
 * Some of this code was based on the tutorial at: raywenderlich.com/149239/htc-vive-tutorial-unity 
 */

public class LaserPointer : MonoBehaviour
{
    public Transform cameraRigTransform;
    public Transform headTransform; // The camera rig's head
    public Vector3 teleportReticleOffset; // Offset from the floor for the reticle to avoid z-fighting
    public LayerMask teleportMask; // Mask to filter out areas where teleports are allowed

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab; // The laser prefab
    private GameObject laser; // A reference to the spawned laser
    private Transform laserTransform; // The transform component of the laser for ease of use

    private Vector3 hitPoint; // Point where the raycast hits
    private bool shouldTeleport; // True if there's a valid teleport target
    private bool shouldAnswer; // True if there's a valid answer target

    // Prefabs for reticles
    public GameObject answerReticlePrefab;
    public GameObject rotateReticlePrefab;
    public GameObject teleportReticlePrefab;

    // Instances of prefabs
    private GameObject answerReticle;
    private GameObject teleportReticle;
    private GameObject rotateReticle;

    // The zero vector is returned by the controller if touhcpad isn't 
    // being touched
    Vector2 zeroVector = new Vector2(0.0f, 0.0f);

    // Needed for time tracking
    float currentTime;

    // Transforms
    private Transform answerReticleTransform;
    private Transform teleportReticleTransform;
    private Transform rotateReticleTransform;

    // Buttons reserved for special functions
    public const ulong MoveButton = SteamVR_Controller.ButtonMask.Touchpad;
    public const ulong AnswerButton = SteamVR_Controller.ButtonMask.Trigger;

    // Reticle actually being used and its transform
    private GameObject reticle;
    private Transform reticleTransform;

    // References to each of the markers
    private GameObject[] markers = new GameObject[3];

    // Whether the reticle was locked on a marker in the last cycle
    private bool snapped_on = false;
    private bool rotated_on = false;

    int thumb_rotation;

    // A reference to the controller
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }


    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start()
    {
        // Intialize laser
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

        // Create instances of reticles
        teleportReticle = Instantiate(teleportReticlePrefab);
        answerReticle = Instantiate(answerReticlePrefab);
        rotateReticle = Instantiate(rotateReticlePrefab);

        // Make variables for transforms
        teleportReticleTransform = teleportReticle.transform;
        answerReticleTransform = answerReticle.transform;
        rotateReticleTransform = rotateReticle.transform;

        // Use the reticle corresponding to the movement mode
        switch (GameSettings.navMode)
        {
            case "Concordant":
                break;
            case "Hybrid":
                reticle = teleportReticle;
                reticleTransform = teleportReticleTransform;
                break;
            case "Discordant":
                reticle = rotateReticle;
                reticleTransform = rotateReticleTransform;
                break;
            default:
                //MessageBox.Show(Do_Nothing,
                //                "Error: Invalid Interface.\nLaserPointer.cs Line 127.",
                //                "Invalid Interface",
                //                MessageBoxButtons.OK);
                return;
        }

        // Get markers
        markers[0] = GameObject.Find("StartMarker");
        markers[1] = GameObject.Find("FirstMarker");
        markers[2] = GameObject.Find("SecondMarker");

        // Record a start time for the trial
        currentTime = Time.time;
        System.IO.File.AppendAllText(GameSettings.track_file,
                                     "Start time:" + Time.time + "\r\n");

        // Hide reticles and laser
        laser.SetActive(false);
        teleportReticle.SetActive(false);
        answerReticle.SetActive(false);
        rotateReticle.SetActive(false);
    }

    void Update()
    {
        // Check if the user has attempted to teleport (thumbpad) or point (trigger).
        // teleport is a valid move only if the navigation interface allows it and there are
        // markers left, while answer is a valid move only if there are no markers left.
        bool movementInitiated = (GameSettings.navMode != "Concordant" &&
                                  GetCurrentMarker() != null &&
                                  Controller.GetPress(MoveButton));
        bool answerInitiated = (GetCurrentMarker() == null &&
                                Controller.GetPress(AnswerButton));

        //print(movementInitiated);

        // Every 100ms, record the user's thumbpad use if there is any
        if (Time.time - currentTime > 0.1f) {
            currentTime = Time.time;
            if (Controller.GetAxis() != zeroVector)
                System.IO.File.AppendAllText(GameSettings.track_file,
                                             "Time:" + Time.time + " Axis vector:" + 
                                              Controller.GetAxis().ToString() + 
                                              " Thumbrotation:" + GetThumbRotation() + "\r\n");
        }

        // Check if the participant is teleporting
        if (movementInitiated) {
            // Send out a raycast from the controller
            RaycastHit hit;
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask)) {

                // Snap the pointer in place if it's within <1/5 scale> of the intended marker
                hitPoint = hit.point;
                if (!snapped_on) {
                    if (Vector3.Distance(hitPoint, GetCurrentMarker().transform.position) * 5 < GameSettings.scale) {
                        hitPoint = GetCurrentMarker().transform.position;
                        snapped_on = true;
                    }
                } else {
                    if (Vector3.Distance(hitPoint, GetCurrentMarker().transform.position) * 0.8 < GameSettings.scale)
                        hitPoint = GetCurrentMarker().transform.position;
                    else
                        snapped_on = false;
                }

                // Point the laser
                ShowLaser(hit);

                // Show teleport reticle
                reticle.SetActive(true);
                reticleTransform.position = hitPoint + teleportReticleOffset;

                // If using the discordant interface, rotate the reticle to match new orientation
                if (GameSettings.navMode == "Discordant")
                {
                    // Snap the reticle rotation if it's within 10 degrees of the intended orientation
                    // First calculate and simplify the thumb rotation
                    thumb_rotation = (int)(Math.Round(headTransform.eulerAngles.y) + GetThumbRotation());
                    //print(headTransform.eulerAngles.y);
                    //print(GetThumbRotation());
                    //print(thumb_rotation);
                    //print("");
                    while (thumb_rotation > 360)
                        thumb_rotation -= 360;
                    while (thumb_rotation < 0)
                        thumb_rotation += 360;

                    // then compare to the marker arrow
                    Material[] mats = GameObject.Find(GetCurrentMarker().name + "/ringarrow/Arrow").GetComponent<Renderer>().materials;
                    if (snapped_on)
                    {
                        if (!rotated_on)
                        {
                            if (Math.Abs(thumb_rotation - GetCurrentMarker().transform.eulerAngles.y) < 10)
                            {
                                thumb_rotation = (int)GetCurrentMarker().transform.eulerAngles.y;
                                mats[0].color = Color.magenta;
                                mats[1].color = Color.magenta;
                                rotated_on = true;
                            } else
                            {
                                mats[0].color = Color.cyan;
                                mats[1].color = Color.cyan;
                            }
                        } else {
                            if (Math.Abs(thumb_rotation - GetCurrentMarker().transform.eulerAngles.y) < 80)
                            {
                                thumb_rotation = (int)GetCurrentMarker().transform.eulerAngles.y;
                                mats[0].color = Color.magenta;
                                mats[1].color = Color.magenta;
                            } else {
                                mats[0].color = Color.cyan;
                                mats[1].color = Color.cyan;
                                rotated_on = false;
                            }
                        }
                    }

                    GameObject.Find(GetCurrentMarker().name + "/ringarrow/Arrow").GetComponent<Renderer>().materials = mats;

                    reticleTransform.rotation = Quaternion.Euler(0, thumb_rotation, 0);
                }
                // If you're in this block, you hit something with the teleport mask
                shouldTeleport = true;
            }
            else {
                // If you didn't hit the somthing with the teleport mask, turn off the laser
                laser.SetActive(false);
                reticle.SetActive(false);
                shouldTeleport = false;
            }
        }
        // If not teleporting, check if the participant is answering
        else if (answerInitiated) {
            // Send out a raycast from the controller
            RaycastHit hit;

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                // Point the laser
                hitPoint = hit.point;
                ShowLaser(hit);

                //Show teleport reticle
                answerReticle.SetActive(true);
                answerReticleTransform.position = hitPoint + teleportReticleOffset;

                // If you're in this block, you hit something with the teleport mask
                shouldAnswer = true;
            }
            else {
                // If you didn't hit the somthing with the teleport mask, turn off the laser
                laser.SetActive(false);
                answerReticle.SetActive(false);
                shouldAnswer = false;
            }
        }

        // If the player initiated a teleport to a valid location (object with teleport mask)
        // then teleport them there
        if (Controller.GetPressUp(MoveButton) && shouldTeleport && GameSettings.navMode != "Concordant")
        {
            Teleport();
            if (GameSettings.navMode == "Discordant")
                Rotate();

            // Hide the laser after teleporting
            laser.SetActive(false);
            answerReticle.SetActive(false);
        }
        else if (Controller.GetPressUp(AnswerButton) && shouldAnswer)
        {
            // Writes the response to a file
            System.IO.File.AppendAllText(GameSettings.response_file, "Response: " +
                                                                     answerReticleTransform.position.ToString() +
                                                                     " Time: " + Time.time + "\r\n");
            answerReticle.SetActive(false);
            laser.SetActive(false);

            // Display a configuration-specific popup message for the experimenter
            //if (GameSettings.env == "Training")
            //    MessageBox.Show(Do_Nothing, GameSettings.navMode + " interface training complete. Press Space to re-run, or Escape to return to menu.",
            //                    GameSettings.navMode + " Training Complete",
            //                    MessageBoxButtons.OK);
            //else
            //    MessageBox.Show(Do_Nothing, GameSettings.navMode + " interface trial " + GameSettings.current_trial.ToString() + "/" + 
            //                                GameSettings.trial_count.ToString() + " complete. Press Space to continue, Backspace to redo, or Escape to return to menu.",
            //                    GameSettings.navMode + " " + GameSettings.current_trial.ToString() + "/" + GameSettings.trial_count.ToString() + " Complete",
            //                    MessageBoxButtons.OK);
        }
    }

    /*
     * 	Name: GetCurrentMarker
     *
     *	Purpose: This function gets us the marker that the participant is currently viewing. When the
     *		 paritipant collides with a marker, the marker's active field is set to false, and since
     *		 the markers appear in order, the current marker is the first one that isn't false. If
     *		 all the markers are not active (the pariticpants has collided with all of them), then
     *		 null is returned.
     */
    private GameObject GetCurrentMarker()
    {
        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i].activeSelf)
            {
                return markers[i];
            }
        }

        return null;
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true); //Show the laser
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.LookAt(hitPoint); // Rotate laser facing the hit point
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            Vector3.Distance(trackedObj.transform.position, hitPoint)); // Scale laser so it fits exactly between the controller & the hit point
    }

    /*
     * 	Name: Teleport
     *
     * 	Purpose: This function teleports the participant by moving them to the point that the laser
     * 		 hits the floor. 
     *
     */
    private void Teleport()
    {

        shouldTeleport = false;
        reticle.SetActive(false);

        /* The  difference vector is the offset between the players position and the
         * center of the camera rig. The y component is set to zero so that the player
         * isn't teleported vertically
             */
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0;

        cameraRigTransform.position = hitPoint + difference;
    }

    /*
     * Name: GetThumbRotation
     *
     * Purpose: This function finds the degree of turn indicated by the user on
     * 		on the controller trackpad. The position the user is touching the
     * 		trackpad is returned with the statement: Controller.GetAxis()
     */
    float GetThumbRotation()
    {
        /* Find the angle between the forward direction (up on the pad) and the 
	 * vector starting at the center and ending at the point where the user
	 * has their thumb on the pad
	 */
        float thumbAngle = Vector2.Angle(Vector2.up, Controller.GetAxis());


        /* Turning to the left is considiered a negative and turning to the left
         * is considered positive. The x value reflects these sign conventions
         */
        float turnSign = Mathf.Sign(Controller.GetAxis().x);

        return thumbAngle * turnSign;
    }

    /*
     * 	Name: Rotate
     *
     * 	Purpose: This function rotates the player by rotating the camera rig. The
     * 		 amount that the camera rig is rotated is the amount specified by
     * 		 the user on the trackpad.
     */
    void Rotate()
    {
        //cameraRigTransform.Rotate(0, GetThumbRotation(), 0);
        cameraRigTransform.RotateAround(headTransform.position, Vector3.up, thumb_rotation - (int)Math.Round(cameraRigTransform.eulerAngles.y));
    }
    
    //void Do_Nothing(DialogResult result) { }
}