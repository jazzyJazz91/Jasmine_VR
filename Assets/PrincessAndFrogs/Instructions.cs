using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour
{
public void instructions()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
