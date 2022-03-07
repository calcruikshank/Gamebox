using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelector : MonoBehaviour
{
    [SerializeField] string methodToCall;
    [SerializeField] Transform transformToCallMethodFrom;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Select()
    {
        if (transformToCallMethodFrom == null)
        {
            Transform finalParent = Crutilities.singleton.GetFinalParent(this.transform);

            MonoBehaviour[] allScriptsInParent = finalParent.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour mb in allScriptsInParent)
            {
                mb.Invoke(methodToCall, 0f);
            }
        }

    }
}
