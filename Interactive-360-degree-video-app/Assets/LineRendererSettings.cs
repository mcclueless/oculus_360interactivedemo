using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LineRendererSettings : MonoBehaviour
{
    //Declare a LineRenderer to store the component attached to the GameObject.
    [SerializeField] LineRenderer rend;
    //Settings for the LineRenderer are stored as a Vector3 array of points. Set up a V3 array to //initialize in Start.
    Vector3[] points;

    public GameObject panel;
    public Button btn;
    public VideoPlayer videoPlayer;
    public GameObject interactionDisplay;

    // Array with video URLs
    string[] VideoList = {
        "https://videos1360.s3.eu-central-1.amazonaws.com/Bridge.mp4",
        "https://videos1360.s3.eu-central-1.amazonaws.com/Helpoort%201.mp4",
        "https://videos1360.s3.eu-central-1.amazonaws.com/Park.mp4"
    };

    // Boolean indicating weather the raycast is currently hitting a sphere
    bool hitSphere = false;

    //Start is called before the first frame update
    void Start()
    {
     //get the LineRenderer attached to the gameobject.
     rend = gameObject.GetComponent<LineRenderer>();

     //initialize the LineRenderer
     points = new Vector3[2];
     //set the start point of the linerenderer to the position of the gameObject.
     points[0] = Vector3.zero;
     //set the end point 20 units away from the GO on the Z axis (pointing forward)
     points[1] = transform.position + new Vector3(0, 0, 10000);
     //finally set the positions array on the LineRenderer to our new values
     rend.SetPositions(points);
     rend.enabled = true;

     // Init video
     /// Get video that is up next; index is stored with the LoadingData script
     string videoURL = VideoList[LoadingData.videoToShow];
     /// Set URL of video player
     videoPlayer.url = videoURL;
     /// Load video
     videoPlayer.Prepare();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if Sphere was hit and call function
        if (hitSphere){
            if((Input.GetAxis("Submit") != 0)){
                ShowInteraction();
            }
        }

        //Check if button was hit and propagate click event
        if(AlignLineRenderer(rend) && Input.GetAxis("Submit") > 0)
        {
            btn.onClick.Invoke();
        }
    }

    public LayerMask layerMask;
    public bool AlignLineRenderer(LineRenderer rend)
    {
     bool hitBtn = false;
     Ray ray;
     ray = new Ray(transform.position, transform.forward);

     RaycastHit hit;

     if (Physics.Raycast(ray, out hit, Mathf.Infinity))
     {
         points[1] = Vector3.zero + new Vector3(0, 0, hit.distance);
         rend.startColor = Color.green;
         rend.endColor = Color.green;

         btn = hit.collider.gameObject.GetComponent<Button>();

         // Check if sphere is hit and set hitSpehere dummy to true
         if(hit.collider.tag == "MBSphereCollider"){
             hitSphere = true;
         }

         hitBtn = true;
     }
     else
     {
         points[1] = Vector3.zero + new Vector3(0, 0, 10000);
         rend.startColor = Color.blue;
         rend.endColor = Color.blue;

         // Reset hitSphere to false because it is currently not hitting a sphere
         hitSphere = false;

         hitBtn = false;
     }
     rend.SetPositions(points);
     rend.material.color = rend.startColor;

     return hitBtn;
    }

    // Not only change color if an object is clicked, but also open new scenes
    public void ColorChangeOnClick()
    {
        // Give visual feedback by setting the line color to red
        rend.startColor = Color.red;
        rend.endColor = Color.red;
        rend.material.color = rend.startColor;

        if (btn != null)
        {
            ///////
            // Main menu actions
            ///////
            if (btn.name == "btn_chapter_select")
            {
                SceneManager.LoadScene("Assets/Scenes/Chapter_select.unity");
            }
            else if (btn.name == "btn_video_player")
            {
                SceneManager.LoadScene("Assets/Scenes/Video_player.unity");
            }
            else if (btn.name == "btn_go_to_menu")
            {
                SceneManager.LoadScene("Assets/Scenes/Welcome.unity");
            }
            ///////
            // Chapter selection
            ///////
            else if (btn.name == "btn_video_player_ch1")
            {
                LoadingData.videoToShow = 0;
                SceneManager.LoadScene("Assets/Scenes/Video_player.unity");
            }
            else if (btn.name == "btn_video_player_ch2")
            {
                LoadingData.videoToShow = 1;
                SceneManager.LoadScene("Assets/Scenes/Video_player.unity");
            }
            else if (btn.name == "btn_video_player_ch3")
            {
                LoadingData.videoToShow = 2;
                SceneManager.LoadScene("Assets/Scenes/Video_player.unity");
            }
            ///////
            // Video controls
            ///////
            else if (btn.name == "btn_video_pause")
            {
                videoPlayer.Pause();
            }
            else if (btn.name == "btn_video_play")
            {
                videoPlayer.Play();
            }
            else if (btn.name == "btn_video_restart")
            {
                videoPlayer.time = 0;
            }
            else if(btn.name == "btn_video_next")
            {
                int oldVideoIndex = LoadingData.videoToShow;

                int newVideoIndex = 0;
                if(oldVideoIndex < (VideoList.Length - 1) ){
                    newVideoIndex = oldVideoIndex + 1;
                    LoadingData.videoToShow = newVideoIndex;

                    SceneManager.LoadScene("Assets/Scenes/Loading.unity");
                }
                else{
                    newVideoIndex = 0;
                    LoadingData.videoToShow = newVideoIndex;

                    SceneManager.LoadScene("Assets/Scenes/Completed.unity");
                }
            }
            ///////
            // Interaction window
            ///////
            else if(btn.name == "btn_interaction_close")
            {
                interactionDisplay.SetActive(false);
            }

        }
    }

    // Show interaction window
    public void ShowInteraction()
    {
        interactionDisplay.SetActive(true);
    }
}
