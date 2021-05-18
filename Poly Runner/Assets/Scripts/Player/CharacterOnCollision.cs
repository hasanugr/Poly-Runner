using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOnCollision : MonoBehaviour
{
    [SerializeField] Character m_char;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Obstacle"))
            m_char.OnCharacterColliderHit(collision.collider);
    }
}
