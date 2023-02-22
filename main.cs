using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject character;
    public GameObject cameraCenter;
    public float yOffset = 1f;
    public float sensitivity = 3f;
    public Camera camera;

    public float scrollSensitivity = 5f;
    public float scrollDampening = 6f;

    public float minZoom = 3.5f;
    public float maxZoom = 15f;
    public float defZoom = 10f;
    public float zoomDistance;

    public float collisionSensitivity = 4.5f;

    public RaycastHit _camHit;
    private Vector3 _camDist;

    private void Start()
    {
        _camDist = camera.transform.localPosition;
        zoomDistance = defZoom;
        _camDist.z = zoomDistance;

        Cursor.visible = false;
    }

    private void Update()
    {
        cameraCenter.transform.position = new Vector3(character.transform.position.x,
            character.transform.position.y + yOffset, character.transform.position.z);

        var rotation = Quaternion.Euler(
            cameraCenter.transform.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity / 2,
            cameraCenter.transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * sensitivity / 2,
            cameraCenter.transform.rotation.eulerAngles.z);
        
        cameraCenter.transform.rotation = rotation;

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            var scrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
            scrollAmount *= zoomDistance * 0.3f;
            zoomDistance += -scrollAmount;
            zoomDistance = Mathf.Clamp(zoomDistance, minZoom, maxZoom);
        }

        if (_camDist.z != -zoomDistance)
        {
            _camDist.z = Mathf.Lerp(_camDist.z, -zoomDistance, Time.deltaTime * scrollDampening);
        }

        camera.transform.localPosition = _camDist;

        GameObject obj = new GameObject();
        obj.transform.SetParent(camera.transform.parent);
        obj.transform.localPosition = new Vector3(camera.transform.localPosition.x, camera.transform.localPosition.y,
            camera.transform.localPosition.z - collisionSensitivity);

        if (Physics.Linecast(cameraCenter.transform.position, obj.transform.position, out _camHit))
        {
            camera.transform.position = _camHit.point;

            var localposition = new Vector3(camera.transform.localPosition.x, camera.transform.localPosition.y,
                camera.transform.localPosition.z + collisionSensitivity);

            camera.transform.localPosition = localposition;
        }
        Destroy(obj);

        if (camera.transform.localPosition.z > -1f)
        {
            camera.transform.localPosition =
                new Vector3(camera.transform.localPosition.x, camera.transform.localPosition.y, -1f);
        }
    }
}
