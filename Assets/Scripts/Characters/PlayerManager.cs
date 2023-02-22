using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.SceneManagement;
using PGGE.Multiplayer;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public string mPlayerPrefabName;
    public PlayerSpawnPoints mSpawnPoints;

    [HideInInspector]
    public GameObject mPlayerGameObject;
    [HideInInspector]
    private ThirdPersonCamera mThirdPersonCamera;

    // Start is called before the first frame update
    void Start()
    {
        RandomCharacter();
        StartCoroutine(Coroutine_DelayPlayerLoad(1.0f));
    }

    void RandomCharacter()
    {
        if (ConnectionController.mRandomNumber == 0)
            mPlayerPrefabName = "Prefabs/SciFiPlayer_Networked";
        else if (ConnectionController.mRandomNumber == 1)
            mPlayerPrefabName = "Prefabs/RPGHeroHP_Networked";
    }

    IEnumerator Coroutine_DelayPlayerLoad(float secs)
    {
        yield return new WaitForSeconds(secs);

        CreatePlayer();
    }

    void CreatePlayer()
    {
        mPlayerGameObject = PhotonNetwork.Instantiate(mPlayerPrefabName,
            mSpawnPoints.GetSpawnPoint().position,
            mSpawnPoints.GetSpawnPoint().rotation,
            0);

        switch (mPlayerPrefabName)
        {
            case "Prefabs/SciFiPlayer_Networked":
                mPlayerGameObject.GetComponent<PlayerMovement>().mFollowCameraForward = false;
                break;
            case "Prefabs/RPGHeroHP_Networked":
                mPlayerGameObject.GetComponent<Player2Movement>().mFollowCameraForward = false;
                break;
            default:
                Debug.Log("No player prefabs selected");
                break;
        }
        mThirdPersonCamera = Camera.main.gameObject.AddComponent<ThirdPersonCamera>();
        mThirdPersonCamera.mPlayer = mPlayerGameObject.transform;
        mThirdPersonCamera.mDamping = 5.0f;
        mThirdPersonCamera.mPositionOffset = new Vector3(1.0f, 2.0f, -3.0f);
        mThirdPersonCamera.mCameraType = CameraType.Follow_Track_Pos_Rot;
    }

    public void LeaveRoom()
    {
        StartCoroutine(PlayButtonAudioAndOnClickListener());
    }

    public override void OnLeftRoom()
    {
        Debug.LogFormat("OnLeftRoom()");
        SceneManager.LoadScene("Menu");
    }

    IEnumerator PlayButtonAudioAndOnClickListener()
    {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        AudioSource audioSource = button.GetComponent<AudioSource>();
        audioSource.Play();
        Debug.Log("audio start");
        yield return new WaitForSeconds(0.3f);
        audioSource.Stop();
        Debug.Log("audio end");

        Debug.LogFormat("LeaveRoom");
        PhotonNetwork.LeaveRoom();
    }
}
