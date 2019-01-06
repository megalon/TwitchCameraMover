using System;
using System.Linq;
using CustomUI.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using CameraPlus;

namespace TwitchCameraMover
{
    class CameraMover : MonoBehaviour
    {
        public static CameraMover Instance;
        private Camera cam;
        private Vector3 lookAtPosition = new Vector3(0, 1f, 0f);
        public Vector3 targetPosition;
        public GameObject objToLookAt;
        private float lerpSpeed = 2f;
        private float minDistance = 2.5f;

        private static GameObject newCube = null;
        private Transform _headTransform;
        private static FloorAdjustViewController floorAdjustViewController;
        private static float _playerHeight = 1;

        public static void OnLoad()
        {
            SubMenu subMenu = SettingsUI.CreateSubMenu("Twitch Camera Mover");


            // Create the gameobject
            if (CameraMover.Instance == null)
            {
                new GameObject("CameraMover").AddComponent<CameraMover>();
            }

            if (newCube == null)
            {
                newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DontDestroyOnLoad(newCube);
                newCube.SetActive(true);
                newCube.transform.localScale = new Vector3(0.15f, 0.15f, 0.22f);
                newCube.name = "CustomCameraCube";
                newCube.transform.position = new Vector3(0, 1.5f, 0.5f);
            }

            if (floorAdjustViewController == null)
            {
                getPlayerHeight();
            }
        }

        public void Awake()
        {
            if (CameraMover.Instance == null)
            {
                CameraMover.Instance = this;
                DontDestroyOnLoad(base.gameObject);

                if (objToLookAt == null)
                {
                    objToLookAt = new GameObject();
                    objToLookAt.transform.SetPositionAndRotation(lookAtPosition, objToLookAt.transform.rotation);
                    DontDestroyOnLoad(objToLookAt);
                }

                findCamera();
            }
            else
            {
                Destroy(this);
            }
        }

        public void SceneManagerOnActiveSceneChanged( Scene oldScene, Scene newScene)
        {
            if (newScene.name == "GameCore")
            {
                getHeadTransform();
                getPlayerHeight();
                objToLookAt.transform.position.Set(objToLookAt.transform.position.x, _playerHeight, objToLookAt.transform.position.z);
            }
            else
            {
                _headTransform = null;
            }
        }

        public void LateUpdate()
        {
            if (cam == null)
            {
                Plugin.Log("Could not update CameraMover, cam is null!", Plugin.LogLevel.Error);
                return;
            }
            if (targetPosition == null)
            {
                Plugin.Log("targetPosition is null!", Plugin.LogLevel.Error);
                return;
            }

            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            cam.transform.LookAt(objToLookAt.transform);

            newCube.transform.position = cam.transform.position;
            newCube.transform.rotation = cam.transform.rotation;

            // Disable the cube if it's to close to the player
            if (_headTransform != null)
            {
                var distance = Vector3.Distance(newCube.transform.position, _headTransform.position);
                if (distance <= minDistance)
                {
                    if (newCube.GetComponent<MeshRenderer>().enabled)
                        newCube.GetComponent<MeshRenderer>().enabled = false;
                }else
                {
                    if (!newCube.GetComponent<MeshRenderer>().enabled)
                        newCube.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }

        public void findCamera()
        {   
            Plugin.Log("Finding the main camera", Plugin.LogLevel.Debug);

            int i = 0;
            foreach (var c in Camera.allCameras)
            {
                Plugin.Log(string.Format("Camera {0} | name: {1} | position: {2}", i, c.name, c.transform.position.ToString()), Plugin.LogLevel.Debug);
                ++i;

                if (c.name.Equals("cameraplus.cfg"))
                {
                    cam = c;
                }
            }

            if (cam != null) Plugin.Log("Found main camera!", Plugin.LogLevel.Debug);
            else Plugin.Log("Could not find main camera!", Plugin.LogLevel.Error);

            targetPosition = cam.transform.position;
        }

        public void getHeadTransform()
        {
            var playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
            _headTransform = ReflectionUtil.GetPrivateField<Transform>(playerController, "_headTransform");
        }

        public static void getPlayerHeight()
        {
            floorAdjustViewController = Resources.FindObjectsOfTypeAll<FloorAdjustViewController>().FirstOrDefault();
            if (floorAdjustViewController == null)
            {
                Plugin.Log("Could not find floorAdjustViewController!", Plugin.LogLevel.Error);
                return;
            }
            _playerHeight = ReflectionUtil.GetPrivateField<float>(floorAdjustViewController, "_playerHeight");
        }


        float minCameraPos = -30f;
        float maxCameraPos = 30f;

        // Changed x, y, z to x, z, y so it's 
        // Left/Right, Forward/Back, Up/Down
        public void moveCamera(float x, float y, float z)
        {
            x = Mathf.Clamp(x, minCameraPos, maxCameraPos);
            y = Mathf.Clamp(y, minCameraPos, maxCameraPos);
            z = Mathf.Clamp(z, minCameraPos, maxCameraPos);

            Plugin.Log(string.Format("Attempting to move camera to position | x:{0} y:{2} z:{1}", x, y, z), Plugin.LogLevel.Info);
            if (cam == null)
            {
                Plugin.Log("Could not move camera, camera is null!", Plugin.LogLevel.Error);
                return;
            }

            // NOTE THAT THIS IS x, z, y on purpose!
            Plugin.Log("New cam position: {0}" + cam.transform.position, Plugin.LogLevel.Debug);
            targetPosition = new Vector3(x, z, y);
        }
    }
}
