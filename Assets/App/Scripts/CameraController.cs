using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal
{
    public class CameraController : MonoBehaviour
    {

        public static CameraController Instance;

        WebCamTexture webcamTexture;
        Renderer renderer;

        [SerializeField] Renderer screenshotRenderer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            renderer = GetComponent<Renderer>();
        }

        private void Start()
        {
            OpenCamera();
        }

        private void OnEnable()
        {
            TriggerButton.PressedDownCallback += TakePicture;
                
        }

        private void OnDisable()
        {
            TriggerButton.PressedDownCallback -= TakePicture;
        }

        // Open the camera
        public void OpenCamera()
        {
            webcamTexture = new WebCamTexture();
            renderer.material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }

        public void TakePicture()
        {
            StartCoroutine(TakePhotoRoutine());
        }

        private IEnumerator TakePhotoRoutine()  // Start this Coroutine on some button click
        {
            // NOTE - you almost certainly have to do this here:

            yield return new WaitForEndOfFrame();

            // it's a rare case where the Unity doco is pretty clear,
            // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
            // be sure to scroll down to the SECOND long example on that doco page 

            Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
            photo.SetPixels(webcamTexture.GetPixels());
            photo.Apply();

            screenshotRenderer.material.mainTexture = photo;

            //Encode to a PNG
            //byte[] bytes = photo.EncodeToPNG();
            //Write out the PNG. Of course you have to substitute your_path for something sensible
            //File.WriteAllBytes(your_path + "photo.png", bytes);
        }
    }

}

