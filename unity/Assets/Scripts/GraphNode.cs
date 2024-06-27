using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode : MonoBehaviour
{
    public int ID { get; private set; }
    public string Title { get; private set; }
    public int Year { get; private set; }
    public int NCitation { get; private set; }
    public string DocType { get; private set; }
    public string Publisher { get; private set; }
    public string Doi { get; private set; }

    public void Initialize(int id, Vector3 position, string title = "", int year = 0, int nCitation = 0, string docType = "", string publisher = "", string doi = "")
    {
        ID = id;
        Title = title;
        Year = year;
        NCitation = nCitation;
        DocType = docType;
        Publisher = publisher;
        Doi = doi;
        transform.position = position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
