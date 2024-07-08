using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    public UIDocument Menus;
    [Scene]
    public int AppartementScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Menus.rootVisualElement.Q<Button>("Play").clicked += ()=> LoadScene(AppartementScene);
    }

    private void LoadScene(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
