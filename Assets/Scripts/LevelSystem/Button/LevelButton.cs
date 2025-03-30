using UnityEngine;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(Collider2D))]
public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private int maxLevel = 10; // Toplam seviye sayısı

    private int currentLevel = 1;

    private void Start()
    {
        LoadCurrentLevel();
        UpdateButtonText();
    }


    private void LoadCurrentLevel()
    {
        // PlayerPrefs'ten kayıtlı seviyeyi alır veya varsayılan olarak 1 kullanır
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    private void UpdateButtonText()
    {
        if (levelText != null)
        {
            if (currentLevel > maxLevel)
            {
                levelText.text = "Finished";
            }
            else
            {
                levelText.text = $"Level {currentLevel}";
            }
        }
        else
        {
            // TextMeshProUGUI bileşeni bulunamazsa, çocuk objelerden bulmaya çalış
            levelText = GetComponentInChildren<TextMeshProUGUI>();

            if (levelText != null)
            {
                UpdateButtonText(); // Şimdi bulunan text ile tekrar dene
            }
            else
            {
                Debug.LogError("LevelButton'da TextMeshProUGUI bileşeni bulunamadı!");
            }
        }
    }

    private void OnMouseDown()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        // Eğer tüm seviyeler tamamlandıysa hiçbir şey yapma
        if (currentLevel > maxLevel)
        {
            Debug.Log("Tüm seviyeler tamamlandı!");
            return;
        }
         UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");

    }

    /// <summary>
    /// Seviye ilerlemesini dışarıdan güncelleme için public metot
    /// </summary>
    public void UpdateLevel(int newLevel)
    {
        currentLevel = newLevel;
        UpdateButtonText();
    }
}