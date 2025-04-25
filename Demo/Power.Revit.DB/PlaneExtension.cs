namespace Power.Revit.DB;

public static class PlaneExtension
{
    public static Plane CreatePlane(this XYZ normal, XYZ origin)
    {
        if (normal is null)
            throw new ArgumentNullException(nameof(normal));

        if (origin is null)
            throw new ArgumentNullException(nameof(origin));

        // Don't know how to do it.
#if R19_0_OR_GREATER
        return new Plane(normal, origin);

#elif R25_0_OR_GREATER
        return Plane.CreateByNormalAndOrigin(normal, origin);

#endif
    }
}