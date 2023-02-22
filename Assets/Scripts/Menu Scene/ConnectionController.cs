using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

namespace PGGE.Multiplayer
{
    public class ConnectionController : MonoBehaviourPunCallbacks
    {
        const string gameVersion = "1";

        public byte maxPlayersPerRoom = 4;

        public GameObject mConnectionProgress;
        public GameObject mBtnJoinRandomRoom;
        public GameObject mInpPlayerName;
        bool isConnecting = false;
        bool isRandomRoom = false;

        public GameObject mBtnBackToMenu;
        public GameObject mChooseRooms;
        public InputField mInputRoomName;

        public GameObject mMenuCanvas;
        public GameObject mMultiplayerCanvas;
        public GameObject mPanelListOfRooms;

        public GameObject mBtnRandomCharacter;
        public GameObject mBtnSciFi;
        public GameObject mBtnRPGHero;
        public static int mRandomNumber;
        public GameObject mChooseCharacterPanel;
        public GameObject mChooseCharacterAlert;

        private string selectedRoom;
        private string thisAction;

        private void Awake()
        {
            //This ensures we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room
            //sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            mMenuCanvas.SetActive(true);
            mMultiplayerCanvas.SetActive(false);
            mPanelListOfRooms.SetActive(false);
            mConnectionProgress.SetActive(false);
            mChooseCharacterPanel.SetActive(true);
        }

        public void JoinRandomRoom()
        {
            // on click button to load the multiplayer scene
            thisAction = "JoinRandomRoom";

            StartCoroutine(PlayButtonAudioAndOnClickListener());
        }

        public void ChooseRooms()
        {
            // player must choose their character first
            if (mChooseCharacterPanel.activeSelf == true)
            {
                StartCoroutine(ChooseCharacterAlert());
                Debug.Log("Please choose your character first");
                return;
            }
            // play the audio
            AudioSource audioSource = EventSystem.current.currentSelectedGameObject.GetComponent<AudioSource>();
            audioSource.Play();
            mPanelListOfRooms.SetActive(true);
        }

        public void JoinSpecificRoom()
        {
            thisAction = "JoinSpecificRoom";

            StartCoroutine(PlayButtonAudioAndOnClickListener());
        }

        public string OwnRoomName()
        {
            // player to create their own room
            string value = mInputRoomName.text;
            Debug.Log(value);

            if (string.IsNullOrEmpty(value))
            {
                Debug.Log("Room name is null or empty");
                value = null;
            }
            return value;
        }

        public void CreateOwnRoom()
        {
            thisAction = "CreateOwnRoom";
            OwnRoomName();
            StartCoroutine(PlayButtonAudioAndOnClickListener());
        }

        public string GetChosenRoomName()
        {
            Button chosenRoom = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            Debug.Log(chosenRoom.name);
            return chosenRoom.name;
        }

        public override void OnConnectedToMaster()
        {
            if (isConnecting && isRandomRoom)
            {
                Debug.Log("OnConnectedToMaster() was called by PUN");
                PhotonNetwork.JoinRandomRoom();
            }
            if (isConnecting && !isRandomRoom)
            {
                Debug.Log("OnConnectedToMaster() was called by PUN");
                PhotonNetwork.JoinRoom(selectedRoom);
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
            isConnecting = false;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. " +
                "No random room available" +
                ", so we create one by Calling: " +
                "PhotonNetwork.CreateRoom");

            // Failed to join a random room.
            // This may happen if no room exists or they are all full. In either case, we create a new room.
            PhotonNetwork.CreateRoom(null,
                new RoomOptions
                {
                    MaxPlayers = maxPlayersPerRoom
                });
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRoomFailed() was called by PUN. " +
                "Room to join not available" +
                ", so we create one by Calling: " +
                "PhotonNetwork.CreateRoom");

            PhotonNetwork.CreateRoom(selectedRoom,
                new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() called by PUN. Client is in a room.");
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("We load the default room for multiplayer");
                Debug.Log(PhotonNetwork.CurrentRoom);
                PhotonNetwork.LoadLevel("MultiplayerMap00");
            }
        }

        public void BackToMainMenu()
        {
            // the player to go back to the main menu scene
            thisAction = "BackToMenu";
            StartCoroutine(PlayButtonAudioAndOnClickListener());
        }

