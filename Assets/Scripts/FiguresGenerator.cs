using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Settings = GameManager.Settings;
public class FiguresGenerator : MonoBehaviour
{
	private readonly System.Random _rnd = new System.Random();

	public void Generate()
	{
		for (int i = 1; i <= 10; i++)
		{
			var oldGo = GameObject.Find($"Figure{i}");
			DestroyImmediate(oldGo);
		}
		
		int[,] figures = //JsonConvert.DeserializeObject<int[,]>("[[1,1,2,2,2,2],[1,1,2,2,3,2],[1,1,1,1,3,2],[1,1,1,1,3,2]]");
		
		Generator.GetPuzzle(
			Settings.Xlength,
			Settings.Ylength,
			Settings.FigureN,
			Settings.MinSize,
			Settings.MaxSize, out int resultFigureN);
		
		GameManager.CurrentPuzzle = figures;

		Debug.Log(JsonConvert.SerializeObject(figures));

		float x = Settings.Bounds.MinX;
		float y = Settings.Bounds.MinY;
		float highestX = Settings.Bounds.MaxX;
		float highestY = float.MinValue;
		
		GameObject figuresParent = GameObject.Find("Figures");

		Material[] mats = Resources.LoadAll("Materials/FigureColors", typeof(Material)).Cast<Material>().ToArray();
		List<int> colorNs = Enumerable.Range(0, mats.Length).ToList();
		List<int> figureNumbers = Enumerable.Range(1, resultFigureN).ToList();
		
		//should be in game manager, temporary fix
		GameManager.Figures = new List<GameObject>();
		for (int iteration = 1; iteration <= resultFigureN; iteration++)
		{
			var randomIndex = _rnd.Next(0, figureNumbers.Count);
			int figureNumber = figureNumbers[randomIndex];
			figureNumbers.RemoveAt(randomIndex);
			
			var go = new GameObject($"Figure{figureNumber}");
			go.AddComponent<MeshFilter>();
			var renderer = go.AddComponent<MeshRenderer>();
			go.AddComponent<FigureHandler>();

			int randomI = _rnd.Next(0, colorNs.Count);
			renderer.material = mats[colorNs[randomI]];
			colorNs.RemoveAt(randomI);
			MeshCollider collider = go.AddComponent<MeshCollider>();
			
			GenerateFigureMesh(go, figures, figureNumber);
			
			if (iteration == 1)
			{
				highestY = y + collider.bounds.size.y;
			}


			var position = GetCorrectPosition(figures, figureNumber);
			go.transform.position = position;
				//GetNextPosition(collider, figureNumber, ref x, ref y, ref highestX, ref highestY);
			
			go.transform.parent = figuresParent.transform;
			
			GameManager.Figures.Add(go);
		}
	}

	private Vector3 GetCorrectPosition(int[,] figures, int i)
	{
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

		return new Vector3(minXIndex, -minYIndex + Settings.Ylength, -0.01f);
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
					float _y = - y + minYIndex - 1;
					
					verticesList.Add(new Vector3(_x, _y, 0));
					verticesList.Add(new Vector3(_x + 1, _y, 0));
					verticesList.Add(new Vector3(_x, _y + 1, 0));
					verticesList.Add(new Vector3(_x + 1, _y + 1, 0));

					int p0 = verticesList.Count - 4;
					triangles.Add(new Vector3(p0 + 2, p0 + 1, p0));
					triangles.Add(new Vector3(p0 + 3, p0 + 1, p0 + 2));
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

	private Vector3 GetNextPosition(MeshCollider collider, int i, ref float x, ref float y, ref float highestX, ref float highestY)
	{
		float width = collider.bounds.size.x;
		float height = collider.bounds.size.y;
			
		if(x + width > highestX)
		{
			x = -2;
			y = highestY;
		}
		
		if (highestY < y + height)
		{
			highestY = y + height;
		}

		var vector = new Vector3(x, y, -0.01f);
		x += width;
		return vector;
	}
}
