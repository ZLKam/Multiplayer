using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour
{
    public AudioSource footstepSound;
    public List<AudioClip> footstepConcreteClipList;
    public List<AudioClip> footstepDirtClipList;
    public List<AudioClip> footstepMetalClipList;
    public List<AudioClip> footstepSandClipList;
    public List<AudioClip> footstepWoodClipList;

    [SerializeField] private List<AudioClip> footStepList;

    private int clipToUse;

    void Update()
    {
        //change the footStepList when the ground texture change
        //if (canvasController.groundTextureDropdown.options[canvasController.groundTextureDropdown.value].text == "Concrete")
        //{
        //    footStepList = footstepConcreteClipList;
        //    footstepSound.clip = footstepConcreteClipList[clipToUse];
        //}
        //else if (canvasController.groundTextureDropdown.options[canvasController.groundTextureDropdown.value].text == "Dirt")
        //{
        //    footStepList = footstepDirtClipList;
        //    footstepSound.clip = footstepDirtClipList[clipToUse];
        //}
        //else if (canvasController.groundTextureDropdown.options[canvasController.groundTextureDropdown.value].text == "Metal")
        //{
        //    footStepList = footstepMetalClipList;
        //    footstepSound.clip = footstepMetalClipList[clipToUse];
        //}
        //else if (canvasController.groundTextureDropdown.options[canvasController.groundTextureDropdown.value].text == "Sand")
        //{
        //    footStepList = footstepSandClipList;
        //    footstepSound.clip = footstepSandClipList[clipToUse];
        //}
        //else if (canvasController.groundTextureDropdown.options[canvasController.groundTextureDropdown.value].text == "Wood")
        //{
        //    footStepList = footstepWoodClipList;
        //    footstepSound.clip = footstepWoodClipList[clipToUse];
        //}
    }

    public void FootStepSound()
    {
        // random the audio and volume to be play
        clipToUse = Random.Range(0, footStepList.Count);
        footstepSound.volume = Random.Range(0.3f, 1);
        footstepSound.Play();
    }
}
