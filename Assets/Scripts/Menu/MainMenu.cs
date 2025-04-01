using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("Кнопка 'Играть' нажата. Попытка загрузить GameScene...");
        if (SceneManager.GetSceneByName("GameScene") != null)
        {
            SceneManager.LoadScene("GameScene"); // Запуск сцены игры
        }
        else
        {
            Debug.LogError("Сцена 'GameScene' не найдена! Добавьте её в Build Settings.");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Кнопка 'Выход' нажата. Закрытие приложения...");
        Application.Quit(); // Выход из игры

        // Для отладки в редакторе Unity:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
