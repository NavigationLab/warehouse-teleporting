using UnityEngine;

/*
 * Deals with player collsion with markers
 * Will take appropriate action on collision (usually disappearing collided marker and spawning next one)
 */ 

public class MarkerDestroy : MonoBehaviour {

    public FirstMarkerSpawn otherScript;
    public SecondMarkerSpawn otherScript2;

    public void OnTriggerEnter(Collider other)
    {
        //if the cube (moving with the player) collides with the start marker
        if (other.gameObject.CompareTag("Marker")) {
            // Make a note of that in the data output
            System.IO.File.AppendAllText(GameSettings.time_file, "Start marker time: " + Time.time + "\r\n"); 
            other.gameObject.SetActive(false); //make start marker disappear
            otherScript.SpawnFirstMarker(); //call on first marker script to spawn the first marker
        }

        //if the player collides with the first marker, make it disappear and spawn the second marker
        else if (other.gameObject.CompareTag("FirstMarker")) {
            other.gameObject.SetActive(false);
            otherScript2.SpawnSecondMarker();
        }

        //if the player collides with the second marker, make it disappear and write that to file
        else if (other.gameObject.CompareTag("SecondMarker")) {
            System.IO.File.AppendAllText(GameSettings.time_file, "Second marker time: " + Time.time + "\r\n");
            other.gameObject.SetActive(false);
        }
    }
}
