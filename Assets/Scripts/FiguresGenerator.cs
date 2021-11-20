using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FiguresGenerator : MonoBehaviour
{
	int xlength = 6; 
	int ylength = 4;
	int figureN = 8;
	int minSize = 3;
	public static int maxSize = 7;
	
	private readonly System.Random _rnd = new System.Random();

	public void Generate()
	{
		Material[] mats = Resources.LoadAll("Materials/FigureColors", typeof(Material)).Cast<Material>().ToArray();
		List<int> colorNs = Enumerable.Range(0, mats.Length).ToList();

		var figures = Generator.GetPuzzle(xlength, ylength, figureN, minSize, maxSize, out int resultFigureN);

		for (int i = 1; i <= figureN; i++)
		{
			var oldGo = GameObject.Find($"Figure{i}");
			DestroyImmediate(oldGo);
		}

		for (int i = 1; i <= resultFigureN; i++)
		{
			var go = new GameObject($"Figure{i}");
			go.transform.position = Vector3.down * 5 + Vector3.forward * FigureHandler.ZStep * i;
			go.AddComponent<MeshFilter>();
			var renderer = go.AddComponent<MeshRenderer>();
			
			int randomI = _rnd.Next(0, colorNs.Count);
			renderer.material = mats[colorNs[randomI]];
			colorNs.RemoveAt(randomI);

			go.AddComponent<MeshCollider>();
			GenerateFigureMesh(go, figures, i);

			go.AddComponent<FigureHandler>();
		}
	}
	
	private void GenerateFigureMesh(GameObject gameObject, int[,] figures, int i)
	{
		bool firstPointDrawn = false;
		int offsetx = 0;
		int offsety = 0;
		
		List<Vector3> verticesList = new List<Vector3>();
		List<Vector3> triangles = new List<Vector3>();
		for (int y = 0; y < figures.GetLength(0); y++)
		{
			for (int x = 0; x < figures.GetLength(1); x++)
			{
				if (figures[y, x] == i)
				{
					if (!firstPointDrawn)
					{
						firstPointDrawn = true;
						offsetx = x;
						offsety = y;
					}
					
					int _x = x - offsetx;
					int _y = y - offsety;

					verticesList.Add(new Vector3(_x, _y, 0));
					verticesList.Add(new Vector3(_x + 1, _y, 0));
					verticesList.Add(new Vector3(_x, _y + 1, 0));
					verticesList.Add(new Vector3(_x + 1, _y + 1, 0));

					int c = verticesList.Count - 4;
					triangles.Add(new Vector3(c + 2, c + 1, c));
					triangles.Add(new Vector3(c + 3, c + 1, c + 2));
				}
			}
		}
      
		int[] trianglesArray = triangles
			.Select(x => new[] { (int)x.x, (int)x.y, (int)x.z })
			.SelectMany(x => x).ToArray();
     
		var meshFilter = gameObject.GetComponent<MeshFilter>();

		Mesh mesh = new Mesh
		{
			vertices = verticesList.ToArray(),
			triangles = trianglesArray
		};
     
		mesh.RecalculateNormals ();
		meshFilter.sharedMesh = mesh;

		gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
	}
}
