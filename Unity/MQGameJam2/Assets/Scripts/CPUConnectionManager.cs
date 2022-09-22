using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUConnectionManager : MonoBehaviour
{
    public static CPUConnectionManager instance;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform[] connectionPoints;
    [SerializeField] private LineRenderer cablePrefab;

    [SerializeField] private List<Transform[]> connectedPoints = new List<Transform[]>();

    private int currentLine;
    private int correctConnections;

    private float timer;

    private List<LineRenderer> cables = new List<LineRenderer>();
    private Color[] colors = new Color[] {
        new Color(255,0,0),
        new Color(0,255,0),
        new Color(0,0,255),
        new Color(255,255,0),
        new Color(0,255,255),
        new Color(255,0,255),
        new Color(255,255,255),
        new Color(0,0,0)
    };

    bool activated;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < connectionPoints.Length / 2; i++)
        {
            LineRenderer lr = Instantiate(cablePrefab, transform.position, Quaternion.identity);
            lr.positionCount = 0;

            cables.Add(lr);
        }

        CreateConnections();
    }

    private Transform selectedConnection;

    public void Activate()
    {
        timer = Time.time;
        activated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.layer == 7)
                    {
                        Debug.Log(hit.transform.name);
                        selectedConnection = hit.transform;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform != null && selectedConnection != null && hit.transform.gameObject.layer == 7 && hit.transform != selectedConnection)
                    {
                        cables[currentLine].positionCount = 2;
                        cables[currentLine].SetPositions(new Vector3[] { selectedConnection.position, hit.transform.position });
                        cables[currentLine].startColor = selectedConnection.GetComponent<MeshRenderer>().material.color;
                        cables[currentLine].endColor = hit.transform.GetComponent<MeshRenderer>().material.color;

                        Transform[] connIndex = GetConnectionFromPoint(hit.transform);

                        if (hit.transform == connIndex[0] || hit.transform == connIndex[1])
                        {
                            if (selectedConnection == connIndex[0] || selectedConnection == connIndex[1])
                            {
                                correctConnections++;
                            }
                        }

                        currentLine++;

                        if (currentLine >= cables.Count)
                        {
                            Debug.Log("Completed cables: " + (correctConnections / (float)connectedPoints.Count * 100) + "%");

                            GameStateManager.instance.TowerStats((correctConnections / (float)connectedPoints.Count * 100), Time.time - timer);
                            GameStateManager.instance.SetGameState(GameState.MONITOR);
                            activated = false;
                        }
                        selectedConnection = null;
                    }
                }
            }
        }
    }

    Transform[] GetConnectionFromPoint(Transform connection)
    {
        for (int i = 0; i < connectedPoints.Count; i++)
        {
            for (int j = 0; j < connectedPoints[i].Length; j++)
            {
                if (connection == connectedPoints[i][j])
                {
                    return connectedPoints[i];
                }
            }
        }
        return null;
    }

    public void ResetConnections()
    {
        for(int i =0; i < cables.Count; i++)
        {
            cables[i].positionCount = 0;
        }

        currentLine = 0;
        correctConnections = 0;
        connectedPoints.Clear();

        CreateConnections();
    }

    void CreateConnections()
    {
        List<Transform> remainingConnections = new List<Transform>();
        remainingConnections.AddRange(connectionPoints);

        int currLR = 0;
        while (remainingConnections.Count > 0)
        {
            int rndIndex1 = Random.Range(0, remainingConnections.Count);
            int rndIndex2 = rndIndex1;
            while (rndIndex2 == rndIndex1)
            {
                rndIndex2 = Random.Range(0, remainingConnections.Count);
            }

            remainingConnections[rndIndex1].GetComponent<MeshRenderer>().material.color = colors[currLR];
            remainingConnections[rndIndex2].GetComponent<MeshRenderer>().material.color = colors[currLR];

            Transform p1 = remainingConnections[rndIndex1];
            Transform p2 = remainingConnections[rndIndex2];

            connectedPoints.Add(new Transform[] { p1, p2 });
            currLR++;

            remainingConnections.Remove(p1);
            remainingConnections.Remove(p2);
        }
    }
}
