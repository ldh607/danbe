using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using CellBig;
using UnityEngine.SceneManagement;
public class ColliderEffect : MonoBehaviour
{
    [System.Serializable]
    public class EffectData
    {
        public GameObject effectObj;
        public float EnterTime;
        public float ExitTime;
        public bool IsEffSoundActive = false;
        public SoundType ActiveSound;
    }

    public Collider TriggerCollider = null;
    public List<EffectData> Effects = new List<EffectData>();

    int StayChack;
    public ObiSolver solver;
    Coroutine Checker;

    public void Awake()
    {
        Debug.Log("awake" + this.name);
        solver = Transform.FindObjectOfType<ObiSolver>();
        if (TriggerCollider == null)
            TriggerCollider = transform.GetComponent<Collider>();
    }

    void OnEnable()
    {

        //Debug.Log("OnEnable" + this.name);
        if (solver != null)
            solver.OnCollision += Solver_OnCollision;
        if (TriggerCollider == null)
        {
            TriggerCollider = transform.GetComponent<Collider>();
        }
        if (Checker == null)
            Checker = StartCoroutine(TimmerCheck());

        foreach (var item in Effects)
        {
            item.effectObj.SetActive(false);
        }
    }

    void OnDisable()
    {
        //Debug.Log("OnDisable"+this.name);
        if (solver != null)
            solver.OnCollision -= Solver_OnCollision;
        if (TriggerCollider == null)
            TriggerCollider = transform.GetComponent<Collider>();
        if (Checker != null)
        {
            StopCoroutine(Checker);
            Checker = null;
        }
    }

    void Solver_OnCollision(object sender, Obi.ObiSolver.ObiCollisionEventArgs e)
    {
        test++;
        if (test >= 6)
        {
            test = 0;
            for (int i = 0; i < e.contacts.Count; ++i)
            {
                if (e.contacts.Data[i].distance < 0.001f)
                {
                    Component collider;
                    if (ObiCollider.idToCollider.TryGetValue(e.contacts.Data[i].other, out collider))
                    {
                        if (collider == TriggerCollider)
                        {
                            StayChack++;
                        }
                    }
                }
            }
        }
    }
    int test = 0;
    IEnumerator TimmerCheck()
    {
        float EnterTimmer = 0;
        float ExitTimmer = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (StayChack > 0)
            {
                EnterTimmer += 0.1f;
                foreach (var item in Effects)
                {
                    if (item.EnterTime >= 0 && item.EnterTime < EnterTimmer)
                    {
                        item.effectObj.SetActive(true);
                        if (item.ActiveSound != SoundType.None && !item.IsEffSoundActive)
                        {
                            Message.Send<CellBig.UI.Event.GimmicActive>(new CellBig.UI.Event.GimmicActive());
                            item.IsEffSoundActive = true;
                            if (CellBig.Scene.SceneManager.Instance.nowScene != CellBig.Constants.SceneName.SelectGameScene)
                                SoundManager.Instance.PlaySound(item.ActiveSound);
                        }
                    }
                }
                ExitTimmer = 0;
            }
            else
            {
                ExitTimmer += 0.1f;
                foreach (var item in Effects)
                {
                    if (item.ExitTime >= 0 && item.ExitTime < ExitTimmer)
                    {
                        item.effectObj.SetActive(false);
                        if (item.ActiveSound != SoundType.None && item.IsEffSoundActive)
                        {
                            item.IsEffSoundActive = false;
                            SoundManager.Instance.StopSound(item.ActiveSound);
                        }
                    }
                }
                if (ExitTimmer > 0.5f)
                    EnterTimmer = 0;
            }
            StayChack = 0;
        }
    }
}
