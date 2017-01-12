using UnityEngine;
using System.Collections.Generic;

public class BezierCurve {

    private const int points_between = 10;
    private Dictionary<int, long> factorialLookup;
    private Vector3[] points;
    private Vector3[] finerPoints;

    public BezierCurve (params Vector3[] _points)
    {
        factorialLookup = new Dictionary<int, long>();
        factorialLookup.Add(0, 1);
        factorialLookup.Add(1, 1);

        //need to add leading vector on front
        List<Vector3> listPoints = new List<Vector3>(_points);
        listPoints.Add(_points[0]);
        points = listPoints.ToArray();

        generatePoints();
    }

    private void generatePoints()
    {
        int total_points = points.Length * points_between;
        finerPoints = new Vector3[total_points];

        for(int i = 0; i < total_points; i++)
        {
            Debug.Log((float)i / total_points + "fff" );
            finerPoints[i] = pointAt((float)i / total_points);
        }
    }

    private Vector3 pointAt(float t)
    {
        t = Mathf.Clamp01(t);
        int n = points.Length;
        Vector3 sum = Vector3.zero;

        for(int i = 0; i < n; i++)
        {
            sum += bernsteinPoly(n - 1, i, t) * points[i];
        }

        return sum;
    }

    private float bernsteinPoly(int n, int i, float t)
    {
        return binomCoeff(n, i) * Mathf.Pow(t, i) * Mathf.Pow(1 - i, n - i);
    }

    private float binomCoeff(int n, int i)
    {
        return factorial(n) / (factorial(i) * factorial(n - i));
    }

    public long factorial(int n)
    {
        long num;
        if (!factorialLookup.ContainsKey(n))
        {
            num = n * factorial(n - 1);
            factorialLookup.Add(n, num);
            Debug.Log("Added: " + n + " " + num);
            return num;
        }
        else
        {
            num = factorialLookup[n];
            Debug.Log("from cache: " + n + "  " + num);
            return num;
        }
    }

    /*
     * This method is used for debuging the bezier curve
     * https://en.wikipedia.org/wiki/B%C3%A9zier_curve
     */
    public void drawPath()
    {
        for (int i = 0; i < finerPoints.Length - 1; i++)
        {
            DrawLine(finerPoints[i], finerPoints[i + 1], Color.red);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.name = "Bezier Line";
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(2f, 2f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

}
