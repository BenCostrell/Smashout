using UnityEngine;

public class TotalBounds
{
    public TotalBounds(GameObject parentObject)
    {
        Quaternion currentRotation = parentObject.transform.rotation;
        parentObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        this._bounds = new Bounds(parentObject.transform.position, Vector3.zero);

        foreach (Renderer renderer in parentObject.GetComponentsInChildren<Renderer>())
        {
            _bounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = _bounds.center - parentObject.transform.position;
        _bounds.center = localCenter;

        parentObject.transform.rotation = currentRotation;
    }

    public static implicit operator Bounds(TotalBounds b)
    {
        return b.bounds;
    }

    public Bounds bounds
    {
        get { return _bounds; }
    }

    private Bounds _bounds;
}
