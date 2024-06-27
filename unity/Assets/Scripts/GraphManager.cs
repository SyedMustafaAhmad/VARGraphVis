using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public partial class GraphManager : MonoBehaviour
{
    [Header("File Paths")]
    public string NodesFilePath;
    public string RelationshipsFilePath;

    [Header("Prefabs")]
    public GameObject NodePrefab;
    public GameObject EdgePrefab;
    public GameObject BoundingBoxPrefab; // Reference to the bounding box prefab
    private GameObject boundingBox; // Private reference to the instantiated bounding box

    [Header("Force-Directed Graph Settings")]
    public float MaxDistanceFromCenter = 5f; 
    public float RepulsiveForceConstant = 50f; // Reduced from 100
    public float SpringLength = 2f; // Adjusted from 1f
    public float SpringConstant = 0.05f; // Adjusted from 0.1f
    public float Damping = 0.9f; // Increased damping
    public int MaxIterationsFDG = 250;
    [SerializeField]
    [Tooltip("Load the Force-Directed Graph layout from disk if a saved layout exists.")]
     [Space(10)]
    private bool loadFDGFromDisk = false;

    [Header("Edge Bundling Settings")]
    public float K = 0.12f; // Base spring constant
    public int InitialIterations = 50;
    public float IDecrementFactor = 2f / 3f;
    public int Cycles = 6;
     [Space(10)]
    public float StepSizeInitial = 0.126f;
    public float StepDecrementFactor = 0.8f;
    public float CompatibilityThreshold = 0.44f;
    public int SubdivisionCount = 10;
    public int MaxIterations = 50;
    [Space(10)]
    [Range(3, 21)]
    public int WindowSize = 5;
    [Range(0.1f, 5f)]
    public float Sigma = 2f;

   

    private Dictionary<int, GraphNode> nodeMap = new Dictionary<int, GraphNode>();
    private List<GraphEdge> edges = new List<GraphEdge>(); // List to hold edge references

    void Start()
    {
        // Instantiate the bounding box and set it to be invisible
        boundingBox = Instantiate(BoundingBoxPrefab);
        boundingBox.SetActive(false);

        forces = new Dictionary<int, Vector3>();

        string savePath = Path.Combine(Application.persistentDataPath, "nodePositions.dat");

        List<Dictionary<string, string>> nodesData = CSVParser.Read(NodesFilePath);
        List<Dictionary<string, string>> relationshipsData = CSVParser.Read(RelationshipsFilePath);
        InitializeGraph(nodesData, relationshipsData);  
        if (File.Exists(savePath) && loadFDGFromDisk)
        {
            LoadGraphFromDisk(savePath);
            StartCoroutine(BundleEdges());
        }
        else
        {
            StartCoroutine(RunForceDirectedLayout(savePath));
        }
    }

    public void InitializeGraph(List<Dictionary<string, string>> nodesData, List<Dictionary<string, string>> relationshipsData)
    {
        foreach (var row in nodesData)
        {
            if (row.ContainsKey("id"))
            {
                int id = int.Parse(row["id"]);
                string title = row.ContainsKey("title") ? row["title"] : "";
                int year = row.ContainsKey("year") ? int.Parse(row["year"]) : 0;
                int nCitation = row.ContainsKey("n_citation") ? int.Parse(row["n_citation"]) : 0;
                string docType = row.ContainsKey("doc_type") ? row["doc_type"] : "";
                string publisher = row.ContainsKey("publisher") ? row["publisher"] : "";
                string doi = row.ContainsKey("doi") ? row["doi"] : "";

                Vector3 pos = Vector3.zero + Random.insideUnitSphere * MaxDistanceFromCenter;
                GameObject nodeObject = Instantiate(NodePrefab, pos, Quaternion.identity);
                GraphNode graphNode = nodeObject.AddComponent<GraphNode>();
                graphNode.Initialize(id, pos, title, year, nCitation, docType, publisher, doi);
                nodeMap[id] = graphNode;
            }
            else
            {
                Debug.LogError("Key 'id' not found in node data.");
            }
        }

        foreach (var row in relationshipsData)
        {
            if (row.ContainsKey("start") && row.ContainsKey("end"))
            {
                int sourceID = int.Parse(row["start"]);
                int targetID = int.Parse(row["end"]);

                if (nodeMap.ContainsKey(sourceID) && nodeMap.ContainsKey(targetID))
                {
                    GraphNode sourceNode = nodeMap[sourceID];
                    GraphNode targetNode = nodeMap[targetID];

                    GameObject edgeObject = Instantiate(EdgePrefab);
                    GraphEdge graphEdge = edgeObject.AddComponent<GraphEdge>();
                    graphEdge.Initialize(sourceNode, targetNode);
                    edges.Add(graphEdge); // Store the reference
                }
                else
                {
                    Debug.LogWarning($"Source node with ID {sourceID} or target node with ID {targetID} does not exist in node map.");
                }
            }
            else
            {
                Debug.LogError("Keys 'start' or 'end' not found in relationship data.");
            }
        }
    }

    public void SaveNodePositions(string filePath)
    {
        GraphData graphData = new GraphData();
        foreach (var node in nodeMap)
        {
            graphData.Nodes.Add(new NodeData(node.Key, node.Value.transform.position));
        }

        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Create(filePath))
        {
            bf.Serialize(file, graphData);
        }

        Debug.Log("Node positions saved.");
    }

    public void LoadGraphFromDisk(string filePath)
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(filePath, FileMode.Open))
            {
                GraphData graphData = (GraphData)bf.Deserialize(file);

                foreach (var nodeData in graphData.Nodes)
                {
                    Debug.Log(nodeData.ID);
                    if (nodeMap.ContainsKey(nodeData.ID))
                    {
                        nodeMap[nodeData.ID].transform.position = nodeData.Position.ToVector3();
                    }
                }

                // Update edge positions after loading nodes
                foreach (var edge in edges)
                {
                    edge.UpdatePosition();
                }
            }

            Debug.Log("Graph loaded from disk.");
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }
}

