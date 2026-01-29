using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string Game = "Game"; // 🔹 ändra till det exakta namnet på din spelscen

    public void StartGame()
    {
        SceneManager.LoadScene(Game);
    }
}
