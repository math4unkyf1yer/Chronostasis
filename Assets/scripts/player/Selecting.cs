using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Selecting : MonoBehaviour
{
    public Camera cam; // Assign your camera (usually the Main Camera)
    public LineRenderer lineRenderer;
    public float lineLength = 14f;
    public LayerMask raycastLayers;
    private OutlineHighlighter lastHighlighted;
    private OutlineHighlighter highligtedCl;
    private bool selectedObject;

    public PostProcessVolume grayscaleVolume;
    private bool isGrayscale = false;

    private bool isRewinding = false;
    private Coroutine rewindCoroutine = null;
    private GameObject rewindingObject = null;
    private ColorGrading colorGrading;

    [Header("Swap")]
    public bool swap;
    private GameObject player;

    private string saveTypeRb;

    [Header("Grabbing")]
    public GameObject objectOn;
    private GameObject grabbedObject;
    public bool isGrabbing;
    public bool cannotGrabble = false;
    private Rigidbody grabbedRigid;
    private Vector3 previousPos;
    private Vector3 currentVelocity;
    private float grabDistance;
    public float maxThrowSpeed = 10f;
    private float maxDistance;
    private float minDistance = 3f;
    public float scrollSpeed = 2f;
    private PlatformMove platformScript;

    [Header("Beam")]
    public float beamOffseyrs = 1.5f;
    public LineRenderer beamRenderer;

    [Header("Cooldowns")]
    public CoolDownAbility coolDownScript;
    private bool isOnCoolDown = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        maxDistance = lineLength;
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // Set line renderer to have 2 points
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the starting position (camera position)
        Vector3 startPos = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        // Get the direction the camera is facing and multiply by lineLength
        Vector3 endPos = startPos + direction * lineLength;

        StopTragectory();

        // Raycast
        if (Physics.Raycast(startPos, direction, out RaycastHit hitInfo, lineLength)) 
        {
            // Check if the first hit object's layer is included in raycastLayers //make sure can ' go trough objects 
            if (((1 << hitInfo.collider.gameObject.layer) & raycastLayers) != 0 && !selectedObject && grabbedObject == null)
            {
                endPos = hitInfo.point;

                OutlineHighlighter highlighter = hitInfo.collider.GetComponent<OutlineHighlighter>();

                if (highlighter != null)
                {
                    if (lastHighlighted != null && lastHighlighted != highlighter)
                        lastHighlighted.Unhighlight();

                    highlighter.Highlight();
                    lastHighlighted = highlighter;
                    highligtedCl = highlighter;
                }

                GameObject target = hitInfo.collider.gameObject;

                if (target.CompareTag("notGrab"))
                {
                    cannotGrabble = true;
                }
                else
                {
                    cannotGrabble = false;
                }

                // Set grabbedObject only when left click
                if (Input.GetMouseButtonDown(0) && !isGrayscale && !cannotGrabble)
                {
                    grabbedObject = target;
                    grabbedRigid = grabbedObject.GetComponent<Rigidbody>();

                    if (grabbedRigid != null)
                    {
                        grabbedRigid.useGravity = false;
                        grabbedRigid.velocity = Vector3.zero;
                        grabbedRigid.angularVelocity = Vector3.zero;
                    }

                    grabDistance = Vector3.Distance(cam.transform.position, grabbedObject.transform.position);
                    previousPos = grabbedObject.transform.position;
                }

                // Rewind trigger
                if (Input.GetMouseButtonDown(1) && !isGrayscale && !isOnCoolDown)
                {
                    grabbedObject = target;
                    grabbedRigid = grabbedObject.GetComponent<Rigidbody>();
                    if(swap == false)
                    {
                        lastHighlighted.selectedHighlight("yellow");
                        target.layer = LayerMask.NameToLayer("Highlighted");
                        StartCoroutine(ApplyGrayscale(lastHighlighted, target));
                    }
                    else
                    {
                        if (!grabbedObject.CompareTag("notGrab"))
                        {
                            lastHighlighted.selectedHighlight("purple");
                            target.layer = LayerMask.NameToLayer("Highlighted");
                            StartCoroutine(SwapStart(target, lastHighlighted));
                        }
                    }
                }
            }
            else
            {
                // If first hit was not a valid layer, clear highlight
                ClearLastHighlight();
            }
        }
        else
        {
            ClearLastHighlight();
        }

        Grabbing();

        // Update the Line Renderer
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    void Grabbing()
    {
        if(grabbedObject != null)
        {
            if (grabbedObject.CompareTag("notGrab") || grabbedObject == objectOn)
            {
                //remove line renderer 
                isGrabbing = false;
                BeamDeactive();
                return; // Prevent grabbing
            }
            if (Input.GetMouseButton(0) && !cannotGrabble)
            {
                if (cannotGrabble == true)
                    return;
                BeamActive();
                LookBeam();
                isGrabbing = true;
                if(grabbedObject.gameObject.tag == "Platform" && grabbedRigid.isKinematic)
                {
                    grabbedRigid.isKinematic = false;
                }
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                int maxSpeed = 20;

                grabDistance += scroll * scrollSpeed;              // Update distance
                grabDistance = Mathf.Clamp(grabDistance, minDistance, maxDistance);

                Vector3 targetPos = cam.transform.position + cam.transform.forward * grabDistance;
                Vector3 toTarget = targetPos - grabbedRigid.position;

                // Calculate a desired velocity with smoothing
                // Smooth it by limiting max acceleration or by interpolating velocity

                // Option 1: Simple proportional velocity (like a spring)
                float smoothTime = 0.1f; // smaller = snappier, bigger = smoother
                Vector3 desiredVelocity = toTarget / smoothTime;
                currentVelocity = desiredVelocity;

                // Clamp to max speed
                desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

                grabbedRigid.velocity = desiredVelocity;

                previousPos = grabbedRigid.position;
                
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (grabbedRigid != null)
                {
                    isGrabbing = false;
                    if(grabbedRigid.gameObject.tag == "objGravity" || grabbedRigid.gameObject.tag == "PlatformG")
                    {
                        grabbedRigid.useGravity = true;
                        grabbedRigid.velocity = Vector3.ClampMagnitude(currentVelocity, maxThrowSpeed);
                    }
                    if (grabbedRigid.gameObject.tag == "Platform")
                    {
                            grabbedRigid.isKinematic = true;                        
                    }
                    grabbedRigid = null;
                    BeamDeactive();
                }
                grabbedObject = null;
            }
        }
    }

    void LookBeam()
    {
        Vector3 start = transform.position + transform.forward * beamOffseyrs;
        Vector3 end = grabbedObject.transform.position;

        int pointCount = 15;
        beamRenderer.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            // Add curve using a sine wave (curve up or down)
            Vector3 dragOffset = transform.forward * Mathf.Sin(t * Mathf.PI) * 1.5f;
            point += dragOffset;

            beamRenderer.SetPosition(i, point);
        }
    }
    void BeamActive()
    {
        beamRenderer.enabled = true;
    }
    void BeamDeactive()
    {
        beamRenderer.enabled = false;
    }
    void ClearLastHighlight()
    {
        if (lastHighlighted != null)
        {
            lastHighlighted.Unhighlight();
            lastHighlighted = null;
        }
    }

    IEnumerator ApplyGrayscale(OutlineHighlighter outlineScript, GameObject hitObject)
    {
        selectedObject = true;
        isGrayscale = true;
        isRewinding = true;
        rewindingObject = hitObject;
        grayscaleVolume.enabled = true;

        //make it kinematic 
        if (grabbedRigid.useGravity)
        {
            saveTypeRb = "Gravity";
            grabbedRigid.useGravity = false;
            grabbedRigid.isKinematic = true;
        }
        else
        {
            saveTypeRb = "Kinematic";
        }
        TrailRenderer objTrailRenderer = hitObject.GetComponent<TrailRenderer>();
        LineRenderer objLineRend = hitObject.GetComponent<LineRenderer>();

        PositionTracker tracker = hitObject.GetComponent<PositionTracker>();
        List<TransformSnapshot> history = null;

        checkIfPlatform(hitObject);

        if (tracker != null)
        {
            history = tracker.GetFullHistory();
        }

        if (history != null && history.Count > 0)
        {
            if (objTrailRenderer != null)
                objTrailRenderer.enabled = false;
            // Move back through the recorded positions/rotations
            rewindCoroutine = StartCoroutine(ReplayTrajectory(hitObject, history, tracker.recordInterval));
            yield return rewindCoroutine;
            objLineRend.enabled = false;
            if (objTrailRenderer != null)
                objTrailRenderer.enabled = true;
            isRewinding = false;
        }
        outlineScript.UnselectedHighlight();
        var rb = rewindingObject.GetComponent<Rigidbody>();
   //     StopVelocity(rb);
        hitObject.layer = LayerMask.NameToLayer("MoveObject");
        grayscaleVolume.enabled = false;
        isGrayscale = false;
        selectedObject = false;
        if(objTrailRenderer != null)
            objTrailRenderer.enabled = true;
        rewindingObject = null;
        if (platformScript != null)
        {
            platformScript.platformRewinding = false;
            platformScript = null;
        }
        CheckGravOrKin();
        StartCoroutine(CooldownRewind());
    }
    void CheckGravOrKin()
    {
        if(saveTypeRb == "Gravity")
        {
            grabbedRigid.useGravity = true;
            grabbedRigid.isKinematic = false;
        }
        else
        {
            //nothing
        }
        grabbedObject = null;
        grabbedRigid = null;
    }
    void checkIfPlatform(GameObject hitObject)
    {
        if (hitObject.GetComponent<PlatformMove>() != null)
            platformScript = hitObject.GetComponent<PlatformMove>();

        if (platformScript != null)
        {
            platformScript.StartRewind();
        }
    }
    IEnumerator ReplayTrajectory(GameObject obj, List<TransformSnapshot> history, float interval)
    {
        LineRenderer objLineRend = obj.GetComponent<LineRenderer>();
        if (objLineRend == null) yield break;

        objLineRend.enabled = true;
        objLineRend.positionCount = history.Count;

        // Set all positions up front
        for (int i = 0; i < history.Count; i++)
        {
            objLineRend.SetPosition(i, history[i].position);
        }
        // Play history in reverse (from newest to oldest)
        for (int i = history.Count - 1; i > 0; i--)
        {
            Vector3 startPos = history[i].position;
            Quaternion startRot = history[i].rotation;
            Vector3 endPos = history[i - 1].position;
            Quaternion endRot = history[i - 1].rotation;

            float elapsed = 0f;
            while (elapsed < interval)
            {
                if (!isRewinding) yield break;
                float t = elapsed / interval;
                Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
                obj.transform.position = currentPos;
                obj.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                elapsed += Time.deltaTime;

                yield return null;
            }
        }
    }

    void StopTragectory()
    {
        if (Input.GetMouseButtonDown(1) && isRewinding && rewindCoroutine != null && isOnCoolDown == false)
        {
            highligtedCl.UnselectedHighlight();
            Debug.Log("Stopping rewind.");
            isRewinding = false;

            if (rewindCoroutine != null)
            {
                StopCoroutine(rewindCoroutine);
                rewindCoroutine = null;
            }

            if (rewindingObject != null)
            {
                // Reset object visuals/state if needed
                var objTrail = rewindingObject.GetComponent<TrailRenderer>();
                var objLine = rewindingObject.GetComponent<LineRenderer>();
                if (objLine != null) objLine.enabled = false;
                if (objTrail != null) objTrail.enabled = true;
                // 🆕 Reset Rigidbody velocity
                var rb = rewindingObject.GetComponent<Rigidbody>();
            //    StopVelocity(rb);
                rewindingObject.layer = LayerMask.NameToLayer("MoveObject");
            }

            grayscaleVolume.enabled = false;
            isGrayscale = false;
            selectedObject = false;
            rewindingObject = null;
            if (platformScript != null)
            {
                platformScript.platformRewinding = false;
                platformScript = null;
            }
            CheckGravOrKin();
            StartCoroutine(CooldownRewind());


        }
    }

    // SWAP POWERS

    IEnumerator SwapStart(GameObject hitobject, OutlineHighlighter outlineScript)
    {
        GameObject swapObject = hitobject;
        selectedObject = true;
        isGrayscale = true;
        grayscaleVolume.enabled = true;
        yield return new WaitForSeconds(0.7f);
        Vector3 newPlayerPosition = hitobject.transform.position;
        hitobject.transform.position = player.transform.position;
        player.transform.position = newPlayerPosition;
        swapObject = null;
        grabbedObject = null;
        outlineScript.UnselectedHighlight();
        selectedObject = false;
        grayscaleVolume.enabled = false;
        isGrayscale = false;
        hitobject.layer = LayerMask.NameToLayer("MoveObject");
        StartCoroutine(CooldownRewind());
    }
  /*  void StopVelocity(Rigidbody rb)
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
        }
    }*/
    IEnumerator CooldownRewind()
    {
        coolDownScript.SetMaxFill();
        isOnCoolDown = true;

        yield return StartCoroutine(coolDownScript.CoolDownRoutine(5f));
        isOnCoolDown = false;
    }
}
