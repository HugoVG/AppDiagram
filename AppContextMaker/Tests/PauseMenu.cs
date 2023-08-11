namespace AppContextMaker.Tests;

public class PauseMenu
{
    public bool Paused = false;
    public GameObject PauseMenuCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        // On escape key pause/unpause 
        PauseMenuCanvas.SetActive(!Paused); // Sets the menu 
        Time.timeScale = !Paused ? 0f : 1f;
        Cursor.visible = !Paused;
        Paused = !Paused;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}

internal class KeyCode
{
    public static object Escape { get; set; }
}

internal class Input
{
    public static bool GetKeyDown(object escape)
    {
        throw new NotImplementedException();
    }
}

public class Time
{
    public static float timeScale;
}

public class Cursor
{
    public static bool visible;
}

public class SceneManager
{
    public static void LoadScene(int buildIndex)
    {
        throw new NotImplementedException();
    }

    public static dynamic GetActiveScene()
    {
        throw new NotImplementedException();
    }
}

public class GameObject
{
    public void SetActive(bool b)
    {
        throw new NotImplementedException();
    }
}