        IEnumerator PlayButtonAudioAndOnClickListener()
        {
            if (mChooseCharacterPanel.activeSelf == true)
            {
                StartCoroutine(ChooseCharacterAlert());
                Debug.Log("Please choose your character first");
                yield break;
            }
            AudioSource audioSource = EventSystem.current.currentSelectedGameObject.GetComponent<AudioSource>();
            audioSource.Play();
            Debug.Log("audio start");
            yield return new WaitForSeconds(0.3f);
            audioSource.Stop();
            Debug.Log("audio end");

            ButtonListener();
        }

        void ButtonListener()
        {
            switch (thisAction)
            {
                // different function when different cases
                case "JoinRandomRoom":
                    mPanelListOfRooms.SetActive(false);
                    mChooseRooms.SetActive(false);
                    mBtnJoinRandomRoom.SetActive(false);
                    mInpPlayerName.SetActive(false);
                    mConnectionProgress.SetActive(true);
                    isRandomRoom = true;

                    // we check if we are connected or not, we join if we are, 
                    // else we initiate the connection to the server.
                    if (PhotonNetwork.IsConnected)
                    {
                        // Attempt joining a random Room. 
                        // If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                        PhotonNetwork.JoinRandomRoom();
                    }
                    else
                    {
                        // Connect to Photon Online Server.
                        isConnecting = PhotonNetwork.ConnectUsingSettings();
                        PhotonNetwork.GameVersion = gameVersion;
                    }
                    break;

                case "JoinSpecificRoom":
                    mPanelListOfRooms.SetActive(false);
                    mChooseRooms.SetActive(false);
                    mBtnJoinRandomRoom.SetActive(false);
                    mInpPlayerName.SetActive(false);
                    mConnectionProgress.SetActive(true);

                    selectedRoom = GetChosenRoomName();

                    if (PhotonNetwork.IsConnected)
                    {
                        PhotonNetwork.JoinRoom(selectedRoom);
                    }
                    else
                    {
                        isConnecting = PhotonNetwork.ConnectUsingSettings();
                        PhotonNetwork.GameVersion = gameVersion;
                    }
                    break;

                case "CreateOwnRoom":
                    mPanelListOfRooms.SetActive(false);
                    mChooseRooms.SetActive(false);
                    mBtnJoinRandomRoom.SetActive(false);
                    mInpPlayerName.SetActive(false);
                    mConnectionProgress.SetActive(true);

                    selectedRoom = OwnRoomName();
                    Debug.Log(selectedRoom);

                    if (PhotonNetwork.IsConnected)
                    {
                        PhotonNetwork.JoinRoom(selectedRoom);
                    }
                    else
                    {
                        isConnecting = PhotonNetwork.ConnectUsingSettings();
                        PhotonNetwork.GameVersion = gameVersion;
                    }
                    break;

                case "BackToMenu":
                    mMenuCanvas.SetActive(true);
                    mMultiplayerCanvas.SetActive(false);
                    Debug.Log("Going back to main menu");
                    break;

                case null:
                    Debug.Log("No action done");
                    break;
            }
        }

        public void RandomCharacter()
        {
            mRandomNumber = Random.Range(0, 2);
            Debug.Log("You chose random character");
            StartCoroutine(ChooseCharacterAudio());
        }

        public void SciFi()
        {
            mRandomNumber = 0;
            Debug.Log("You chose Sci-Fi Character");
            StartCoroutine(ChooseCharacterAudio());
        }

        public void RPGHero()
        {
            mRandomNumber = 1;
            Debug.Log("You chose RPG Hero Character");
            StartCoroutine(ChooseCharacterAudio());
        }

        IEnumerator ChooseCharacterAlert()
        {
            mChooseCharacterAlert.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            mChooseCharacterAlert.SetActive(false);
        }

        IEnumerator ChooseCharacterAudio()
        {
            // play the sound then change the panel
            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            AudioSource audioSource = button.GetComponent<AudioSource>();
            audioSource.Play();
            Debug.Log("audio start");
            yield return new WaitForSeconds(0.3f);
            audioSource.Stop();
            Debug.Log("audio stop");

            mChooseCharacterPanel.SetActive(false);
        }
    }
}
