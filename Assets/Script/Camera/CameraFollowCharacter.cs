using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCharacter : MonoBehaviour
{
    #region Component
    private Camera MainCamera;
    #endregion
    #region Public
    public GameObject LeftBorder;
    public GameObject RightBorder;
    public GameObject TopBorder;
    public GameObject BottomBorder;
    public GameObject Character;
    #endregion
    #region Private
    private float CameraWidth;
    private float CameraHeight;
    private float YBelowValue;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Set Data for camera
        MainCamera = GetComponent<Camera>();
        CameraHeight = Camera.main.orthographicSize * 2;
        CameraWidth = CameraHeight * Camera.main.aspect;
        YBelowValue = transform.position.y - Character.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector2 ObjectPos = Character.transform.position;
        Vector2 CameraPos = new();
        // Check if Camera's left border is outside of border pos, if yes move the x pos of camera to the left border's pos
        if (ObjectPos.x - CameraWidth / 2 < LeftBorder.transform.position.x)
        {
            CameraPos.x = LeftBorder.transform.position.x + CameraWidth / 2;
        }
        // same for right
        else if (ObjectPos.x + CameraWidth / 2 > RightBorder.transform.position.x)
        {
            CameraPos.x = RightBorder.transform.position.x - CameraWidth / 2;
        }
        // If both is false then set it normally
        else
        {
            CameraPos.x = ObjectPos.x;
        }
        // Check if Camera's top border is outside of border pos, if yes move the y pos of camera to the top border's pos
        if (ObjectPos.y + YBelowValue + CameraHeight / 2 > TopBorder.transform.position.y)
        {
            CameraPos.y = TopBorder.transform.position.y - CameraHeight / 2;
        }
        // same for bottom
        else if (ObjectPos.y + YBelowValue - CameraHeight / 2 < BottomBorder.transform.position.y)
        {
            CameraPos.y = BottomBorder.transform.position.y + CameraHeight / 2;
        }
        // If both is false then set it normally + offset value
        else
        {
            CameraPos.y = ObjectPos.y + YBelowValue;
        }
        transform.position = new Vector3(CameraPos.x, CameraPos.y, transform.position.z);
    }
}
