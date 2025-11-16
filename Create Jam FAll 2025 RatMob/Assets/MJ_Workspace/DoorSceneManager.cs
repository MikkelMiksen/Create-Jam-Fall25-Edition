using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneManager : MonoBehaviour
{
    [Header("Scene to Load When Player Touches")]
    public string sceneToLoad = "NextScene";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player touched the door
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
