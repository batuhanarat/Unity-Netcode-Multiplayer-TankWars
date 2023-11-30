using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private  int MAX_LENGTH = 12;
    [SerializeField] private  int MIN_LENGTH = 3;
    
    public const string PlayerNameKey = "PlayerName";

   private  void Start()
   {
       if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
       {
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
           return;
       }
       nameInputField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
       HandleNameChanged();
   }

   public void HandleNameChanged()
   {
       confirmButton.interactable = (nameInputField.text.Length >= MIN_LENGTH
                                     && nameInputField.text.Length <= MAX_LENGTH);

   }

   public void Connect()
   {
       PlayerPrefs.SetString(PlayerNameKey, nameInputField.text);
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
   }

}
