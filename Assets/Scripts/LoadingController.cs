﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private Button _localButton;
    [SerializeField] private Button _cloudButton;

    // Start is called before the first frame update
    private void Start()
    {
        // tambahkan onclick listener
        _localButton.onClick.AddListener(() =>
        {
            // disable button agar tidak spam
            SetButtonInteractable(false);
            UserDataManager.LoadFromLocal();
            SceneManager.LoadScene(1);
        });
        _cloudButton.onClick.AddListener(() =>
        {
            SetButtonInteractable(false);
            StartCoroutine(UserDataManager.LoadFromCloud(() => SceneManager.LoadScene(1)));
        });
        //UserDataManager.Load();
        //SceneManager.LoadScene(1);
    }

    private void SetButtonInteractable(bool interactable)
    {
        _localButton.interactable = interactable;
        _cloudButton.interactable = interactable;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
