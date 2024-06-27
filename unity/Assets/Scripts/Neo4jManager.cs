using System;
using System.Threading.Tasks;
using UnityEngine;
using Neo4j.Driver;

public class Neo4jManager : MonoBehaviour
{
    private IDriver _driver;
    private string uri = "neo4j+s://6095dc87.databases.neo4j.io:7687"; // Use +s for secure Bolt connection
    private string user = "neo4j";
    private string password = "APGMB_0ZIYaw8PFXq_Cy3utL7AoEClikHFR-xMaxk2E";

    void Start()
    {
        Debug.Log("skldfm");
        try
        {
            ConnectToNeo4j();
            // RunQuery().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to connect to Neo4j: {ex.Message}");
        }
    }

    private void ConnectToNeo4j()
    {
        try
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            var queryOperation = _driver.ExecutableQuery("MATCH (p:Paper) RETURN p LIMIT 5").ExecuteAsync();
            Debug.Log(queryOperation.Result);
            Debug.Log("sjkdsss");
            Debug.Log("Connected to Neo4j AuraDB");
           
        }
        catch (Exception ex)
        {
            Debug.LogError($"Connection error: {ex.Message}");
            // Additional logging here can help understand the nature of the connection failure
        }
    }

    private async Task RunQuery()
    {
        var queryOperation = await _driver.ExecutableQuery("MATCH (p:Paper) RETURN p LIMIT 5").ExecuteAsync();
        Debug.Log(queryOperation.Result);
        Debug.Log("sjkdsss");
        // var session = _driver.AsyncSession();
        // try
        // {
        //     var result = await session.RunAsync("MATCH (p:Paper) RETURN p LIMIT 5");
        //     var records = await result.ToListAsync();

        //     foreach (var record in records)
        //     {
        //         var node = record["p"].As<INode>();
        //         Debug.Log($"Paper: {node.Properties["title"]}");
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Debug.LogError($"Error: {ex.Message}");
        // }
        // finally
        // {
        //     await session.CloseAsync();
        // }
    }

    private void OnDestroy()
    {
        _driver?.Dispose();
    }

    
}
