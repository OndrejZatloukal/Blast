using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThurst = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool collisionsDisabled = false;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

	// Use this for initialization
	void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        { 
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionsDisabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("OK");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
            state = State.Transcending;
            audioSource.Stop();
            audioSource.PlayOneShot(success);
            successParticles.Play();
            Invoke("LoadNextLevel", levelLoadDelay);   
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadCurrentLevel", levelLoadDelay);
    }

    private void LoadCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }


    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex +1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThurst * Time.deltaTime);
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
}
