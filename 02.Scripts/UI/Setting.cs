using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;

    [SerializeField] private Slider soundSlider;
    [SerializeField] private Image SoundSliderColor;


    [SerializeField] private Slider hapticSlider;
    [SerializeField] private Image HapticSliderColor;


    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;


    private void Awake()
    {
        if (PlayerPrefs.HasKey("sound"))
            soundSlider.value = PlayerPrefs.GetInt("sound");
        else
        {
            soundSlider.value = 1;

        }
        if (PlayerPrefs.HasKey("haptic"))
            hapticSlider.value = PlayerPrefs.GetInt("haptic");

        OnChangeSoundSlider();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void OnChangeSoundSlider()
    {
        if (soundSlider.value == 1)
        {
            SoundSliderColor.color = onColor;
            AudioListener.volume = 1;
        }
        else
        {
            SoundSliderColor.color = offColor;
            AudioListener.volume = 0;
        }

        SaveSetting();
    }

    public void OnChangeHapticSlider()
    {
        if (hapticSlider.value == 1)
        {
            HapticSliderColor.color = onColor;
        }
        else
        {
            HapticSliderColor.color = offColor;
        }

        SaveSetting();
    }

    public void OnSettingPanel()
    {
        settingPanel.SetActive(!settingPanel.activeSelf);

        if (settingPanel.activeSelf)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    private void SaveSetting()
    {
        PlayerPrefs.SetInt("sound", (int)soundSlider.value);
        PlayerPrefs.SetInt("haptic", (int)hapticSlider.value);
    }
}
