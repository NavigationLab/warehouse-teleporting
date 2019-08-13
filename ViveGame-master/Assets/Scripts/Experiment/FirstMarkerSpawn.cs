using UnityEngine;

/*
 * Deals with the position of the first marker
 */

public class FirstMarkerSpawn : MonoBehaviour {
    
    private GameObject FirstMarker;
    private GameObject StartMarker;
    private GameObject GameController;
    private GameObject ring; // the ring around the bottom of the first marker
    private GameObject arrow; //the directional arrow of the first marker

    Renderer rend; //renderer for FirstMarker
    Renderer ring_rend; // renderer for FirstMarker's ring
    Renderer arrow_rend; // renderer for FirstMarker's arrow
    int startDegree; //degree of start arrow
    public static float degree; //degree of first arrow

    int randNum; //determines which random length is picked
    float length; //the length of the path from the start to the first marker
    float[] lengthArray = new float[] { 0, .5f, 1 };  //determines the amount (meter) that will be added to the base distance 


    void Start ()
    {
        //when the marker is initialized, move it somewhere where it won't be seen or be in the way
        this.transform.position = new Vector3(10, -10, 10);

        //determines the random amount that will be added to the base distance from start marker to first marker
        randNum = Random.Range(0, lengthArray.Length);
        length = lengthArray[randNum] / 3.28f; //divide to convert to meter amt

        //start marker
        StartMarker = GameObject.Find("StartMarker");

        //firstmarker's arrow
        ring = GameObject.Find("FirstMarker/ringarrow/Ring");
        arrow = GameObject.Find("FirstMarker/ringarrow/Arrow");

        //renderer for first marker and its arrow
        rend = this.GetComponent<Renderer>();
        ring_rend = ring.GetComponent<Renderer>();
        arrow_rend = arrow.GetComponent<Renderer>();

        //make firstmarker invisible on start, until startmarker collision
        rend.enabled = false;
        ring_rend.enabled = false;
        arrow_rend.enabled = false;
    }

    /*
     * called when player collides with start marker in Collide.cs
     * makes first marker appear and puts it at the appropriate length away from start marker
     * and orients it according to the degree given to it by Restart.cs
     */
    public void SpawnFirstMarker()
    {
        //make marker and its arrow appear
        rend.enabled = true;
        ring_rend.enabled = true;
        arrow_rend.enabled = true;

        //finding the rotation of start marker
        startDegree = StartMarkerSpawn.degree;

        //temporarily setting first marker to same degree of start marker
        this.transform.Rotate(0, startDegree, 0);

        // temporarily setting first marker to same position of start marker
        this.transform.position = StartMarker.transform.position;

        // now moving firstmarker forward given amount
        this.transform.Translate(Vector3.back * (1.524f + length ) * GameSettings.scale, Space.Self); //4ft + random amt

        //changing rotation of firstmarker to the same rotation plus or minus a certain amount
        degree = startDegree + Restart.degree;
        this.transform.Rotate(0, this.transform.rotation.y + Restart.degree, 0);


    }
}
