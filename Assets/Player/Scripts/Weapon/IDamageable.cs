using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void CmdDamage(float damage, PlayerComponents killer_name, PlayerComponents killed_name, string reason);
}