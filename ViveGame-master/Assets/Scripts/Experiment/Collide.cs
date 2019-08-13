using UnityEngine;

/*changes the center of the cube that collides with the marker to match where the player's eye is (HMD position in the tracked space)*/

public class Collide : MonoBehaviour
{
    //represents the camera (player position in tracked space)
    private GameObject cam; 

    void Start()
    {
        cam = GameObject.Find("Camera (eye)");
    }

    void Update()
    {
        //sets the center of the cube (box collider) to the camera location
        this.transform.position = cam.GetComponent<Transform>().position;
    }
}
