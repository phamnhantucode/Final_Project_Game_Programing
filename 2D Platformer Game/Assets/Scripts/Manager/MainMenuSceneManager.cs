using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneManager : MonoBehaviour
{
    void Start()
    {
        ApplicationModel.CurrentScene = (int)Loader.Scene.MainMenu;
        SoundManager.Instance.Play(SoundManager.SoundTags.Ambiance1);
    }

    public void PlayGame()
    {
        ApplicationModel.PreviousScene = ApplicationModel.CurrentScene;
        ApplicationModel.CurrentScene = (int)Loader.Scene.Scene2;
        SceneManager.LoadScene(Loader.Scene.Loading.ToString());
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
