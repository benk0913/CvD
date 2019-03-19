using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorState
{
    CharacterInfo CurrentCharacter;

    public int CurrentHP;

    public ActorState(CharacterInfo character)
    {
        this.CurrentCharacter = character;

    }

}
