using System;

public class GameHelper
{
    /// <summary>
    /// Checks if the object passed in is null and throws an 
    /// <see cref="ArgumentNullException"/> if it is. This logs the type of
    /// object passed in.
    /// </summary>
    /// <param name="t">The object to null check.</param>
    public static void IsNull<T>(T t)
    {
        if (t == null)
        {
            throw new ArgumentNullException(typeof(T).ToString());
        }
    }
}
