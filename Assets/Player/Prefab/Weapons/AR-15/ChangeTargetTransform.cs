using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ChangeTargetTransform : MonoBehaviour
{
    [SerializeField] TwoBoneIKConstraint twoBoneIKConstraintDatas;
    [SerializeField] Transform realodClipGrip;
    [SerializeField] Transform leftRifleGrip;

    public void SetRealodTargetTransform() {
        twoBoneIKConstraintDatas.data.target = realodClipGrip;
    }
}
