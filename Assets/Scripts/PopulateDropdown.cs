using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateDropdown : MonoBehaviour {

    // Use this for initialization
    public Dropdown dropdown;
    List<string> names = new List<string>();
    List<Drug> rows = new List<Drug>();
    private void Start()
    {
        rows = this.gameObject.GetComponent<DrugCSVManager>().GetRowList();
        PopulateList();
    }


    private void PopulateList()
    {
        foreach (Drug drug in rows)
        {
            names.Add(drug.DrugName + " " + drug.Concentration + " " + drug.ConcentrationUnit + " " + drug.AdditionalText + " (" + drug.Unit + ")");
        }
        dropdown.AddOptions(names);
    }
}
