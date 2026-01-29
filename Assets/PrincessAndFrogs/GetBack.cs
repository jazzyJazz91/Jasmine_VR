using UnityEngine;
using UnityEngine.SceneManagement;

public class GetBack : MonoBehaviour
{
    // Anropa denna metod från din knapp i UI (OnClick)
    public void BackToMainMenu()
    {
        // Ladda scenen med namnet "MainMenu"
        SceneManager.LoadScene("Main menu");
    }
}
