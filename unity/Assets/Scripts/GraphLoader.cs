using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GraphLoader : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject edgePrefab;
    public float forceStrength = 10f;
    public float damping = 0.9f;

    private Dictionary<int, GameObject> nodes = new Dictionary<int, GameObject>();
    private List<Edge> edges = new List<Edge>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGraph();
            StartCoroutine(ApplyForceDirectedLayout());
        }
    }

    private void LoadGraph()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "cit_patents-short.csv");
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                bool isFirstLine = true;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }
                    var values = line.Split(',');
                    int fromNode = int.Parse(values[0]);
                    int toNode = int.Parse(values[1]);
                    CreateNode(fromNode);
                    CreateNode(toNode);
                    CreateEdge(fromNode, toNode);
                }
            }
        }
        else
        {
            Debug.LogError("CSV file not found at " + path);
        }
    }

    private void CreateNode(int id)
    {
        if (!nodes.ContainsKey(id))
        {
            GameObject node = Instantiate(nodePrefab, Random.insideUnitSphere * 5f, Quaternion.identity);
            node.name = "Node " + id;
            nodes[id] = node;
        }
    }

    private void CreateEdge(int fromNode, int toNode)
    {
        GameObject fromNodeObject = nodes[fromNode];
        GameObject toNodeObject = nodes[toNode];
        if (edgePrefab != null)
        {
            GameObject edge = Instantiate(edgePrefab);
            Edge edgeComponent = edge.GetComponent<Edge>();
            if (edgeComponent != null)
            {
                edgeComponent.Initialize(fromNodeObject, toNodeObject);
                edges.Add(edgeComponent);
            }
            else
            {
                Debug.LogError("Edge component not found on instantiated edge prefab.");
            }
        }
        else
        {
            Debug.LogError("Edge prefab is not assigned.");
        }
    }

    private IEnumerator<WaitForSeconds> ApplyForceDirectedLayout()
    {
        while (true)
        {
            foreach (var edge in edges)
            {
                Vector3 direction = edge.toNode.transform.position - edge.fromNode.transform.position;
                float distance = direction.magnitude;
                Vector3 force = forceStrength * direction / distance;

                edge.fromNode.GetComponent<Rigidbody>().AddForce(force);
                edge.toNode.GetComponent<Rigidbody>().AddForce(-force);
            }

            foreach (var node in nodes.Values)
            {
                var rb = node.GetComponent<Rigidbody>();
                rb.velocity *= damping;
            }

            yield return new WaitForSeconds(0.02f);
        }
    }
}

public class Edge : MonoBehaviour
{
    public GameObject fromNode;
    public GameObject toNode;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    public void Initialize(GameObject from, GameObject to)
    {
        fromNode = from;
        toNode = to;
    }

    private void Update()
    {
        if (fromNode != null && toNode != null)
        {
            lineRenderer.SetPosition(0, fromNode.transform.position);
            lineRenderer.SetPosition(1, toNode.transform.position);
        }
    }
}
