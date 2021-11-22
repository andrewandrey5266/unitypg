using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FiguresGenerator : MonoBehaviour
{
	private readonly System.Random _rnd = new System.Random();

	public void Generate()
	{
		Material[] mats = Resources.LoadAll("Materials/FigureColors", typeof(Material)).Cast<Material>().ToArray();
		List<int> colorNs = Enumerable.Range(0, mats.Length).ToList();

		int[,] figures = Generator.GetPuzzle(
			GameManager.Settings.Xlength,
			GameManager.Settings.Ylength,
			GameManager.Settings.FigureN,
			GameManager.Settings.MinSize,
			GameManager.Settings.MaxSize, out int resultFigureN);

		for (int i = 1; i <= GameManager.Settings.FigureN; i++)
		{
			var oldGo = GameObject.Find($"Figure{i}");
			DestroyImmediate(oldGo);
		}

		float  x = -2, y = -6, highestX = 6, highestY = 0;
		var figuresParent = GameObject.Find("Figures");
		for (int i = 1; i <= resultFigureN; i++)
		{
			var go = new GameObject($"Figure{i}");
			go.AddComponent<MeshFilter>();
			var renderer = go.AddComponent<MeshRenderer>();
			go.AddComponent<FigureHandler>();

			int randomI = _rnd.Next(0, colorNs.Count);
			renderer.material = mats[colorNs[randomI]];
			colorNs.RemoveAt(randomI);
			var collider = go.AddComponent<MeshCollider>();
			
			GenerateFigureMesh(go, figures, i);

			var cx = collider.bounds.size.x;
			var cy = collider.bounds.size.y;
			
			if (i == 1)
			{
				highestY = y + cy;
			}

			if (x + cx <= highestX)
			{
				if (highestY < y + cy)
				{
					highestY = y + cy;
				}
			}
			else
			{
				x = -2;
				y = highestY;
				
				if (highestY < y + cy)
				{
					highestY = y + cy;
				}
			}
			
			go.transform.position = new Vector3(x, y, -0.01f);
			
			x += cx;
			
			go.transform.parent = figuresParent.transform;
		}
	}
	
	private void GenerateFigureMesh(GameObject gameObject, int[,] figures, int i)
	{
		//figure out width, to set pivot offset, 
		int minXIndex = int.MaxValue;
		int minYIndex = int.MaxValue;

		for (int y = 0; y < figures.GetLength(0); y++)
		{
			for (int x = 0; x < figures.GetLength(1); x++)
			{
				if (figures[y, x] == i)
				{
					if (x < minXIndex)
					{
						minXIndex = x;
					}
					if (y < minYIndex)
					{
						minYIndex = y;
					}
				}
			}
		}

		List<Vector3> verticesList = new List<Vector3>();
		List<Vector3> triangles = new List<Vector3>();
		for (int y = 0; y < figures.GetLength(0); y++)
		{
			for (int x = 0; x < figures.GetLength(1); x++)
			{
				if (figures[y, x] == i)
				{
					float _x = x - minXIndex;
					float _y = y - minYIndex;
					
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
	
	public void Shuffle<T>(IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = _rnd.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}
