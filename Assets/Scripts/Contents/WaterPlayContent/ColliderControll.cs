using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using CellBig;

public class ColliderControll : MonoBehaviour
{
    public Collider TriggerCollider = null;
    public ObiSolver solver;
    public Rigidbody rigid;
    public SoundType PlaySound;

    public void Awake()
    {
        
        solver = Transform.FindObjectOfType<ObiSolver>();
        if (TriggerCollider == null)
            TriggerCollider = transform.GetComponent<Collider>();
    }

    void OnEnable()
    {
        if (SoundManager.Instance.IsPlaySound(PlaySound))
            SoundManager.Instance.StopSound(PlaySound);
        if (solver != null)
            solver.OnCollision += Solver_OnCollision;
        if (TriggerCollider == null)
            TriggerCollider = transform.GetComponent<Collider>();
        rigid.isKinematic = true;
    }

    void OnDisable()
    {
        if (solver != null)
            solver.OnCollision -= Solver_OnCollision;
        if (TriggerCollider == null)
            TriggerCollider = transform.GetComponent<Collider>();
        
    }

    void Solver_OnCollision(object sender, Obi.ObiSolver.ObiCollisionEventArgs e)
    {
        for (int i = 0; i < e.contacts.Count; ++i)
        {
            if (e.contacts.Data[i].distance < 0.001f)
            {
                Component collider;
                if (ObiCollider.idToCollider.TryGetValue(e.contacts.Data[i].other, out collider))
                {
                    if (collider == TriggerCollider)
                    {
                        if (rigid.isKinematic)
                        {
                            if (!SoundManager.Instance.IsPlaySound(PlaySound))
                                SoundManager.Instance.PlaySound(PlaySound);
                        }
                        rigid.isKinematic = false;
                    }
                }
            }
        }
    }

}
