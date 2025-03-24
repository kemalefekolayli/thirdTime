using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartSceneButton : MonoBehaviour {
   public TMP_Text buttonLabel;


    public void SetButtonText(string text) {
    buttonLabel.text = text;
    }

   public void OnStartButtonClicked() {
          SceneManager.LoadScene("SampleScene");
      }
   void Start(){

   SetButtonText("yarrak");
   }


}