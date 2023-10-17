using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpider : MonoBehaviour
{
    private Camera cam;

    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform target;
    [SerializeField] Transform shoulderLeft;
    [SerializeField] Transform targetBody;
    [SerializeField] Transform body;//Spine1_M
    [SerializeField] Transform targetRightHand;
    FullBodyBipedIK ik;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 startPointTarget;

    private Vector3 startPointRightHand;

    [SerializeField] Transform legRight;
    [SerializeField] Transform legLeft;
    private Vector3 pinLegRight;
    private Vector3 pinLegLeft;
    private void Awake()
    {
        cam = Camera.main;
        ik = GetComponent<FullBodyBipedIK>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = GetMousePosition();
            target.position = leftHand.position;

            targetBody.position = body.position;
            ik.solver.bodyEffector.target = targetBody;
            ik.solver.leftHandEffector.target = target;
            startPointTarget = target.position;

            targetRightHand.position = rightHand.position;
            
            startPointRightHand = rightHand.position;
            Vector3 temp = rightHand.position;
            ik.solver.leftHandEffector.positionWeight = 1f;
            ik.solver.rightHandEffector.positionWeight = 1f;
            //ik.solver.rightHandEffector.position = temp;
            ik.solver.rightHandEffector.target = targetRightHand;
            
            ik.solver.rightFootEffector.positionWeight = 1f;
            ik.solver.leftFootEffector.positionWeight = 1f;
            pinLegLeft = legLeft.position;
            pinLegRight = legRight.position;



            //ik.solver.leftHandEffector.target = null;

            //ik.solver.bodyEffector.positionWeight = 1f;
            //ik.solver.FABRIKPass = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentPoint = GetMousePosition();
            if(DistanceTwoPoint(startPointRightHand,rightHand.position)<0.1f)
            {
                target.position = startPointTarget + (currentPoint - startPoint);
            }
            //target.position = startPointTarget + (currentPoint - startPoint);

            ik.solver.leftFootEffector.position = pinLegLeft;
            ik.solver.rightFootEffector.position = pinLegRight;
        }

        if (Input.GetMouseButtonUp(0))
        {
            ik.solver.rightFootEffector.positionWeight = 0f;
            ik.solver.leftFootEffector.positionWeight = 0f;

            ik.solver.leftHandEffector.target = null;
            ik.solver.leftHandEffector.positionWeight = 0f;
            //ik.solver.rightHandEffector.positionWeight = 0f;
            endPoint = GetMousePosition();
            Debug.Log("Nha chuot ra " + endPoint);
            ik.solver.rightHandEffector.positionWeight = 0;

            ik.solver.rightHandEffector.target = null;
        }
    }

    //private void LateUpdate()
    //{
    //    ik.solver.leftFootEffector.position = pinLegLeft;
    //    ik.solver.rightFootEffector.position = pinLegRight;
    //}
    public Vector3 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        return cam.ScreenToWorldPoint(mousePos);
    }
    private float DistanceTwoPoint(Vector3 point1 ,Vector3 point2)
    {
        Vector2 vec1 = (Vector2)point1;
        Vector2 vec2 = (Vector2)point2;
        return Vector2.Distance(vec1, vec2);
    }
}
