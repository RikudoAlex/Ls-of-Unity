using UnityEngine;
using UnityEngine.Video;

public class MonitorControl : MonoBehaviour
{
    private VideoPlayer heartRateVideo;

    void Start()
    {
        heartRateVideo = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleMonitor();
        }
    }

   public void ToggleMonitor()
{
    Renderer rend = GetComponent<Renderer>();

    if (heartRateVideo.isPlaying)
    {
        heartRateVideo.Pause();
        rend.material.SetColor("_EmissionColor", Color.black);
    }
    else
    {
        heartRateVideo.Play();
        rend.material.SetColor("_EmissionColor", Color.white); 
    }
}
}
