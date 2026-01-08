using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // ضروري للتحكم في الأزرار والـ Slider

public class MainMenu : MonoBehaviour
{
    [Header("قائمة الخيارات - Options Menu")]
    public GameObject optionsPanel; // اسحب لوحة الخيارات (Options Panel) هنا
    public Slider volumeSlider;     // اسحب شريط الصوت (Slider) هنا

    void Start()
    {
        // إخفاء لوحة الخيارات عند تشغيل اللعبة
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        // ضبط قيمة الـ Slider لتطابق مستوى صوت اللعبة الحالي
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            // ربط الـ Slider برمجياً ليتغير الصوت عند تحريكه
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // 1. وظيفة زر البداية (Play)
    public void PlayGame()
    {
        // تحميل المشهد التالي في قائمة Build Settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // 2. وظيفة فتح لوحة الخيارات
    public void OpenOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    // 3. وظيفة إغلاق لوحة الخيارات
    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    // 4. وظيفة التحكم في مستوى الصوت
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        Debug.Log("مستوى الصوت الحالي: " + volume);
    }

    // 5. وظيفة زر الخروج (Exit)
    public void QuitGame()
    {
        Debug.Log("تم الخروج من اللعبة!"); // تظهر في المحرر فقط
        Application.Quit(); // تغلق اللعبة النهائية
    }
}