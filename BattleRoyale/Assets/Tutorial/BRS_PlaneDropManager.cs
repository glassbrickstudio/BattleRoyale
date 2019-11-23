using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRS_PlaneDropManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int MapSize = 10;
    [Range(1, 9)]
    public int DropZoneRange = 8;
    [Header("Plane Settings")]
    public GameObject BRS_PlaneSpawn;
    public float BRS_PlaneAltitude;
    public float BRS_PlaneAirspeed = 100f;

    private Vector3[] PD_L;
    private Vector3[] PD_R;
    public GameObject PlaneStart;
    public GameObject PlaneStop;
    private int startFlightIndex;
    private int endFlightIndex;
    public bool VerifiedPath = false;

    private void Start()
    {
        PD_L = new Vector3[9];
        PD_R = new Vector3[9];

        var _MapSize = MapSize * 1000;
        var setupPosition = new Vector3(-_MapSize, BRS_PlaneAltitude, _MapSize);

        for (int i = 0; i < PD_L.Length; i++)
        {
            PD_L[i] = setupPosition;
            setupPosition = new Vector3(-_MapSize, BRS_PlaneAltitude, (setupPosition.z - 1000));
        }

        setupPosition = new Vector3(_MapSize, BRS_PlaneAltitude, _MapSize);

        for (int i = 0; i < PD_R.Length; i++)
        {
            PD_R[i] = setupPosition;
            setupPosition = new Vector3(_MapSize, BRS_PlaneAltitude, (setupPosition.z - 1000));
        }
        VerifiedPath = false;
        SetupFlightPath();
    }

    private void SetupFlightPath()
    {
        int numberOfAttempts = 0;
        Vector3 startFlight;
        Vector3 endFlight;

        if (!VerifiedPath)
        {
            Debug.Log("Planing optimal Route");
            startFlightIndex = Random.Range(0, PD_L.Length);
            startFlight = PD_L[startFlightIndex];

            endFlightIndex = Random.Range(0, PD_R.Length);
            endFlight = PD_R[endFlightIndex];

            PlaneStart.transform.position = startFlight;
            PlaneStop.transform.position = endFlight;
            PlaneStart.transform.LookAt(PlaneStop.transform);

            RaycastHit objectHit;
            if (Physics.Raycast(PlaneStart.transform.position, PlaneStart.transform.forward, out objectHit, 10000))
            {
                Debug.Log("Trying " + numberOfAttempts++ + " times");
                if (objectHit.collider.gameObject.name == "AcceptableDropZone")
                {
                    VerifiedPath = true;
                    Debug.Log("Optimal Route Calculated");
                    GameObject.Destroy(objectHit.collider.gameObject);
                }
            }
        }
    }

    void LateUpdate()
    {
        if (VerifiedPath)
        {
            SpawnPlane();
        }
    }

    void SpawnPlane()
    {
        PlaneStart.transform.LookAt(PD_R[endFlightIndex]);
        Instantiate(BRS_PlaneSpawn, PlaneStart.transform.position, PlaneStart.transform.rotation);
        VerifiedPath = false;
    }
}