using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootprint : MonoBehaviour
{
    [SerializeField] private Character m_char;

    public void FootPrint()
    {
        m_char.FootPrint();
    }
}
