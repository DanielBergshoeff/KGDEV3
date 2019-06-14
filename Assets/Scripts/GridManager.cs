using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    public List<Grid> grids;
    public List<List<Node>> nodeLists;

    public static GridManager Instance;

	// Use this for initialization
	void Start () {
        if (Instance == null)
            Instance = this;
        grids = new List<Grid>();
        nodeLists = new List<List<Node>>();
	}

    public Node GetRandomNode() {
        for (int i = 0; i < grids.Count; i++) {
            nodeLists.Add(new List<Node>());
        }

        int totalOptions = 0;
        for (int i = 0; i < grids.Count; i++) {
            nodeLists[i] = grids[i].GetAvailableNodes();
            totalOptions += nodeLists[i].Count;
        }
        int rnd = UnityEngine.Random.Range(0, totalOptions);
        for (int i = 0; i < grids.Count; i++) {
            if(nodeLists[i].Count > rnd) {
                return nodeLists[i][rnd];
            }
            else {
                rnd -= nodeLists[i].Count;
            }
        }

        return null;
    }
}
