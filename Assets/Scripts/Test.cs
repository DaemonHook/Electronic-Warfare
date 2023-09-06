using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var text = Resources.Load<TextAsset>("SamplePackage/Maps/sampleSheetUnitDef");
        Debug.Log(text);
        var csv = new CSVDocument(text.text);
        foreach (var h in csv.Headers)
        {
            Debug.Log(h);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
