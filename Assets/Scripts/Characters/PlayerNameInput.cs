using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerNameInput : MonoBehaviour
{
    const string playerNamePrefKey = "PlayerName";
    private InputField mInputField;

    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        mInputField = this.transform.Find("PlayerNameInput").GetComponent<InputField>();
        if (mInputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                // to check that if there is this key call "PlayerName" store in PlayerPrefs
                // then get its value stored in PlayerPrefs of this key
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                mInputField.text = defaultName;
            }
        }
        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName()
    {
        string value = mInputField.text;
        if (string.IsNullOrEmpty(value))
        {
            Debug.Log("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;
        // set the value to PlayerPrefs with the key call "PlayerName" so that the value will be saved whenever player comes to this page
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
}
