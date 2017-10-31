using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boogaloo : Move
{

    Boogaloo(string name, Color col, int range, int pushPower) : base(name, col, range, pushPower)
    {

    }

    public override List<Dancer> CheckRange()
    {
        return null;
    }
}
