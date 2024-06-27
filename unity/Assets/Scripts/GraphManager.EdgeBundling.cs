using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GraphManager
{
    private Dictionary<GraphEdge, List<GraphEdge>> edgeCompatibilityMap;

    public IEnumerator BundleEdges()
    {
        PrecomputeEdgeCompatibilities();

        foreach (GraphEdge edge in edges)
        {
            edge.Subdivide(SubdivisionCount);
        }

        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            float currentStepSize = StepSizeInitial * Mathf.Pow(StepDecrementFactor, iteration / (float)MaxIterations);
            ApplyBundlingForces(currentStepSize);
            // resample and smooth after bundling 
            foreach (GraphEdge edge in edges)
            {
                // edge.Resample(edge.lineRenderer.positionCount);
                edge.Smooth(WindowSize, Sigma);
            }
            yield return new WaitForSeconds(0.02f); // Small delay for each iteration
        }

        // Apply smoothing to each edge after bundling is complete
        // foreach (GraphEdge edge in edges)
        // {
        //     edge.Resample(edge.lineRenderer.positionCount);
        //     edge.Smooth(WindowSize, Sigma);
        // }

        // Once the bundling is done, smooth the lines
        // foreach (GraphEdge edge in edges)
        // {
        //     BezierLineRenderer bezier = edge.GetComponent<BezierLineRenderer>();
        //     if (bezier != null)
        //     {
        //         bezier.Smooth(10); // Adjust the interpolation steps as needed
        //     }
        // }

    }

    

    private void PrecomputeEdgeCompatibilities()
    {
        edgeCompatibilityMap = new Dictionary<GraphEdge, List<GraphEdge>>();
        foreach (GraphEdge edge in edges)
        {
            edgeCompatibilityMap[edge] = GetCompatibleEdges(edge);
        }
    }

    private List<GraphEdge> GetCompatibleEdges(GraphEdge edge)
    {
        List<GraphEdge> compatibleEdges = new List<GraphEdge>();

        foreach (GraphEdge otherEdge in edges)
        {
            if (otherEdge != edge)
            {
                float compatibility = CalculateEdgeCompatibility(edge, otherEdge);
                if (compatibility > CompatibilityThreshold)
                {
                    compatibleEdges.Add(otherEdge);
                }
            }
        }

        return compatibleEdges;
    }

    private void ApplyBundlingForces(float stepSize)
    {
        foreach (GraphEdge edge in edges)
        {
            for (int i = 1; i < edge.lineRenderer.positionCount - 1; i++)
            {
                Vector3 force = CalculateSpringForce(edge, i) + CalculateEdgeAttractionForceForCompatibleEdges(edge, i);
                if (force.magnitude > 0.01f)
                {
                    Vector3 newPosition = edge.lineRenderer.GetPosition(i) + stepSize * force;
                    edge.UpdatePointPosition(i, newPosition);
                }
            }
        }
    }

    private Vector3 CalculateSpringForce(GraphEdge edge, int index)
    {
        Vector3 force = Vector3.zero;

        float initialLength = Vector3.Distance(edge.lineRenderer.GetPosition(0), edge.lineRenderer.GetPosition(edge.lineRenderer.positionCount - 1));
        float k_P = K / (initialLength * (edge.lineRenderer.positionCount - 1));

        // Calculate spring force between subdivision points
        if (index > 0)
        {
            Vector3 direction = edge.lineRenderer.GetPosition(index - 1) - edge.lineRenderer.GetPosition(index);
            force += k_P * direction;
        }

        if (index < edge.lineRenderer.positionCount - 1)
        {
            Vector3 direction = edge.lineRenderer.GetPosition(index + 1) - edge.lineRenderer.GetPosition(index);
            force += k_P * direction;
        }

        return force;
    }

    private Vector3 CalculateEdgeAttractionForceForCompatibleEdges(GraphEdge edge, int index)
    {
        Vector3 force = Vector3.zero;
        List<GraphEdge> compatibleEdges = edgeCompatibilityMap[edge];

        foreach (GraphEdge otherEdge in compatibleEdges)
        {
            Vector3 direction = otherEdge.lineRenderer.GetPosition(index) - edge.lineRenderer.GetPosition(index);
            float distance = direction.magnitude;
            if (distance > 0)
            {
                Vector3 tf = direction.normalized / distance;
                if (tf.magnitude < distance)
                {
                    force += tf;
                }
            }
        }

        return force;
    }

    //    private Vector3 CalculateEdgeAttractionForce(GraphEdge edge, int index)
    // {
    //     // Calculate edge attraction force between corresponding subdivision points of different edges
    //     Vector3 force = Vector3.zero;

    //     foreach (GraphEdge otherEdge in edges)
    //     {
    //         if (otherEdge != edge)
    //         {
    //             Vector3 direction = otherEdge.lineRenderer.GetPosition(index) - edge.lineRenderer.GetPosition(index);
    //             float distance = direction.magnitude;
    //             if (distance > 0)
    //             {
    //                 float compatibility = CalculateEdgeCompatibility(edge, otherEdge);
    //                 force += compatibility * direction.normalized / (distance * distance); // Compatibility * (p_i - q_i) / |p_i - q_i|^2 from Holten's paper
    //             }
    //         }
    //     }

    //     return force;
    // }


    // compatibility calculations 

    private float CalculateEdgeCompatibility(GraphEdge edge1, GraphEdge edge2)
    {
        float angleComp = CalculateAngleCompatibility(edge1, edge2);
        float scaleComp = CalculateScaleCompatibility(edge1, edge2);
        float positionComp = CalculatePositionCompatibility(edge1, edge2);
        float visibilityComp = CalculateVisibilityCompatibility(edge1, edge2);

        return angleComp * scaleComp * positionComp * visibilityComp;
    }

    private float CalculateAngleCompatibility(GraphEdge edge1, GraphEdge edge2)
    {
        Vector3 edge1Dir = edge1.lineRenderer.GetPosition(edge1.lineRenderer.positionCount - 1) - edge1.lineRenderer.GetPosition(0);
        Vector3 edge2Dir = edge2.lineRenderer.GetPosition(edge2.lineRenderer.positionCount - 1) - edge2.lineRenderer.GetPosition(0);

        // Angle Compatibility: A(e1, e2) = |cos(θ)| = |(d1 • d2) / (|d1||d2|)|
        return Mathf.Abs(Vector3.Dot(edge1Dir.normalized, edge2Dir.normalized));
    }

    private float CalculateScaleCompatibility(GraphEdge edge1, GraphEdge edge2)
    {
        float len1 = Vector3.Distance(edge1.lineRenderer.GetPosition(0), edge1.lineRenderer.GetPosition(edge1.lineRenderer.positionCount - 1));
        float len2 = Vector3.Distance(edge2.lineRenderer.GetPosition(0), edge2.lineRenderer.GetPosition(edge2.lineRenderer.positionCount - 1));

        float lavg = (len1 + len2) * 0.5f;

        // Scale Compatibility: S(e1, e2) = 2 / (|lavg / len1| + |len2 / lavg|)
        return 2.0f / (lavg / Mathf.Min(len1, len2) + Mathf.Max(len1, len2) / lavg);
    }

    private float CalculatePositionCompatibility(GraphEdge edge1, GraphEdge edge2)
    {
        Vector3 mid1 = (edge1.lineRenderer.GetPosition(0) + edge1.lineRenderer.GetPosition(edge1.lineRenderer.positionCount - 1)) / 2.0f;
        Vector3 mid2 = (edge2.lineRenderer.GetPosition(0) + edge2.lineRenderer.GetPosition(edge2.lineRenderer.positionCount - 1)) / 2.0f;

        float len1 = Vector3.Distance(edge1.lineRenderer.GetPosition(0), edge1.lineRenderer.GetPosition(edge1.lineRenderer.positionCount - 1));
        float len2 = Vector3.Distance(edge2.lineRenderer.GetPosition(0), edge2.lineRenderer.GetPosition(edge2.lineRenderer.positionCount - 1));

        float lavg = (len1 + len2) * 0.5f;

        // Position Compatibility: P(e1, e2) = lavg / (lavg + |mid1 - mid2|)
        return lavg / (lavg + Vector3.Distance(mid1, mid2));
    }

    private float CalculateVisibilityCompatibility(GraphEdge edge1, GraphEdge edge2)
    {
        Vector3 mid1 = (edge1.lineRenderer.GetPosition(0) + edge1.lineRenderer.GetPosition(edge1.lineRenderer.positionCount - 1)) / 2.0f;
        Vector3 mid2 = (edge2.lineRenderer.GetPosition(0) + edge2.lineRenderer.GetPosition(edge2.lineRenderer.positionCount - 1)) / 2.0f;

        Vector3 dir1 = (edge1.lineRenderer.GetPosition(edge1.lineRenderer.positionCount - 1) - edge1.lineRenderer.GetPosition(0)).normalized;
        Vector3 dir2 = (edge2.lineRenderer.GetPosition(edge2.lineRenderer.positionCount - 1) - edge2.lineRenderer.GetPosition(0)).normalized;

        float visibility = Mathf.Min(CalculateVisibility(mid1, edge1, edge2), CalculateVisibility(mid2, edge2, edge1));

        return visibility;
    }

    private float CalculateVisibility(Vector3 point, GraphEdge edge1, GraphEdge edge2)
    {
        Vector3 dir1 = (edge1.lineRenderer.GetPosition(edge1.lineRenderer.positionCount - 1) - edge1.lineRenderer.GetPosition(0)).normalized;
        Vector3 dir2 = (edge2.lineRenderer.GetPosition(edge2.lineRenderer.positionCount - 1) - edge2.lineRenderer.GetPosition(0)).normalized;

        Vector3 inter1 = point + dir1 * Vector3.Distance(point, edge1.lineRenderer.GetPosition(0));
        Vector3 inter2 = point + dir2 * Vector3.Distance(point, edge2.lineRenderer.GetPosition(0));

        // Visibility Compatibility: V(e1, e2) = max(1 - 2*|inter1 - inter2| / |p_start - p_end|, 0)
        float visibility = Mathf.Max(1 - (2 * Vector3.Distance(inter1, inter2) / Vector3.Distance(edge1.lineRenderer.GetPosition(0), edge1.lineRenderer.GetPosition(edge1.lineRenderer.positionCount - 1))), 0);

        return visibility;
    }

}
