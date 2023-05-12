using UnityEngine;

public interface IMoveable
{
    void TryMove();
    void Initialize(Transform[] path);
}