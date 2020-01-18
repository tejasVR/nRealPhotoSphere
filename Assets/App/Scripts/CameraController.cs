using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal
{
    public class CameraController : MonoBehaviour
    {

        public static CameraController Instance;

        //WebCamTexture webcamTexture;
        //Renderer renderer;

        [SerializeField] Picture picturePrefab;
        [SerializeField] Recording recordingPrefab;

        [SerializeField] GameObject anchor;
        [SerializeField] GameObject lookAt;

        private GameObject currentSnapshot;

        private bool isRecording;

        AudioClip tempRecording;

        //[SerializeField] Renderer screenshotRenderer;

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

            //renderer = GetComponent<Renderer>();
        }

        private void Start()
        {
            //OpenCamera();
        }

        private void OnEnable()
        {
            TriggerButton.PressedDownCallback += TakePicture;
            PlaceSnapshotButton.PressedDownCallback += PlaceSnapshot;
            RecordingButton.PressedDownCallback += Record;
        }

        private void OnDisable()
        {
            TriggerButton.PressedDownCallback -= TakePicture;
            PlaceSnapshotButton.PressedDownCallback -= PlaceSnapshot;
            RecordingButton.PressedDownCallback -= Record;

        }

        // Open the camera
        public void OpenCamera()
        {
            //webcamTexture = new WebCamTexture();
            //renderer.material.mainTexture = webcamTexture;
            //webcamTexture.Play();
        }

        private void TakePicture()
        {
            StartCoroutine(TakePhotoRoutine());
        }

        private IEnumerator TakePhotoRoutine()  // Start this Coroutine on some button click
        {
            if (currentSnapshot == null)
            {
                // NOTE - you almost certainly have to do this here:

                yield return new WaitForEndOfFrame();

                // it's a rare case where the Unity doco is pretty clear,
                // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
                // be sure to scroll down to the SECOND long example on that doco page 

                WebCamTexture webcamTexture = new WebCamTexture();
                webcamTexture = CameraTexture.Instance.webcamTexture;


                Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
                photo.SetPixels(webcamTexture.GetPixels());
                photo.Apply();

                currentSnapshot = Instantiate(picturePrefab.gameObject) as GameObject;

                currentSnapshot.GetComponent<Snapshot>().SetFollowTransform(anchor.transform);
                currentSnapshot.GetComponent<Snapshot>().SetLookAtTransform(lookAt.transform);



                currentSnapshot.GetComponent<Picture>().SetQuadTexture(photo);

                //currentSnapshot.GetComponent<Renderer>().material.mainTexture = photo;

                //screenshotRenderer.material.mainTexture = photo;

                //Encode to a PNG
                //byte[] bytes = photo.EncodeToPNG();
                //Write out the PNG. Of course you have to substitute your_path for something sensible
                //File.WriteAllBytes(your_path + "photo.png", bytes);
            }
        }

        private void PlaceSnapshot()
        {
            if (currentSnapshot != null)
            {
                currentSnapshot.GetComponent<Snapshot>().SetFollowTransform(null);
                currentSnapshot.GetComponent<Snapshot>().SetLookAtTransform(null);

                currentSnapshot = null;
            }
            else
            {
                if (Physics.Raycast(lookAt.transform.position, -lookAt.transform.up, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider.CompareTag("Screenshot"))
                    {
                        currentSnapshot = hit.collider.gameObject;

                        currentSnapshot.GetComponent<Snapshot>().SetFollowTransform(anchor.transform);
                        currentSnapshot.GetComponent<Snapshot>().SetLookAtTransform(lookAt.transform);
                    }
                }
            }
        }

        private GameObject CreateScreenshotPrefab()
        {
            var gO = Instantiate(picturePrefab.gameObject) as GameObject;

            gO.GetComponent<Snapshot>().SetFollowTransform(anchor.transform);

            return gO;
        }

        private void Record()
        {
            if (!isRecording)
            {
                StartRecording();
            }
            else
            {
                EndRecording();
            }
        }

        private void StartRecording()
        {
            tempRecording = Microphone.Start(Microphone.devices[0], true, 10, 44100);
            isRecording = true;
        }

        private void EndRecording()
        {
            Microphone.End(Microphone.devices[0]);

            currentSnapshot = Instantiate(recordingPrefab.gameObject) as GameObject;

            currentSnapshot.GetComponent<Snapshot>().SetFollowTransform(anchor.transform);
            currentSnapshot.GetComponent<Snapshot>().SetLookAtTransform(lookAt.transform);

            currentSnapshot.GetComponent<Recording>().SetAudioClip(tempRecording);

            isRecording = false;
        }
    }
}

