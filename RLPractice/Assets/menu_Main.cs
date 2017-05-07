using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menu_Main : MonoBehaviour {

    public Button start, exit;

    void Start()
    {
        Debug.Log(start.transform.position);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }

    public void ExitGame()
    {

    }
}
