using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;
public class SpaceShipCollider : MonoBehaviour
{
    SpaceShip _spaceShip;
    // Start is called before the first frame update
    void Start()
    {
        _spaceShip = this.transform.parent.GetComponent<SpaceShip>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Ball" && _spaceShip.state == SpaceShipState.Finding)
        {
            _spaceShip.state = SpaceShipState.Catching;
            Ball ball = col.GetComponent<Ball>();
            ball.state = State.Stop;
            var colRigid = col.GetComponent<Rigidbody>();
            col.transform.SetParent(this.transform.parent);
            colRigid.useGravity = false;
            col.GetComponent<CapsuleCollider>().isTrigger = true;
            colRigid.velocity = Vector3.zero;
            _spaceShip.mCheckCoroutine = StartCoroutine(_spaceShip._cCheckCatchTime(col.gameObject));
            SoundManager.Instance.PlaySound(SoundType.SFBall_sfx_cube_0);

        }
    }

}
