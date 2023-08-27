using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void CmdDamage(float damage, string killer_name, string killed_name, string reason);
}